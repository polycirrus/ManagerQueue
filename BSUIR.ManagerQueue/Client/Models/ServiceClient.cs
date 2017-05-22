using BSUIR.ManagerQueue.Client.Exceptions;
using BSUIR.ManagerQueue.Client.Properties;
using BSUIR.ManagerQueue.Infrastructure.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Client.Models
{
    public class ServiceClient
    {
        #region Instance

        public static readonly Lazy<ServiceClient> Instance = new Lazy<ServiceClient>(CreateInstance);

        private static ServiceClient CreateInstance() => new ServiceClient();

        #endregion

        private static class ResourceUri
        {
            public static readonly string Token = "/Token";
        }

        private static readonly string ServiceUriSettingName = "ServiceUri";

        private Lazy<HttpClient> httpClientInstance = new Lazy<HttpClient>(CreateHttpClient);
        private HttpClient HttpClient => httpClientInstance.Value;

        private TokenEndpointGrantResponse TokenInfo { get; set; }

        private ServiceClient()
        {
        }

        public async Task SignIn(string userName, string password)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", userName),
                new KeyValuePair<string, string>("password", password)
            });

            var response = await HttpClient.PostAsync(ResourceUri.Token, content);

            if (!response.IsSuccessStatusCode)
            {
                TokenEndpointErrorResponse error = null;
                try
                {
                    error = await response.Content.ReadAsAsync<TokenEndpointErrorResponse>();
                }
                catch (Exception)
                {
                }

                if (error != null && !string.IsNullOrEmpty(error.ErrorDescription))
                    throw new SignInFailedException(error.ErrorDescription);

                response.EnsureSuccessStatusCode();
            }

            try
            {
                var tokenInfo = await response.Content.ReadAsAsync<TokenEndpointGrantResponse>();
                if (string.IsNullOrEmpty(tokenInfo.AccessToken))
                    throw new ServiceCommunicationException();
                TokenInfo = tokenInfo;
            }
            catch (Exception exception)
            {
                throw new ServiceCommunicationException(Resources.SignInErrorExceptionMessage, exception);
            }
        }

        private static HttpClient CreateHttpClient()
        {
            var serviceUriSetting = ConfigurationManager.AppSettings[ServiceUriSettingName];
            if (string.IsNullOrEmpty(serviceUriSetting))
                throw new ServiceCommunicationException(Resources.ServiceUriNotSetExceptionMessage);

            Uri serviceUri;
            if (!Uri.TryCreate(serviceUriSetting, UriKind.RelativeOrAbsolute, out serviceUri))
                throw new ServiceCommunicationException(Resources.InvalidServiceUriExceptionMessage);

            var httpClient = new HttpClient();
            httpClient.BaseAddress = serviceUri;

            return httpClient;
        }

        #region Models

        private class TokenEndpointErrorResponse
        {
            [JsonProperty("error")]
            public string Error { get; set; }
            [JsonProperty("error_description")]
            public string ErrorDescription { get; set; }
        }

        private class TokenEndpointGrantResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
            [JsonProperty("token_type")]
            public string TokenType { get; set; }
            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
            [JsonProperty("userName")]
            public string UserName { get; set; }
            [JsonProperty(".issued")]
            public DateTime Issued { get; set; }
            [JsonProperty(".expires")]
            public DateTime Expires { get; set; }
        }

        #endregion
    }
}
