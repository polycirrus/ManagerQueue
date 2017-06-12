using System.Collections.Generic;

using System.Threading.Tasks;
using System.Windows.Input;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    using BSUIR.ManagerQueue.Client.Commands;
    using BSUIR.ManagerQueue.Client.Models;
    using BSUIR.ManagerQueue.Client.Views;
    using BSUIR.ManagerQueue.Data.Model;

    public class QueueEntriesViewModel : BaseViewModel
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

        public ICommand EnterQueueCommand => new AsyncDelegateCommand(EnterQueue);

        #endregion

        public QueueEntriesViewModel()
        {
            queueItems = new[]
            {
                new QueueItem()
                {
                    Id = 0,
                    Order = 0,
                    Manager = new Employee()
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
                    Manager = new Employee()
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
                    Manager = new Employee()
                    {
                        FirstName = "Jane",
                        LastName = "Doe",
                        Position = new Position() { JobTitle = "Chief Financial Officer" },
                        Type = Infrastructure.UserType.Manager
                    }
                }
            };
        }

        private async Task EnterQueue()
        {
            var enterQueueWindow = new EnterQueueWindow();
            enterQueueWindow.ShowDialog();
        }
    }
}
