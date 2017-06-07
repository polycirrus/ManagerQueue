using BSUIR.ManagerQueue.Client.Commands;
using BSUIR.ManagerQueue.Client.Models;
using BSUIR.ManagerQueue.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    public class AccountsAdministrationViewModel : BaseViewModel
    {
        private static ServiceClient ServiceClient => ServiceClient.Instance.Value;

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

        public AccountsAdministrationViewModel()
        {
            accounts = new[]
            {
                new Employee()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Position = new Position() { JobTitle = "Chief Executive Officer" },
                    Type = Infrastructure.UserType.Manager
                },
                new Employee()
                {
                    FirstName = "Jack",
                    LastName = "Smith",
                    Position = new Position() { JobTitle = "Janitor" },
                    Type = Infrastructure.UserType.Employee
                },
                new Employee()
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Position = new Position() { JobTitle = "Chief Financial Officer" },
                    Type = Infrastructure.UserType.Manager
                }
            };
        }
    }
}
