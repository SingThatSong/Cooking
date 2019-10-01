using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cooking.Command
{
    public class AsyncDelegateCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Func<Task> _execute;
        private bool _freezeWhenBusy { get; }

        private bool _isBusy;
        private bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                if (_freezeWhenBusy)
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// https://stackoverflow.com/a/7353704/1134449
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AsyncDelegateCommand(Func<Task> execute,
                                    Func<bool> canExecute = null,
                                    bool freezeWhenBusy = false)
        {
            _execute = execute;
            _canExecute = canExecute;
            _freezeWhenBusy = freezeWhenBusy;
        }

        public bool CanExecute(object parameter)
        {
            if (_freezeWhenBusy && IsBusy)
            {
                return false;
            }

            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute();
        }

        public async void Execute(object parameter)
        {
            IsBusy = true;
            // Принудительно переводим выполнение в фоновый поток
            await Task.Delay(1).ConfigureAwait(false);
            await _execute();
            IsBusy = false;
        }
    }
}
