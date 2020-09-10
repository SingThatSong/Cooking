using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;

namespace Cooking.WPF.Commands
{
    /// <summary>
    /// ICommand implementation for WPF bindings.
    /// </summary>
    public abstract class DelegateCommandBase : ICommand
    {
        private readonly Func<Exception, bool>? exceptionHandler;
        private EventHandler? canExecuteChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommandBase"/> class.
        /// </summary>
        /// <param name="executeOnce">Execute function only once, after that CanExecute would return false regardless.</param>
        /// <param name="refreshAutomatically">A value indicating whether CanExecute should be refreshed automatically or manually.</param>
        /// <param name="exceptionHandler">An exception handler function.</param>
        protected DelegateCommandBase(bool executeOnce,
                                      bool refreshAutomatically,
                                      Func<Exception, bool>? exceptionHandler = null)
        {
            ExecuteOnce = executeOnce;
            RefreshAutomatically = refreshAutomatically;
            this.exceptionHandler = exceptionHandler;
        }

        /// <summary>
        /// https://stackoverflow.com/a/7353704/1134449
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (CanExecuteSpecified)
                {
                    if (RefreshAutomatically)
                    {
                        CommandManager.RequerySuggested += value;
                    }
                    else
                    {
                        canExecuteChanged += value;
                    }
                }
            }
            remove
            {
                if (CanExecuteSpecified)
                {
                    if (RefreshAutomatically)
                    {
                        CommandManager.RequerySuggested -= value;
                    }
                    else
                    {
                        canExecuteChanged -= value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a global exception handler function.
        /// </summary>
        public static Func<Exception, bool>? GlobalExceptionHandler { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether command should be executed only once. Is set in child classes.
        /// </summary>
        protected bool ExecuteOnce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether command was executed. Needed when <see cref="ExecuteOnce"/> is true.
        /// </summary>
        protected bool Executed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CanExecute delegate or its substitutions are specified.
        /// <see cref="DelegateCommandBase"/> knows nothing about CanExecute delegates in child classes - they are of different types and set in constructors.
        /// Instead of delegate, it may be other indicators, such as one-time execution.
        /// </summary>
        protected bool CanExecuteSpecified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CanExecute should be refreshed automatically or manually with <see cref="RaiseCanExecuteChanged"/>.
        /// </summary>
        protected bool RefreshAutomatically { get; set; }

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
            try
            {
                ExecuteInternal(parameter);
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
                Executed = true;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Manually raise CanExecuteChanged command.
        /// </summary>
        [SuppressMessage("Design", "CA1030", Justification = "Name is intended to raise an event.")]
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteSpecified)
            {
                if (RefreshAutomatically)
                {
                    CommandManager.InvalidateRequerySuggested();
                }
                else
                {
                    canExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Method to execute when exception is thrown on <see cref="Execute(object?)"/>.
        /// </summary>
        /// <param name="exception">Occured exception.</param>
        /// <returns>Whether exception was handled.</returns>
        protected bool HandleException(Exception exception)
        {
            return exceptionHandler?.Invoke(exception) == true
                || GlobalExceptionHandler?.Invoke(exception) == true;
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