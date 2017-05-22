using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSUIR.ManagerQueue.Client.Commands
{
    public class AsyncDelegateCommand : ICommand
    {
        private readonly Func<Task> action;

        public AsyncDelegateCommand(Func<Task> action)
        {
            this.action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public async void Execute(object parameter) => await action();
    }
}
