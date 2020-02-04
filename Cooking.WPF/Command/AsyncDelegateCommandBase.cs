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
        /// Gets or sets a value indicating whether command would return false when called CanExecute.
        /// </summary>
        protected bool FreezeWhenBusy { get; set; }

        private bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                if (FreezeWhenBusy)
                {
                    CommandManager.InvalidateRequerySuggested();
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
            await ExecuteAsyncInternal(parameter);
            IsBusy = false;
        }

        /// <summary>
        /// Async means internal implementation for Async* commands.
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. May be ignored.</param>
        /// <returns>Awaitable task.</returns>
        protected abstract Task ExecuteAsyncInternal(object? parameter);
    }
}