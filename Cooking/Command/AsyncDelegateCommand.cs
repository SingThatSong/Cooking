using System;
using System.Threading.Tasks;

namespace Cooking.WPF.Commands
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
            FreezeWhenBusy = freezeWhenBusy;

            CanExecuteSpecified = canExecute != null || FreezeWhenBusy || ExecuteOnce;
        }

        protected override bool CanExecuteAsyncInternal(object? parameter) => _canExecute != null ? _canExecute() : true;

        protected override async Task ExecuteAsyncInternal(object? parameter) => await _execute();
    }
}
