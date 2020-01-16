using System.Threading.Tasks;
using System.Windows.Input;

namespace Cooking.WPF.Commands
{
    /// <summary>
    /// ICommand implementation for WPF bindings
    /// </summary>
    public abstract class AsyncDelegateCommandBase : DelegateCommandBase
    {
        protected bool FreezeWhenBusy { get; set; }
        private bool _isBusy;
        private bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                if (FreezeWhenBusy)
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Provide implementation of CanExecute, keep it to buisness logic
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. May be ignored</param>
        /// <returns>If this command can be executed</returns>
        protected override sealed bool CanExecuteInternal(object? parameter)
        {
            if (FreezeWhenBusy && IsBusy)
            {
                return false;
            }

            return CanExecuteAsyncInternal(parameter);
        }

        /// <summary>
        /// Async means internal implementation for Async* commands
        /// </summary>
        protected abstract bool CanExecuteAsyncInternal(object? parameter);

        protected override sealed async void ExecuteInternal(object? parameter)
        {
            IsBusy = true;
            await Task.Delay(1).ConfigureAwait(false);
            await ExecuteAsyncInternal(parameter).ConfigureAwait(false);
            IsBusy = false;
        }

        /// <summary>
        /// Async means internal implementation for Async* commands
        /// </summary>
        protected abstract Task ExecuteAsyncInternal(object? parameter);
    }
}