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

        /// <param name="execute">Method, which is executed on Execute method.</param>
        /// <param name="canExecute">Defines if method can be executed.</param>
        /// <param name="executeOnce">Defines if method could be executed just once (useful for event bindings, such as OnLoading).</param>
        public DelegateCommand(Action execute, Func<bool>? canExecute = null, bool executeOnce = false)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            ExecuteOnce = executeOnce;

            CanExecuteSpecified = this.canExecute != null || ExecuteOnce;
        }

        protected override void ExecuteInternal(object? parameter) => execute();
        protected override bool CanExecuteInternal(object? parameter) => canExecute != null ? canExecute() : true;
    }
}