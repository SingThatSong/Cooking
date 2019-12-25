using System;
using System.Threading.Tasks;

namespace Cooking.Commands
{
    public class AsyncDelegateCommand<T> : AsyncDelegateCommandBase
    {
        private readonly Func<T, bool>? _canExecute;
        private readonly Func<T, Task> _execute;
               
        public AsyncDelegateCommand(Func<T, Task> execute,
                                    Func<T, bool>? canExecute = null,
                                    bool executeOnce = false, 
                                    bool freezeWhenBusy = false)
        {
            _execute = execute;
            _canExecute = canExecute;
            ExecuteOnce = executeOnce;
            FreezeWhenBusy = freezeWhenBusy;

            CanExecuteSpecified = canExecute != null || FreezeWhenBusy || ExecuteOnce;
        }

        protected override bool CanExecuteAsyncInternal(object? parameter) => _canExecute != null && parameter is T tParameter ? _canExecute(tParameter) : true;

        protected override async Task ExecuteAsyncInternal(object? parameter)
        {
            if (parameter is T tParameter)
            {
                await _execute(tParameter).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidCastException("Command parameter is not T");
            }
        }
    }
}
