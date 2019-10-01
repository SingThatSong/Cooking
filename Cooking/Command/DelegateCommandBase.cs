using System;
using System.Windows.Input;

namespace Cooking.Commands
{
    /// <summary>
    /// ICommand implementation for WPF bindings
    /// </summary>
    public abstract class DelegateCommandBase : ICommand
    {
        protected bool _executeOnce;
        protected bool _executed;
        protected bool _canExecuteSpecified;

        /// <summary>
        /// https://stackoverflow.com/a/7353704/1134449
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Implementation of ICommand.CanExecute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object? parameter = null)
        {
            // Single execution
            if (_executeOnce && _executed)
            {
                return false;
            }

            // Always can execute if no condition is specified
            if (!_canExecuteSpecified)
            {
                return true;
            }

            // Calling abstract method
            return CanExecuteInternal(parameter);
        }

        /// <summary>
        /// Provide implementation of CanExecute, keep it to buisness logic
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. May be ignored</param>
        /// <returns>If this command can be executed</returns>
        protected abstract bool CanExecuteInternal(object? parameter);
        
        public void Execute(object? parameter = null)
        {
            // Calling abstract method
            ExecuteInternal(parameter);
            _executed = true;
        }

        /// <summary>
        /// Provide implementation of Execute, keep it to buisness logic
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. May be ignored</param>
        protected abstract void ExecuteInternal(object? parameter);
    }
}