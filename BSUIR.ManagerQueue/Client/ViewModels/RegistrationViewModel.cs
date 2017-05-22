using BSUIR.ManagerQueue.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    public class RegistrationViewModel : BaseViewModel
    {
        private static ServiceClient ServiceClient => ServiceClient.Instance.Value;

        #region Properties

        private string firstName;
        public string FirstName
        {
            get
            {
                return firstName;
            }

            set
            {
                firstName = value;
                NotifyPropertyChanged(nameof(FirstName));
            }
        }

        private string middleName;
        public string MiddleName
        {
            get
            {
                return middleName;
            }

            set
            {
                middleName = value;
                NotifyPropertyChanged(nameof(MiddleName));
            }
        }

        private string lastName;
        public string LastName
        {
            get
            {
                return lastName;
            }

            set
            {
                lastName = value;
                NotifyPropertyChanged(nameof(LastName));
            }
        }

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
            }
        }

        private PasswordBox confirmPasswordBox;
        public PasswordBox ConfirmPasswordBox
        {
            get
            {
                return confirmPasswordBox;
            }

            set
            {
                confirmPasswordBox = value;
                NotifyPropertyChanged(nameof(ConfirmPasswordBox));
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

    }
}
