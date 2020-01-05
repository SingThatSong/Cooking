using System;
using System.Diagnostics;
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

        protected override bool CanExecuteAsyncInternal(object? parameter)
        {
            if (_canExecute != null)
            {
                if (parameter is T tParameter)
                {
                    return _canExecute(tParameter);
                }
                else if (parameter == null && typeof(T).IsClass)
                {
#pragma warning disable CS8653 // Выражение по умолчанию вводит значение NULL для параметра типа.
                    return _canExecute(default);
#pragma warning restore CS8653
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
            
        protected override async Task ExecuteAsyncInternal(object? parameter)
        {
            if (parameter is T tParameter)
            {
                await _execute(tParameter).ConfigureAwait(false);
            }
            else if (parameter == null && typeof(T).IsClass)
            {
#pragma warning disable CS8653 // Выражение по умолчанию вводит значение NULL для параметра типа.
                await _execute(default).ConfigureAwait(false);
#pragma warning restore CS8653
            }
            else
            {
                throw new InvalidOperationException($"Wrong {nameof(AsyncDelegateCommand)} parameter type");
            }
        }
    }
}
