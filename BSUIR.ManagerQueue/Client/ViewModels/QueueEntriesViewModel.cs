using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    using System;
    using BSUIR.ManagerQueue.Client.Commands;
    using BSUIR.ManagerQueue.Client.Models;
    using BSUIR.ManagerQueue.Client.Views;
    using BSUIR.ManagerQueue.Data.Model;

    public class QueueEntriesViewModel : BaseViewModel
    {
        #region Properties

        public override string Title => Properties.Resources.QueueEntriesTabName;

        public IEnumerable<QueueItem> QueueItems => ServiceClient.CurrentUser.ForeignQueueEntries;

        private QueueItem selectedQueueItem;
        public QueueItem SelectedQueueItem
        {
            get
            {
                return selectedQueueItem;
            }

            set
            {
                selectedQueueItem = value;
                NotifyPropertyChanged(nameof(SelectedQueueItem));
                NotifyPropertyChanged(nameof(IsExitQueueEnabled));
            }
        }

        public bool IsExitQueueEnabled
        {
            get
            {
                return !IsBusy && SelectedQueueItem != null;
            }
        }

        public override bool IsBusy
        {
            get => base.IsBusy;

            set
            {
                base.IsBusy = value;
                NotifyPropertyChanged(nameof(IsExitQueueEnabled));
            }
        }

        #endregion

        #region Commands

        public ICommand EnterQueueCommand => new AsyncDelegateCommand(EnterQueue);

        public ICommand ExitQueueCommand => new AsyncDelegateCommand(ExitQueue);

        #endregion

        public QueueEntriesViewModel()
        {
        }

        private async Task EnterQueue()
        {
            var enterQueueWindow = new EnterQueueWindow();
            enterQueueWindow.ShowDialog();
        }

        private async Task ExitQueue()
        {
            IsBusy = true;
            await ServiceClient.DeleteEntry(SelectedQueueItem.Id);
            await ServiceClient.UpdateCurrentUser();
            NotifyPropertyChanged(nameof(QueueItems));
            IsBusy = false;
        }
    }
}
