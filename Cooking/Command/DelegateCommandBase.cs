using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Cooking.Commands
{
    /// <summary>
    /// ICommand implementation for WPF bindings
    /// </summary>
    public abstract class DelegateCommandBase : ICommand
    {
        protected bool ExecuteOnce { get; set; }
        protected bool Executed { get; set; }
        protected bool CanExecuteSpecified { get; set; }

        /// <summary>
        /// https://stackoverflow.com/a/7353704/1134449
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add 
            {
                if (CanExecuteSpecified)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (CanExecuteSpecified)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        /// <summary>
        /// Implementation of ICommand.CanExecute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object? parameter = null)
        {
            // Single execution
            if (ExecuteOnce && Executed)
            {
                return false;
            }

            // Always can execute if no condition is specified
            if (!CanExecuteSpecified)
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
            Executed = true;
        }

        /// <summary>
        /// Provide implementation of Execute, keep it to buisness logic
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. May be ignored</param>
        protected abstract void ExecuteInternal(object? parameter);
    }
}