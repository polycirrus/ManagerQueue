using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    using BSUIR.ManagerQueue.Client.Commands;
    using BSUIR.ManagerQueue.Client.Models;
    using BSUIR.ManagerQueue.Data.Model;
    using BSUIR.ManagerQueue.Infrastructure;

    public class AccountViewModel : BaseViewModel
    {
        #region Properties

        public override string Title => Properties.Resources.AccountTabName;

        private Employee account;
        public Employee Account
        {
            get
            {
                return account;
            }

            set
            {
                account = value;
                NotifyPropertyChanged(nameof(Account));
                InitializeFieldsFromAccount();
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
                NotifyPropertyChanged(nameof(CanSaveAndRevertChanges));
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

        private IEnumerable<UserType> userTypes;
        public IEnumerable<UserType> UserTypes
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

        private UserType selectedUserType;
        public UserType SelectedUserType
        {
            get
            {
                return selectedUserType;
            }

            set
            {
                selectedUserType = value;
                NotifyPropertyChanged(nameof(SelectedUserType));
                NotifyPropertyChanged(nameof(CanSaveAndRevertChanges));
            }
        }

        private string firstName;
        public string FirstName
        {
            get => firstName;

            set
            {
                firstName = value;
                NotifyPropertyChanged(nameof(FirstName));
                NotifyPropertyChanged(nameof(CanSaveAndRevertChanges));
            }
        }
        private string middleName;
        public string MiddleName
        {
            get => middleName;

            set
            {
                middleName = value;
                NotifyPropertyChanged(nameof(MiddleName));
                NotifyPropertyChanged(nameof(CanSaveAndRevertChanges));
            }
        }
        private string lastName;
        public string LastName
        {
            get => lastName;

            set
            {
                lastName = value;
                NotifyPropertyChanged(nameof(LastName));
                NotifyPropertyChanged(nameof(CanSaveAndRevertChanges));
            }
        }

        public override bool IsBusy
        {
            get => base.IsBusy;

            set
            {
                base.IsBusy = value;
                NotifyPropertyChanged(nameof(CanEditAdministratorData));
            }
        }

        public bool CanEditAdministratorData
        {
            get
            {
                return !IsBusy && ServiceClient.CurrentUser.IsAdministrator;
            }
        }

        public bool CanSaveAndRevertChanges
        {
            get
            {
                return Account != null && !IsBusy &&
                    (FirstName != Account.FirstName
                    || MiddleName != Account.Middlename
                    || LastName != Account.LastName
                    || SelectedPosition?.Id != Account.PositionId
                    || SelectedUserType != Account.Type);
            }
        }

        #endregion

        #region Commands

        public ICommand SaveChangesCommand => new AsyncDelegateCommand(SaveChanges);

        public ICommand RevertChangesCommand => new AsyncDelegateCommand(RevertChanges);

        public ICommand ChangePasswordCommand => new AsyncDelegateCommand(ChangePassword);

        #endregion

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            if (!ServiceClient.CurrentUser.IsAdministrator)
            {
                SelectedPosition = Positions.FirstOrDefault();
                return;
            }

            Positions = await ServiceClient.GetPositions();
            SelectedPosition = Positions.FirstOrDefault(position => position.Id == Account?.Position?.Id);
        }

        private void InitializeFieldsFromAccount()
        {
            FirstName = Account.FirstName;
            MiddleName = Account.Middlename;
            LastName = Account.LastName;

            UserTypes = new[] { UserType.Employee, UserType.Manager, UserType.Secretary, UserType.Vice };
            SelectedUserType = Account.Type;

            Positions = new[] { Account.Position };
            SelectedPosition = Positions.First();
        }

        private async Task SaveChanges()
        {
            IsBusy = true;

            var shouldUpdateCurrentUser = Account == ServiceClient.CurrentUser;
            Account = await ServiceClient.SaveAccount(Account);
            if (shouldUpdateCurrentUser)
                ServiceClient.CurrentUser = Account;

            IsBusy = false;
        }

        private async Task RevertChanges()
        {
            FirstName = Account.FirstName;
            MiddleName = Account.Middlename;
            LastName = Account.LastName;
            SelectedUserType = Account.Type;
            SelectedPosition = Positions.First(position => position.Id == Account.Position.Id);
        }

        private Task ChangePassword()
        {
            throw new NotImplementedException();
        }
    }
}
