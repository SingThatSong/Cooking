using System;
using System.Threading.Tasks;

namespace Cooking.WPF.Commands
{
    public class AsyncDelegateCommand<T> : AsyncDelegateCommandBase
    {
        private readonly Func<T, bool>? canExecute;
        private readonly Func<T, Task> execute;

        public AsyncDelegateCommand(Func<T, Task> execute,
                                    Func<T, bool>? canExecute = null,
                                    bool executeOnce = false,
                                    bool freezeWhenBusy = false)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            ExecuteOnce = executeOnce;
            FreezeWhenBusy = freezeWhenBusy;

            CanExecuteSpecified = canExecute != null || FreezeWhenBusy || ExecuteOnce;
        }

        protected override bool CanExecuteAsyncInternal(object? parameter)
        {
            if (canExecute != null)
            {
                if (parameter is T tParameter)
                {
                    return canExecute(tParameter);
                }
                else if (parameter == null && typeof(T).IsClass)
                {
#pragma warning disable CS8653 // Выражение по умолчанию вводит значение NULL для параметра типа.
                    return canExecute(default);
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
                await execute(tParameter);
            }
            else if (parameter == null && typeof(T).IsClass)
            {
#pragma warning disable CS8653 // Выражение по умолчанию вводит значение NULL для параметра типа.
                await execute(default);
#pragma warning restore CS8653
            }
            else
            {
                throw new InvalidOperationException($"Wrong {nameof(AsyncDelegateCommand)} parameter type");
            }
        }
    }
}
