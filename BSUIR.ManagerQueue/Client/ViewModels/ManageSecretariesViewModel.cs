using BSUIR.ManagerQueue.Client.Models;
using BSUIR.ManagerQueue.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    public class ManageSecretariesViewModel : BaseViewModel
    {
        private static ServiceClient ServiceClient => ServiceClient.Instance.Value;

        #region Properties

        private IEnumerable<Employee> currentSecretaries;
        public IEnumerable<Employee> CurrentSecretaries
        {
            get
            {
                return currentSecretaries;
            }

            set
            {
                currentSecretaries = value;
                NotifyPropertyChanged(nameof(CurrentSecretaries));
            }
        }

        private IEnumerable<Employee> availableSecretaries;
        public IEnumerable<Employee> AvailableSecretaries
        {
            get
            {
                return availableSecretaries;
            }

            set
            {
                availableSecretaries = value;
                NotifyPropertyChanged(nameof(AvailableSecretaries));
            }
        }

        #endregion

        #region Commands

        #endregion

        public ManageSecretariesViewModel()
        {
            currentSecretaries = new[]
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

            availableSecretaries = new[]
            {
                new Employee()
                {
                    FirstName = "James",
                    LastName = "Jackson",
                    Position = new Position() { JobTitle = "Secretary" },
                    Type = Infrastructure.UserType.Secretary
                },
                new Employee()
                {
                    FirstName = "Mary",
                    LastName = "Berry",
                    Position = new Position() { JobTitle = "Secretary" },
                    Type = Infrastructure.UserType.Secretary
                },
                new Employee()
                {
                    FirstName = "Jim",
                    LastName = "Clark",
                    Position = new Position() { JobTitle = "Secretary" },
                    Type = Infrastructure.UserType.Secretary
                }
            };
        }
    }
}
