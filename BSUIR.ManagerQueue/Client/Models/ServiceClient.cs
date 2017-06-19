using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace BSUIR.ManagerQueue.Client.Models
{
    using Data.Model;
    using Exceptions;
    using Infrastructure.Models;
    using Properties;

    public class ServiceClient
    {
        #region Constants

        private static class ResourceUri
        {
            public static class Account
            {
                public static readonly string Base = "/api/Account";
                public static readonly string Register = Base + "/Register";
                public static readonly string QueueOwners = Base + "/QueueOwners";
            }

            public static class Queue
            {
                public static readonly string Base = "/api/Queue";
                public static readonly string Entry = "/api/Queue/Entry";
            }

            public static readonly string Token = "/Token";
            public static readonly string Position = "/api/Position";
        }

        private static class Parameters
        {
            public static readonly string Id = "id";
        }

        private static readonly string ServiceUriSettingName = "ServiceUri";

        #endregion

        private Lazy<HttpClient> httpClientInstance = new Lazy<HttpClient>(CreateHttpClient);
        private HttpClient HttpClient => httpClientInstance.Value;

        private TokenEndpointGrantResponse TokenInfo { get; set; }

        public Employee CurrentUser { get; set; }

        private ServiceClient()
        {
        }

        #region Account

        public async Task SignIn(string userName, string password)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", userName),
                new KeyValuePair<string, string>("password", password)
            });

            HttpResponseMessage response;
            try
            {
                response = await HttpClient.PostAsync(ResourceUri.Token, content);
            }
            catch (HttpRequestException exception)
            {
                throw new ServiceCommunicationException(Resources.ConnectionFailedExceptionMessage, exception);
            }

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

            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenInfo.AccessToken);

            await UpdateCurrentUser();
        }

        public async Task UpdateCurrentUser()
        {
            try
            {
                CurrentUser = await Get<Employee>(ResourceUri.Account.Base);
            }
            catch (HttpRequestException exception)
            {
                throw new RequestFailedException(Resources.UserFetchFailedExceptionMessage, exception);
            }

            if (CurrentUser == null)
                throw new RequestFailedException(Resources.UserFetchFailedExceptionMessage);
        }

        public async Task Register(RegisterBindingModel model)
        {
            try
            {
                await Post(ResourceUri.Account.Register, model);
            }
            catch (HttpRequestException exception)
            {
                throw new RequestFailedException(Resources.RegistrationFailedExceptionMessage, exception);
            }
        }

        public async Task<IEnumerable<Employee>> GetQueueOwners()
        {
            return await Get<IEnumerable<Employee>>(ResourceUri.Account.QueueOwners);
        }

        #endregion

        #region Position

        public async Task<IEnumerable<Position>> GetPositions()
        {
            return await Get<IEnumerable<Position>>(ResourceUri.Position);
        }

        public async Task<Position> CreatePosition(Position position)
        {
            try
            {
                return await Post(ResourceUri.Position, position);
            }
            catch (HttpRequestException exception)
            {
                throw new RequestFailedException(string.Format(Resources.UnableToCreateErrorMessageTemplate, Resources.PositionFieldLabel), exception);
            }
        }

        #endregion

        #region Queue

        public async Task<IEnumerable<QueueItem>> GetQueue(int id)
        {
            return await Get<IEnumerable<QueueItem>>($"{ResourceUri.Queue.Base}/{id}");
        }

        public async Task<IEnumerable<QueueItem>> SaveQueue(int id, IEnumerable<QueueItem> queue)
        {
            return await Post($"{ResourceUri.Queue.Base}/{id}", queue);
        }

        public async Task AddEntry(AddQueueEntryModel model)
        {
            await Post(ResourceUri.Queue.Entry, model);
        }

        public async Task DeleteEntry(int queueItemId)
        {
            HttpResponseMessage response;
            try
            {
                response = await HttpClient.DeleteAsync($"{ResourceUri.Queue.Entry}/{queueItemId}");
            }
            catch (HttpRequestException exception)
            {
                throw new ServiceCommunicationException(Resources.ConnectionFailedExceptionMessage, exception);
            }

            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Generic

        private async Task<T> Get<T>(string resourceUri)
        {
            HttpResponseMessage response;
            try
            {
                response = await HttpClient.GetAsync(resourceUri);
            }
            catch (HttpRequestException exception)
            {
                throw new ServiceCommunicationException(Resources.ConnectionFailedExceptionMessage, exception);
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        private async Task<T> Post<T>(string resourceUri, T item)
        {
            HttpResponseMessage response;
            try
            {
                response = await HttpClient.PostAsJsonAsync(resourceUri, item);
            }
            catch (HttpRequestException exception)
            {
                throw new ServiceCommunicationException(Resources.ConnectionFailedExceptionMessage, exception);
            }

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadAsAsync<T>();
                }
                catch
                {
                    return default(T);
                }
            }

            InvalidModelResponse modelResponse = null;
            try
            {
                modelResponse = await response.Content.ReadAsAsync<InvalidModelResponse>();
            }
            catch (Exception)
            {
            }

            if (modelResponse != null)
            {
                if (modelResponse.ModelState != null && modelResponse.ModelState.Any())
                    throw new RequestFailedException(string.Join(Environment.NewLine, modelResponse.ModelState.SelectMany(kvp => kvp.Value)));
                if (!string.IsNullOrEmpty(modelResponse.Message))
                    throw new RequestFailedException(modelResponse.Message);
            }

            response.EnsureSuccessStatusCode();
            return default(T);
        }

        #endregion

        #region Instance

        public static readonly Lazy<ServiceClient> Instance = new Lazy<ServiceClient>(CreateInstance);

        private static ServiceClient CreateInstance() => new ServiceClient();

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

        #endregion

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

        private class InvalidModelResponse
        {
            public string Message { get; set; }
            public IDictionary<string, IEnumerable<string>> ModelState { get; set; }
        }

        #endregion
    }
}
