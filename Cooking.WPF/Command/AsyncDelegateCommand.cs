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
        /// <param name="refreshAutomatically">A value indicating whether CanExecute should be refreshed automatically or manually.</param>
        /// <param name="forceExecutionOnSeparateThread">A value indicating whether Execute must be forced to run on a non-UI thread.</param>
        /// <param name="exceptionHandler">An exception handler function.</param>
        public AsyncDelegateCommand(Func<Task> execute,
                                    Func<bool>? canExecute = null,
                                    bool executeOnce = false,
                                    bool freezeWhenBusy = false,
                                    bool refreshAutomatically = true,
                                    bool forceExecutionOnSeparateThread = true,
                                    Func<Exception, bool>? exceptionHandler = null)
            : base(freezeWhenBusy, executeOnce, refreshAutomatically, forceExecutionOnSeparateThread, exceptionHandler)
        {
            this.execute = execute;
            this.canExecute = canExecute;

            CanExecuteSpecified = canExecute != null || FreezeWhenBusy || ExecuteOnce;
        }

        /// <inheritdoc/>
        protected override bool CanExecuteAsyncInternal(object? parameter) => canExecute == null || canExecute();

        /// <inheritdoc/>
        protected override async Task ExecuteAsyncInternal(object? parameter) => await execute();
    }
}
