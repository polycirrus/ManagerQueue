using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace BSUIR.ManagerQueue.Client.ViewModels
{
    using BSUIR.ManagerQueue.Client.Commands;
    using BSUIR.ManagerQueue.Client.Models;

    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected static ServiceClient ServiceClient => ServiceClient.Instance.Value;

        public abstract string Title { get; }

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

        public ICommand InitializeCommand => new AsyncDelegateCommand(OnLoaded);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task OnLoaded()
        {
            IsBusy = true;
            await Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
            {
                await InitializeAsync();
                IsBusy = false;
            }, DispatcherPriority.ContextIdle);
        }

        protected virtual async Task InitializeAsync()
        {
        }
    }
}
