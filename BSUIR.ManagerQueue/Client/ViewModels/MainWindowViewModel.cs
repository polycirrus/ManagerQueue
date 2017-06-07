using BSUIR.ManagerQueue.Client.Models;
using BSUIR.ManagerQueue.Client.Properties;
using BSUIR.ManagerQueue.Client.Views;
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
                    tabs.Add(new TabItem() { Header = Resources.MyQueueTabName, Content = new QueueView() });
                    break;
                case Infrastructure.UserType.Secretary:
                    AddSecretaryManagedQueueTabs(tabs);
                    break;
            }

            tabs.Add(new TabItem() { Header = Resources.QueueEntriesTabName, Content = new QueueEntriesView() });
            tabs.Add(new TabItem() { Header = Resources.AccountTabName, Content = new AccountView() });

            if (user.IsAdministrator)
                tabs.Add(new TabItem() { Header = Resources.AdministrationTabName, Content = new AdministrationView() });
        }

        private void AddSecretaryManagedQueueTabs(List<TabItem> tabs)
        {
            foreach (var managedQueue in ServiceClient.CurrentUser.ManagedQueues)
                tabs.Add(new TabItem() { Header = string.Format(Resources.ManagedQueueTabNameTemplate, managedQueue.Name), Content = new QueueView() });
        }
    }
}
