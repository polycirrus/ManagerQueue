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

        #endregion

        #region Commands

        #endregion

        protected override async Task InitializeAsync()
        {
            Queues = await ServiceClient.GetQueueOwners();
            await base.InitializeAsync();
        }
    }
}
