using BSUIR.ManagerQueue.Client.Models;
using BSUIR.ManagerQueue.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    public class AccountsAdministrationViewModel : BaseViewModel
    {
        #region Properties

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

        #endregion

        #region Commands

        #endregion

        protected override async Task InitializeAsync()
        {
            Accounts = await ServiceClient.GetAllAccounts();
            await base.InitializeAsync();
        }
    }
}
