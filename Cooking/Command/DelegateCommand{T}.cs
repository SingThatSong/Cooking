using System;
using System.Windows.Input;

namespace Cooking.Commands
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Func<T, bool> _canExecute;
        private readonly bool _executeOnce;
        private readonly Action<T> _execute;

        private bool _executed;

        /// <summary>
        /// https://stackoverflow.com/a/7353704/1134449
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action<T> execute, bool executeOnce = false)
                             : this(execute, null)
        {
        }

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute, bool executeOnce = false)
        {
            _execute = execute;
            _canExecute = canExecute;
            _executeOnce = executeOnce;
        }

        public bool CanExecute(object parameter)
        {
            if (_executeOnce && _executed)
            {
                return false;
            }

            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}