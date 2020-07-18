using System;
using System.Threading.Tasks;

namespace Cooking.WPF.Commands
{
    /// <summary>
    /// ICommand implementation which force execution on separate thread.
    /// </summary>
    public class AsyncDelegateCommand : AsyncDelegateCommandBase
    {
        private readonly Func<bool>? canExecute;
        private readonly Func<Task> execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDelegateCommand"/> class.
        /// </summary>
        /// <param name="execute">Function to execute on separate thread.</param>
        /// <param name="canExecute">Function to determine if <see cref="execute"/> can be executed.</param>
        /// <param name="executeOnce">Execute function only once, after that <see cref="canExecute"/> would return false regardless.</param>
        /// <param name="freezeWhenBusy">UI not blocked when function executed, so user can trigger function multiple times at once. This will prevent it: during execution <see cref="canExecute"/> would return false.</param>
        public AsyncDelegateCommand(Func<Task> execute,
                                    Func<bool>? canExecute = null,
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
        protected override bool CanExecuteAsyncInternal(object? parameter) => canExecute == null || canExecute();

        /// <inheritdoc/>
        protected override async Task ExecuteAsyncInternal(object? parameter) => await execute();
    }
}
