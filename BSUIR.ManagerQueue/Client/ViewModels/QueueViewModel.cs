﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    using BSUIR.ManagerQueue.Client.Commands;
    using BSUIR.ManagerQueue.Client.Properties;
    using BSUIR.ManagerQueue.Client.Views;
    using BSUIR.ManagerQueue.Data.Model;

    public class QueueViewModel : BaseViewModel
    {
        #region Properties

        public override string Title
        {
            get
            {
                if (QueueManager == ServiceClient.CurrentUser)
                    return Resources.MyQueueTabName;

                if (QueueManager == null)
                    return string.Empty;

                return string.Format(Resources.ManagedQueueTabNameTemplate, QueueManager.Name);
            }
        }

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

                QueueItems = queueManager.OwnQueueEntries.Select(queueItem => CloneItem(queueItem));
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
                queueItems = value.OrderBy(x => x.Order).ToList();
                NotifyPropertyChanged(nameof(QueueItems));
                NotifyPropertyChanged(nameof(HasUnsavedOrderChanges));
                NotifyPropertyChanged(nameof(CanMoveUp));
                NotifyPropertyChanged(nameof(CanMoveDown));
                NotifyPropertyChanged(nameof(CanSaveOrResetOrder));
                NotifyPropertyChanged(nameof(CanRemoveItem));
            }
        }

        private QueueItem selectedItem;
        public QueueItem SelectedItem
        {
            get
            {
                return selectedItem;
            }

            set
            {
                selectedItem = value;
                NotifyPropertyChanged(nameof(SelectedItem));
                NotifyPropertyChanged(nameof(CanMoveUp));
                NotifyPropertyChanged(nameof(CanMoveDown));
                NotifyPropertyChanged(nameof(CanSaveOrResetOrder));
                NotifyPropertyChanged(nameof(CanRemoveItem));
            }
        }

        public bool HasUnsavedOrderChanges => !queueItems.Select(x => x.EmployeeId)
            .SequenceEqual(queueManager.OwnQueueEntries.OrderBy(x => x.Order).Select(x => x.EmployeeId));

        public bool CanMoveUp => !IsBusy && SelectedItem != null && SelectedItem.Order > 0;

        public bool CanMoveDown => !IsBusy && SelectedItem != null && SelectedItem.Order < QueueItems.Count() - 1;

        public bool CanSaveOrResetOrder => !IsBusy && QueueItems != null && QueueItems.Any() && HasUnsavedOrderChanges;

        public bool CanRemoveItem => !IsBusy && SelectedItem != null;

        #endregion

        #region Commands

        public ICommand MoveUpCommand => new AsyncDelegateCommand(MoveUp);

        public ICommand MoveDownCommand => new AsyncDelegateCommand(MoveDown);

        public ICommand SaveOrderCommand => new AsyncDelegateCommand(SaveOrder);

        public ICommand ResetOrderCommand => new AsyncDelegateCommand(ResetOrder);

        public ICommand RemoveItemCommand => new AsyncDelegateCommand(RemoveItem);

        public ICommand ManageSecretariesCommand => new AsyncDelegateCommand(ManageSecretaries);

        #endregion

        private async Task MoveUp()
        {
            var newOrder = SelectedItem.Order - 1;
            QueueItems.First(item => item.Order == newOrder).Order += 1;
            SelectedItem.Order = newOrder;
            QueueItems = QueueItems.OrderBy(item => item.Order).ToList();
        }

        private async Task MoveDown()
        {
            var newOrder = SelectedItem.Order + 1;
            QueueItems.First(item => item.Order == newOrder).Order -= 1;
            SelectedItem.Order = newOrder;
            QueueItems = QueueItems.OrderBy(item => item.Order).ToList();
        }

        private async Task SaveOrder()
        {
            IsBusy = true;

            QueueItems = await ServiceClient.SaveQueue(queueManager.Id, QueueItems);
            QueueManager.OwnQueueEntries = (QueueItems as ICollection<QueueItem>) ?? QueueItems.ToList();

            if (SelectedItem != null)
                SelectedItem = QueueItems.First(item => item.Id == SelectedItem.Id);

            IsBusy = false;
        }

        private async Task ResetOrder()
        {
            QueueItems = QueueManager.OwnQueueEntries;
        }

        private async Task RemoveItem()
        {
            IsBusy = true;

            var removedItem = SelectedItem;
            SelectedItem = null;

            await ServiceClient.DeleteEntry(removedItem.Id);
            QueueItems = await ServiceClient.GetQueue(QueueManager.Id);
            QueueManager.OwnQueueEntries = (QueueItems as ICollection<QueueItem>) ?? QueueItems.ToList();

            IsBusy = false;
        }

        private async Task ManageSecretaries()
        {
            var manageSecretariesWindow = new ManageSecretariesWindow();
            manageSecretariesWindow.ShowDialog();
        }

        private QueueItem CloneItem(QueueItem queueItem)
        {
            return new QueueItem()
            {
                Id = queueItem.Id,
                Employee = queueItem.Employee,
                EmployeeId = queueItem.EmployeeId,
                Manager = queueItem.Manager,
                ManagerId = queueItem.ManagerId,
                Order = queueItem.Order
            };
        }
    }
}
