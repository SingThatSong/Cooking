using System;
using System.Windows.Input;

namespace Cooking.WPF.Commands
{
    /// <summary>
    /// ICommand implementation for WPF bindings.
    /// </summary>
    public abstract class DelegateCommandBase : ICommand
    {
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
        /// Gets or sets a value indicating whether command should be executed only once. Is set in child classes.
        /// </summary>
        protected bool ExecuteOnce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether command was executed. Needed when <see cref="ExecuteOnce"/> is true.
        /// </summary>
        protected bool Executed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CanExecute delegate or its substiturions is specified.
        /// <see cref="DelegateCommandBase"/> knows nothing about CanExecute delegates in child classes - they are of different types and set in constructors.
        /// Instead of delegate, it may be other indicators, such as one-time execution.
        /// </summary>
        protected bool CanExecuteSpecified { get; set; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void Execute(object? parameter = null)
        {
            // Calling abstract method
            ExecuteInternal(parameter);
            Executed = true;
        }

        /// <summary>
        /// Provide implementation of CanExecute, keep it to buisness logic.
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. May be ignored.</param>
        /// <returns>If this command can be executed.</returns>
        protected abstract bool CanExecuteInternal(object? parameter);

        /// <summary>
        /// Provide implementation of Execute, keep it to buisness logic.
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. May be ignored.</param>
        protected abstract void ExecuteInternal(object? parameter);
    }
}