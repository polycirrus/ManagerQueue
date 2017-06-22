using System.Collections.Generic;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    using BSUIR.ManagerQueue.Client.Models;
    using BSUIR.ManagerQueue.Client.Properties;

    public class MainWindowViewModel : BaseViewModel
    {
        #region Properties

        public override string Title => Resources.ApplicationName;

        private IEnumerable<BaseViewModel> tabs;
        public IEnumerable<BaseViewModel> Tabs
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
            var tabs = new List<BaseViewModel>();

            var user = ServiceClient.CurrentUser;
            switch (user.Type)
            {
                case Infrastructure.UserType.Manager:
                case Infrastructure.UserType.Vice:
                    tabs.Add(new QueueViewModel() { QueueManager = user });
                    break;
                case Infrastructure.UserType.Secretary:
                    foreach (var managedQueue in ServiceClient.CurrentUser.ManagedQueues)
                        tabs.Add(new QueueViewModel() { QueueManager = managedQueue });
                    break;
            }

            tabs.Add(new QueueEntriesViewModel());
            
            tabs.Add(new AccountViewModel() { Account = user });

            if (user.IsAdministrator)
            {
                tabs.Add(new AccountsAdministrationViewModel());
                tabs.Add(new QueuesAdministrationViewModel());
            }

            this.tabs = tabs;
        }
    }
}
