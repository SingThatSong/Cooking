using System;

namespace Cooking.WPF.Commands
{
    /// <summary>
    /// DelegateCommand without parameters.
    /// </summary>
    public class DelegateCommand : DelegateCommandBase
    {
        private readonly Func<bool>? canExecute;
        private readonly Action execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="execute">Method, which is executed on Execute method.</param>
        /// <param name="canExecute">Defines if method can be executed.</param>
        /// <param name="executeOnce">Defines if method could be executed just once (useful for event bindings, such as OnLoading).</param>
        /// <param name="refreshAutomatically">A value indicating whether CanExecute should be refreshed automatically or manually.</param>
        /// <param name="exceptionHandler">An exception handler function.</param>
        public DelegateCommand(Action execute,
                               Func<bool>? canExecute = null,
                               bool executeOnce = false,
                               bool refreshAutomatically = true,
                               Func<Exception, bool>? exceptionHandler = null)
            : base(executeOnce, refreshAutomatically, exceptionHandler)
        {
            this.execute = execute;
            this.canExecute = canExecute;

            CanExecuteSpecified = this.canExecute != null || ExecuteOnce;
        }

        /// <summary>
        /// Implementation of <see cref="DelegateCommandBase" /> ExecuteInternal.
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. Ignored.</param>
        protected override void ExecuteInternal(object? parameter) => execute();

        /// <summary>
        /// Implementation of <see cref="DelegateCommandBase" /> CanExecuteInternal.
        /// </summary>
        /// <param name="parameter">Parameter, provided in CommandParameter attribute. Ignored.</param>
        /// <returns>If this command can be executed.</returns>
        protected override bool CanExecuteInternal(object? parameter) => canExecute == null || canExecute();
    }
}