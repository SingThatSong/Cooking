using System;

namespace Cooking.WPF.Commands
{
    /// <summary>
    /// Generic DelegateCommand where T is a parameter type.
    /// </summary>
    /// <typeparam name="T">Command parameter type.</typeparam>
    public class DelegateCommand<T> : DelegateCommandBase
    {
        private readonly Func<T, bool>? canExecute;
        private readonly Action<T> execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">Method, which is executed on Execute method.</param>
        /// <param name="canExecute">Defines if method can be executed.</param>
        /// <param name="executeOnce">Defines if method could be executed just once (useful for event bindings, such as OnLoading).</param>
        /// <param name="refreshAutomatically">A value indicating whether CanExecute should be refreshed automatically or manually.</param>
        /// <param name="exceptionHandler">An exception handler function.</param>
        public DelegateCommand(Action<T> execute,
                               Func<T, bool>? canExecute = null,
                               bool executeOnce = false,
                               bool refreshAutomatically = true,
                               Func<Exception, bool>? exceptionHandler = null)
            : base(executeOnce, refreshAutomatically, exceptionHandler)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            ExecuteOnce = executeOnce;

            CanExecuteSpecified = this.canExecute != null || ExecuteOnce;
        }

        /// <summary>
        /// Implementation of <see cref="DelegateCommandBase" /> ExecuteInternal.
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. May be ignored.</param>
        protected override void ExecuteInternal(object? parameter)
        {
            if (parameter is T tParameter)
            {
                execute(tParameter);
            }
            else if (parameter == null && typeof(T).IsClass)
            {
                execute(default);
            }
            else
            {
                throw new InvalidCastException("Command parameter is not T");
            }
        }

        /// <summary>
        /// Implementation of <see cref="DelegateCommandBase" /> CanExecuteInternal.
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. May be ignored.</param>
        /// <returns>If this command can be executed.</returns>
        protected override bool CanExecuteInternal(object? parameter)
        {
            if (canExecute != null)
            {
                if (parameter is T tParameter)
                {
                    return canExecute(tParameter);
                }
                else if (parameter == null && typeof(T).IsClass)
                {
                    return canExecute(default);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}