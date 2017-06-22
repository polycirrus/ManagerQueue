using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    using BSUIR.ManagerQueue.Client.Models;
    using BSUIR.ManagerQueue.Data.Model;

    public class AccountsAdministrationViewModel : BaseViewModel
    {
        #region Properties

        public override string Title => Properties.Resources.AccountsAdministrationTabName;

        private IEnumerable<Employee> accounts;
        public IEnumerable<Employee> Accounts
        {
            get
            {
                return accounts;
            }

            set
            {
                accounts = value;
                NotifyPropertyChanged(nameof(Accounts));
            }
        }

        private AccountViewModel accountViewModel;
        public AccountViewModel AccountViewModel
        {
            get => accountViewModel;

            set
            {
                accountViewModel = value;
                NotifyPropertyChanged(nameof(AccountViewModel));
            }
        }

        #endregion

        #region Commands

        #endregion

        public AccountsAdministrationViewModel()
        {
            AccountViewModel = new AccountViewModel();
        }

        protected override async Task InitializeAsync()
        {
            Accounts = await ServiceClient.GetAllAccounts();
            await base.InitializeAsync();
        }
    }
}
