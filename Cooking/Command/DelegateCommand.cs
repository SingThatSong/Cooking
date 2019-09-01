using System;
using System.Windows.Input;

namespace Cooking.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly bool _executeOnce;
        private readonly Action _execute;

        private bool _executed;

        /// <summary>
        /// https://stackoverflow.com/a/7353704/1134449
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action execute, bool executeOnce = false)
                       : this(execute, null, executeOnce)
        {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute, bool executeOnce = false)
        {
            _execute = execute;
            _canExecute = canExecute;
            _executeOnce = executeOnce;
        }

        public bool CanExecute()
        {
            return CanExecute(null);
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

            return _canExecute();
        }

        public void Execute()
        {
            Execute(null);
        }

        public void Execute(object parameter)
        {
            _executed = true;
            _execute();
        }
    }
}