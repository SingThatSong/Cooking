using System;
using System.Windows.Input;

namespace Cooking.Commands
{    
    /// <summary>
    /// Generic DelegateCommand where T is a parameter type
    /// </summary>
    public class DelegateCommand<T> : DelegateCommandBase
    {
        private readonly Func<T, bool>? _canExecute;
        private readonly Action<T> _execute;

        /// <param name="execute">Method, which is executed on Execute method</param>
        /// <param name="canExecute">Defines if method can be executed</param>
        /// <param name="executeOnce">Defines if method could be executed just once (useful for event bindings, such as OnLoading)</param>
        public DelegateCommand(Action<T> execute, Func<T, bool>? canExecute = null, bool executeOnce = false)
        {
            _execute = execute;
            _canExecute = canExecute;
            _canExecuteSpecified = _canExecute != null;
            _executeOnce = executeOnce;
        }

        protected override void ExecuteInternal(object? parameter) => _execute((T)parameter);
        protected override bool CanExecuteInternal(object? parameter) => _canExecute != null && parameter != null ? _canExecute((T)parameter) : true;
    }
}