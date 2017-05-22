using BSUIR.ManagerQueue.Client.Commands;
using BSUIR.ManagerQueue.Client.Models;
using BSUIR.ManagerQueue.Client.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    public class SignInViewModel : BaseViewModel
    {
        private static ServiceClient ServiceClient => ServiceClient.Instance.Value;

        #region Properties

        private string userName;
        public string UserName
        {
            get
            {
                return userName;
            }

            set
            {
                userName = value;
                NotifyPropertyChanged(nameof(UserName));
            }
        }

        private PasswordBox passwordBox;
        public PasswordBox PasswordBox
        {
            get
            {
                return passwordBox;
            }

            set
            {
                passwordBox = value;
                NotifyPropertyChanged(nameof(PasswordBox));
                InitializeForm();
            }
        }

        private bool rememberMe;
        public bool RememberMe
        {
            get
            {
                return rememberMe;
            }

            set
            {
                rememberMe = value;
                NotifyPropertyChanged(nameof(RememberMe));
            }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }

            set
            {
                isBusy = value;
                NotifyPropertyChanged(nameof(IsBusy));
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                errorMessage = value;
                NotifyPropertyChanged(nameof(ErrorMessage));
            }
        }

        #endregion

        #region Commands

        public ICommand SignInCommand => new AsyncDelegateCommand(SignIn);

        public ICommand RegisterCommand => new AsyncDelegateCommand(Register);

        #endregion

        public event EventHandler SignInSucceded;

        private async Task SignIn()
        {
            ErrorMessage = null;
            IsBusy = true;
            
            try
            {
                await ServiceClient.SignIn(UserName, PasswordBox.Password);
            }
            catch (Exception exception)
            {
                ErrorMessage = exception.Message;
                IsBusy = false;
                return;
            }

            if (RememberMe)
                CredentialManager.SaveCredential(UserName, PasswordBox.SecurePassword);

            IsBusy = false;
            ShowMainWindow();
            SignInSucceded?.Invoke(this, EventArgs.Empty);
        }

        private async Task Register()
        {
            var registrationWindow = new RegistrationWindow();
            registrationWindow.ShowDialog();
        }

        private void ShowMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.Activate();
        }

        private void InitializeForm()
        {
            var credential = CredentialManager.GetCredential();
            if (credential != null)
            {
                RememberMe = true;
                UserName = credential.Item1;
                PasswordBox.Password = credential.Item2;
            }
        }
    }
}
