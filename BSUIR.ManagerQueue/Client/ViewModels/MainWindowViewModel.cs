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
    }
}
