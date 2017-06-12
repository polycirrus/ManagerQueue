using BSUIR.ManagerQueue.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    public class EnterQueueViewModel : BaseViewModel
    {
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

        public EnterQueueViewModel()
        {
            queues = new[]
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
