using System;
using System.Threading.Tasks;

namespace Cooking.WPF.Commands
{
    /// <summary>
    /// ICommand implementation which force execution on separate thread. Takes arbitrary paremeter as input.
    /// </summary>
    /// <typeparam name="T">Command parameter type.</typeparam>
    public class AsyncDelegateCommand<T> : AsyncDelegateCommandBase
    {
        private readonly Func<T, bool>? canExecute;
        private readonly Func<T, Task> execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDelegateCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">Function to execute on separate thread.</param>
        /// <param name="canExecute">Function to determine if <see cref="execute"/> can be executed.</param>
        /// <param name="executeOnce">Execute function only once, after that <see cref="canExecute"/> would return false regardless.</param>
        /// <param name="freezeWhenBusy">UI not blocked when function executed, so user can trigger function multiple times at once. This will prevent it: during execution <see cref="canExecute"/> would return false.</param>
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

        /// <inheritdoc/>
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
                    return canExecute(default);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsyncInternal(object? parameter)
        {
            if (parameter is T tParameter)
            {
                await execute(tParameter);
            }
            else if (parameter == null && typeof(T).IsClass)
            {
                await execute(default);
            }
            else
            {
                throw new InvalidOperationException($"Wrong {nameof(AsyncDelegateCommand)} parameter type");
            }
        }
    }
}
