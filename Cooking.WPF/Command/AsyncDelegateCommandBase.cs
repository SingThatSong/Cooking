using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cooking.WPF.Commands
{
    /// <summary>
    /// ICommand implementation for WPF bindings.
    /// </summary>
    public abstract class AsyncDelegateCommandBase : DelegateCommandBase
    {
        private bool isBusy;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDelegateCommandBase"/> class.
        /// </summary>
        /// <param name="freezeWhenBusy">UI not blocked when function executed, so user can trigger function multiple times at once. This will prevent it: during execution CanExecute would return false.</param>
        /// <param name="executeOnce">Execute function only once, after that CanExecute would return false regardless.</param>
        /// <param name="refreshAutomatically">A value indicating whether CanExecute should be refreshed automatically or manually.</param>
        /// <param name="exceptionHandler">An exception handler function.</param>
        protected AsyncDelegateCommandBase(bool freezeWhenBusy,
                                           bool executeOnce,
                                           bool refreshAutomatically,
                                           Func<Exception, bool>? exceptionHandler = null)
            : base(executeOnce, refreshAutomatically, exceptionHandler)
        {
            FreezeWhenBusy = freezeWhenBusy;
        }

        /// <summary>
        /// Gets a value indicating whether command would return false when called CanExecute.
        /// </summary>
        protected bool FreezeWhenBusy { get; }

        private bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                if (FreezeWhenBusy)
                {
                    RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Implementation of <see cref="DelegateCommandBase" /> CanExecuteInternal.
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. May be ignored.</param>
        /// <returns>If this command can be executed.</returns>
        protected sealed override bool CanExecuteInternal(object? parameter)
        {
            if (FreezeWhenBusy && IsBusy)
            {
                return false;
            }

            return CanExecuteAsyncInternal(parameter);
        }

        /// <summary>
        /// Async means internal implementation for Async* commands.
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. May be ignored.</param>
        /// <returns>Returns value which indicates whether command can be executed.</returns>
        protected abstract bool CanExecuteAsyncInternal(object? parameter);

        /// <summary>
        /// Implementation of <see cref="DelegateCommandBase" /> ExecuteInternal.
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. May be ignored.</param>
        protected sealed override async void ExecuteInternal(object? parameter)
        {
            IsBusy = true;

            // Force execution on non-UI thread
            await Task.Delay(1);
            try
            {
                await ExecuteAsyncInternal(parameter);
            }
            catch (Exception exception)
            {
                bool isHandeled = HandleException(exception);
                if (!isHandeled)
                {
                    throw;
                }
            }
            finally
            {
                IsBusy = false;
            }

            if (!FreezeWhenBusy)
            {
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Async means internal implementation for Async* commands.
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. May be ignored.</param>
        /// <returns>Awaitable task.</returns>
        protected abstract Task ExecuteAsyncInternal(object? parameter);
    }
}