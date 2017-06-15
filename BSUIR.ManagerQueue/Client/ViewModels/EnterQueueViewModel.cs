using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    using BSUIR.ManagerQueue.Client.Models;
    using BSUIR.ManagerQueue.Data.Model;
    using Commands;
    using System.Windows.Input;

    public class EnterQueueViewModel : BaseViewModel
    {
        public static ServiceClient ServiceClient => ServiceClient.Instance.Value;

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
                return !isBusy && SelectedQueue != null;
            }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }

            set
            {
                isBusy = value;
                NotifyPropertyChanged(nameof(IsBusy));
                NotifyPropertyChanged(nameof(IsEnterQueueEnabled));
            }
        }

        #endregion

        #region Commands

        public ICommand EnterQueueCommand => new AsyncDelegateCommand(EnterQueue);

        #endregion

        public EnterQueueViewModel()
        {
            IsBusy = true;
            Task.Run(async () =>
            {
                Queues = await ServiceClient.GetQueueOwners();
                IsBusy = false;
            });
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
