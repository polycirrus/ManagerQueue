using BSUIR.ManagerQueue.Client.Models;
using BSUIR.ManagerQueue.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    public class QueuesAdministrationViewModel : BaseViewModel
    {
        private static ServiceClient ServiceClient => ServiceClient.Instance.Value;

        #region Properties

        private IEnumerable<Employee> queues;
        public IEnumerable<Employee> Queues
        {
            get
            {
                return queues;
            }

            set
            {
                queues = value;
                NotifyPropertyChanged(nameof(Queues));
            }
        }

        #endregion

        #region Commands

        #endregion

        public QueuesAdministrationViewModel()
        {
            Queues = DemoQueues;
        }

        private static Employee[] DemoQueues => new[]
        {
            new Employee()
            {
                FirstName = "John",
                LastName = "Doe",
                Position = new Position() { JobTitle = "Chief Executive Officer" },
                Type = Infrastructure.UserType.Manager,
                OwnQueueEntries = DemoQueueItems
            },
            new Employee()
            {
                FirstName = "Jack",
                LastName = "Smith",
                Position = new Position() { JobTitle = "Janitor" },
                Type = Infrastructure.UserType.Employee,
                OwnQueueEntries = DemoQueueItems
            },
            new Employee()
            {
                FirstName = "Jane",
                LastName = "Doe",
                Position = new Position() { JobTitle = "Chief Financial Officer" },
                Type = Infrastructure.UserType.Manager,
                OwnQueueEntries = DemoQueueItems
            }
        };

        private static QueueItem[] DemoQueueItems => new[]
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
}
