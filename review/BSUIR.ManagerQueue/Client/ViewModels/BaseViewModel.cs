using System.ComponentModel;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    using BSUIR.ManagerQueue.Client.Models;

    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected static ServiceClient ServiceClient => ServiceClient.Instance.Value;

        private bool isBusy;
        public virtual bool IsBusy
        {
            get
            {
                return isBusy;
            }

            set
            {
                isBusy = value;
                NotifyPropertyChanged(nameof(IsBusy));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public BaseViewModel()
        {
            IsBusy = true;
            Task.Run(async () =>
            {
                await InitializeAsync();
                IsBusy = false;
            });
        }

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual async Task InitializeAsync()
        {
        }
    }
}
