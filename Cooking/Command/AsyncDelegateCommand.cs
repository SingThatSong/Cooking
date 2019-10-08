using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cooking.Commands
{
    public class AsyncDelegateCommand : AsyncDelegateCommandBase
    {
        private readonly Func<bool>? _canExecute;
        private readonly Func<Task> _execute;

        public AsyncDelegateCommand(Func<Task> execute,
                                    Func<bool>? canExecute = null,
                                    bool executeOnce = false,
                                    bool freezeWhenBusy = false)
        {
            _execute = execute;
            _canExecute = canExecute;
            ExecuteOnce = executeOnce;
            CanExecuteSpecified = canExecute != null;
            FreezeWhenBusy = freezeWhenBusy;
        }

        protected override bool CanExecuteAsyncInternal(object? parameter) => _canExecute != null ? _canExecute() : true;

        protected override Task ExecuteAsyncInternal(object? parameter) => _execute();
    }
}
