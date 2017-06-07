using BSUIR.ManagerQueue.Client.Models;
using BSUIR.ManagerQueue.Data.Model;
using BSUIR.ManagerQueue.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        private static ServiceClient ServiceClient => ServiceClient.Instance.Value;

        #region Properties

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
                NotifyPropertyChanged(nameof(CanEditAdministratorData));
            }
        }

        private bool canEditAdministratorData;
        public bool CanEditAdministratorData
        {
            get
            {
                return !isBusy && ServiceClient.CurrentUser.IsAdministrator;
            }
        }

        #endregion

        #region Commands

        #endregion

        public AccountViewModel()
        {
            account = ServiceClient.CurrentUser;
            if (account == null)
                return;

            positions = new[] { account.Position };
            Task.Run(() => 
            {
                System.Threading.Thread.Sleep(100);
                SelectedPosition = positions.First();
            });

            userTypes = new[] { UserType.Employee, UserType.Manager, UserType.Secretary, UserType.Vice };
            selectedUserType = account.Type;
        }
    }
}
