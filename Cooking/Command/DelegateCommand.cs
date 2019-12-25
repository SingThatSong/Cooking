using System;

namespace Cooking.Commands
{
    /// <summary>
    /// DelegateCommand without parameters
    /// </summary>
    public class DelegateCommand : DelegateCommandBase
    {
        private readonly Func<bool>? _canExecute;
        private readonly Action _execute;

        /// <param name="execute">Method, which is executed on Execute method</param>
        /// <param name="canExecute">Defines if method can be executed</param>
        /// <param name="executeOnce">Defines if method could be executed just once (useful for event bindings, such as OnLoading)</param>
        public DelegateCommand(Action execute, Func<bool>? canExecute = null, bool executeOnce = false)
        {
            _execute = execute;
            _canExecute = canExecute;
            ExecuteOnce = executeOnce;

            CanExecuteSpecified = _canExecute != null || ExecuteOnce;
        }

        protected override void ExecuteInternal(object? parameter) => _execute();
        protected override bool CanExecuteInternal(object? parameter) => _canExecute != null ? _canExecute() : true;
    }
}