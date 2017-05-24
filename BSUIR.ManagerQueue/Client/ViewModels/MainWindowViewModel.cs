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
            tabs = new[]
            {
                new TabItem() { Header = Resources.MyQueueTabName, Content = new QueueView() },
                new TabItem() { Header = Resources.QueueEntriesTabName, Content = new QueueEntriesView() },
                new TabItem() { Header = Resources.AccountTabName, Content = new AccountView() }
            };
        }
    }
}
