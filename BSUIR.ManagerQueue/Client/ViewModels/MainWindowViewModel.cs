using BSUIR.ManagerQueue.Client.Models;
using BSUIR.ManagerQueue.Client.Properties;
using BSUIR.ManagerQueue.Client.Views;
using BSUIR.ManagerQueue.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private static ServiceClient ServiceClient => ServiceClient.Instance.Value;

        #region Properties

        private IEnumerable<TabItem> tabs;
        public IEnumerable<TabItem> Tabs
        {
            get
            {
                return tabs;
            }

            set
            {
                tabs = value;
                NotifyPropertyChanged(nameof(Tabs));
            }
        }

        #endregion

        #region Commands

        #endregion

        public MainWindowViewModel()
        {
            var tabs = new List<TabItem>();

            var user = ServiceClient.CurrentUser;
            switch (user.Type)
            {
                case Infrastructure.UserType.Manager:
                case Infrastructure.UserType.Vice:
                    AddDemoQueueItems(user);
                    var queueView = new QueueView();
                    ((QueueViewModel)queueView.DataContext).QueueManager = user;
                    tabs.Add(new TabItem() { Header = Resources.MyQueueTabName, Content = queueView });
                    break;
                case Infrastructure.UserType.Secretary:
                    AddSecretaryManagedQueueTabs(tabs);
                    break;
            }

            tabs.Add(new TabItem() { Header = Resources.QueueEntriesTabName, Content = new QueueEntriesView() });

            var accountView = new AccountView();
            ((AccountViewModel)accountView.DataContext).Account = ServiceClient.CurrentUser;
            tabs.Add(new TabItem() { Header = Resources.AccountTabName, Content = accountView });

            if (user.IsAdministrator)
            {
                tabs.Add(new TabItem() { Header = Resources.AccountsAdministrationTabName, Content = new AccountsAdministrationView() });
                tabs.Add(new TabItem() { Header = Resources.QueuesAdministrationTabName, Content = new QueuesAdministrationView() });
            }

            this.tabs = tabs;
        }

        private void AddSecretaryManagedQueueTabs(List<TabItem> tabs)
        {
            foreach (var managedQueue in ServiceClient.CurrentUser.ManagedQueues)
                tabs.Add(new TabItem() { Header = string.Format(Resources.ManagedQueueTabNameTemplate, managedQueue.Name), Content = new QueueView() });
        }

        private void AddDemoQueueItems(Employee user)
        {
            user.OwnQueueEntries = new[]
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
}
