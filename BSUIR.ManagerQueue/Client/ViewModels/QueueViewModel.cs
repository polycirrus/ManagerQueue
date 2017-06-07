using BSUIR.ManagerQueue.Client.Commands;
using BSUIR.ManagerQueue.Client.Models;
using BSUIR.ManagerQueue.Client.Views;
using BSUIR.ManagerQueue.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    public class QueueViewModel : BaseViewModel
    {
        private static ServiceClient ServiceClient => ServiceClient.Instance.Value;

        #region Properties

        private IEnumerable<QueueItem> queueItems;
        public IEnumerable<QueueItem> QueueItems
        {
            get
            {
                return queueItems;
            }

            set
            {
                queueItems = value;
                NotifyPropertyChanged(nameof(QueueItems));
            }
        }

        #endregion

        #region Commands

        public ICommand ManageSecretariesCommand => new AsyncDelegateCommand(ManageSecretaries);

        #endregion

        public QueueViewModel()
        {
            queueItems = new[]
            {
                new QueueItem()
                {
                    Id = 0,
                    Order = 0,
                    Employee = new Employee()
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        Position = new Position() { JobTitle = "Chief Executive Officer" },
                        Type = Infrastructure.UserType.Manager
                    }
                },
                new QueueItem()
                {
                    Id = 2,
                    Order = 1,
                    Employee = new Employee()
                    {
                        FirstName = "Jack",
                        LastName = "Smith",
                        Position = new Position() { JobTitle = "Janitor" },
                        Type = Infrastructure.UserType.Employee
                    }
                },
                new QueueItem()
                {
                    Id = 1,
                    Order = 2,
                    Employee = new Employee()
                    {
                        FirstName = "Jane",
                        LastName = "Doe",
                        Position = new Position() { JobTitle = "Chief Financial Officer" },
                        Type = Infrastructure.UserType.Manager
                    }
                }
            };
        }

        private async Task ManageSecretaries()
        {
            var manageSecretariesWindow = new ManageSecretariesWindow();
            manageSecretariesWindow.ShowDialog();
        }
    }
}
