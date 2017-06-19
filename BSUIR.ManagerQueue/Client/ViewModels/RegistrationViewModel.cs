using BSUIR.ManagerQueue.Client.Commands;
using BSUIR.ManagerQueue.Client.Models;
using BSUIR.ManagerQueue.Client.Properties;
using BSUIR.ManagerQueue.Data.Model;
using BSUIR.ManagerQueue.Infrastructure;
using BSUIR.ManagerQueue.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    public class RegistrationViewModel : BaseViewModel
    {
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

        private IEnumerable<Position> positions;
        public IEnumerable<Position> Positions
        {
            get
            {
                return positions;
            }

            set
            {
                positions = value;
                NotifyPropertyChanged(nameof(Positions));
            }
        }

        private Position selectedPosition;
        public Position SelectedPosition
        {
            get
            {
                return selectedPosition;
            }

            set
            {
                selectedPosition = value;
                NotifyPropertyChanged(nameof(SelectedPosition));
            }
        }

        private string positionComboBoxText;
        public string PositionComboBoxText
        {
            get
            {
                return positionComboBoxText;
            }

            set
            {
                positionComboBoxText = value;
                NotifyPropertyChanged(nameof(PositionComboBoxText));
            }
        }

        private IEnumerable<Tuple<UserType, string>> userTypes;
        public IEnumerable<Tuple<UserType, string>> UserTypes
        {
            get
            {
                return userTypes;
            }

            set
            {
                userTypes = value;
                NotifyPropertyChanged(nameof(UserTypes));
            }
        }

        private Tuple<UserType, string> selectedUserType;
        public Tuple<UserType, string> SelectedUserType
        {
            get
            {
                return selectedUserType;
            }

            set
            {
                selectedUserType = value;
                NotifyPropertyChanged(nameof(SelectedUserType));
            }
        }

        #endregion

        #region Commands

        public ICommand RegisterCommand => new AsyncDelegateCommand(Register);

        public ICommand CancelCommand => new AsyncDelegateCommand(Cancel);

        #endregion

        public event EventHandler<RegistrationFinishedEventArgs> RegistrationFinished;

        public RegistrationViewModel()
        {
            userTypes = new[]
            {
                new Tuple<UserType, string>(UserType.Employee, Resources.EmployeeRoleName),
                new Tuple<UserType, string>(UserType.Secretary, Resources.SecretaryRoleName),
                new Tuple<UserType, string>(UserType.Vice, Resources.ViceManagerRoleName),
                new Tuple<UserType, string>(UserType.Manager, Resources.ManagerRoleName)
            };
            selectedUserType = userTypes.First();
        }

        protected override async Task InitializeAsync()
        {
            await LoadPositions();
            await base.InitializeAsync();
        }

        private async Task Register()
        {
            ErrorMessage = null;
            if (!ValidateForm())
                return;

            IsBusy = true;
            try
            {
                var position = SelectedPosition;
                if (position == null && !string.IsNullOrEmpty(PositionComboBoxText))
                {
                    position = await CreatePosition(PositionComboBoxText);
                    if (position == null)
                        return;
                }

                var model = new RegisterBindingModel()
                {
                    FirstName = FirstName,
                    Middlename = MiddleName,
                    LastName = LastName,
                    PositionId = position.Id,
                    UserType = selectedUserType.Item1,
                    UserName = UserName,
                    Password = PasswordBox.Password,
                    ConfirmPassword = ConfirmPasswordBox.Password
                };

                try
                {
                    await ServiceClient.Register(model);
                    RegistrationFinished?.Invoke(this, new RegistrationFinishedEventArgs(true));
                }
                catch (Exception exception)
                {
                    ErrorMessage = exception.Message;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Cancel()
        {
            RegistrationFinished?.Invoke(this, new RegistrationFinishedEventArgs(false));
        }

        private async Task LoadPositions()
        {
            IEnumerable<Position> positions;
            try
            {
                positions = await ServiceClient.GetPositions();
            }
            catch (Exception exception)
            {
                ErrorMessage = exception.Message;
                return;
            }

            Positions = positions;
        }

        private async Task<Position> CreatePosition(string positionComboBoxText)
        {
            var position = new Position() { JobTitle = positionComboBoxText };

            try
            {
                return await ServiceClient.CreatePosition(position);
            }
            catch (Exception exception)
            {
                ErrorMessage = exception.Message;
                return null;
            }
        }

        private bool ValidateForm()
        {
            var errorList = new List<string>();
            var validationResult = true;

            if (string.IsNullOrEmpty(FirstName))
            {
                errorList.Add(string.Format(Resources.MissingFieldErrorMessageTemplate, Resources.FirstNameFieldLabel));
                validationResult = false;
            }

            if (string.IsNullOrEmpty(LastName))
            {
                errorList.Add(string.Format(Resources.MissingFieldErrorMessageTemplate, Resources.LastNameFieldLabel));
                validationResult = false;
            }

            if (string.IsNullOrEmpty(LastName))
            {
                errorList.Add(string.Format(Resources.MissingFieldErrorMessageTemplate, Resources.PositionFieldLabel));
                validationResult = false;
            }

            if (string.IsNullOrEmpty(UserName))
            {
                errorList.Add(string.Format(Resources.MissingFieldErrorMessageTemplate, Resources.UserNameFieldLabel));
                validationResult = false;
            }

            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                errorList.Add(string.Format(Resources.MissingFieldErrorMessageTemplate, Resources.PasswordFieldLabel));
                validationResult = false;
            }

            if (!string.Equals(PasswordBox.Password, ConfirmPasswordBox.Password, StringComparison.Ordinal))
            {
                errorList.Add(Resources.PasswordsDoNotMatchErrorMessage);
                validationResult = false;
            }

            if (errorList.Any())
                ErrorMessage = string.Join(Environment.NewLine, errorList);

            return validationResult;
        }

        public class RegistrationFinishedEventArgs : EventArgs
        {
            public bool RegistrationSucceeded { get; set; }

            public RegistrationFinishedEventArgs()
            {
            }

            public RegistrationFinishedEventArgs(bool registrationSucceeded)
            {
                RegistrationSucceeded = registrationSucceeded;
            }
        }
    }
}
