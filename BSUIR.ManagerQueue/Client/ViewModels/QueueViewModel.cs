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

        private Employee queueManager;
        public Employee QueueManager
        {
            get
            {
                return queueManager;
            }

            set
            {
                queueManager = value;
                NotifyPropertyChanged(nameof(QueueManager));

                QueueItems = queueManager.OwnQueueEntries;
            }
        }

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
        
        private async Task ManageSecretaries()
        {
            var manageSecretariesWindow = new ManageSecretariesWindow();
            manageSecretariesWindow.ShowDialog();
        }
    }
}
