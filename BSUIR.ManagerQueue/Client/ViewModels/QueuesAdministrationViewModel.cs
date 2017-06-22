using BSUIR.ManagerQueue.Client.Models;
using BSUIR.ManagerQueue.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    public class QueuesAdministrationViewModel : BaseViewModel
    {
        #region Properties

        public override string Title => Properties.Resources.QueuesAdministrationTabName;

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

        private QueueViewModel queueViewModel;
        public QueueViewModel QueueViewModel
        {
            get => queueViewModel;

            set
            {
                queueViewModel = value;
                NotifyPropertyChanged(nameof(QueueViewModel));
            }
        }

        #endregion

        #region Commands

        #endregion

        public QueuesAdministrationViewModel()
        {
            QueueViewModel = new QueueViewModel();
        }

        protected override async Task InitializeAsync()
        {
            Queues = await ServiceClient.GetQueueOwners();
            await base.InitializeAsync();
        }
    }
}
