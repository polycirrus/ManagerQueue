using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    using BSUIR.ManagerQueue.Client.Models;
    using BSUIR.ManagerQueue.Data.Model;
    using Commands;

    public class EnterQueueViewModel : BaseViewModel
    {
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

        private QueueItem selectedQueue;
        public QueueItem SelectedQueue
        {
            get
            {
                return selectedQueue;
            }

            set
            {
                selectedQueue = value;
                NotifyPropertyChanged(nameof(SelectedQueue));
                NotifyPropertyChanged(nameof(IsEnterQueueEnabled));
            }
        }

        public bool IsEnterQueueEnabled
        {
            get
            {
                return !IsBusy && SelectedQueue != null;
            }
        }

        public override bool IsBusy
        {
            get
            {
                return base.IsBusy;
            }

            set
            {
                base.IsBusy = value;
                NotifyPropertyChanged(nameof(IsEnterQueueEnabled));
            }
        }

        #endregion

        #region Commands

        public ICommand EnterQueueCommand => new AsyncDelegateCommand(EnterQueue);

        #endregion

        protected override async Task InitializeAsync()
        {
            Queues = await ServiceClient.GetQueueOwners();
            await base.InitializeAsync();
        }

        private async Task EnterQueue()
        {
            IsBusy = true;

            await ServiceClient.AddEntry(new Infrastructure.Models.AddQueueEntryModel()
            {
                EntrantId = ServiceClient.CurrentUser.Id,
                QueueId = SelectedQueue.Id
            });

            IsBusy = false;
        }
    }
}
