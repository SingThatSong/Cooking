using System;

namespace Cooking.WPF.Commands
{
    /// <summary>
    /// Generic DelegateCommand where T is a parameter type.
    /// </summary>
    public class DelegateCommand<T> : DelegateCommandBase
    {
        private readonly Func<T, bool>? _canExecute;
        private readonly Action<T> _execute;

        /// <param name="execute">Method, which is executed on Execute method.</param>
        /// <param name="canExecute">Defines if method can be executed.</param>
        /// <param name="executeOnce">Defines if method could be executed just once (useful for event bindings, such as OnLoading).</param>
        public DelegateCommand(Action<T> execute, Func<T, bool>? canExecute = null, bool executeOnce = false)
        {
            _execute = execute;
            _canExecute = canExecute;
            ExecuteOnce = executeOnce;

            CanExecuteSpecified = _canExecute != null || ExecuteOnce;
        }

        protected override void ExecuteInternal(object? parameter)
        {
            if (parameter is T tParameter)
            {
                _execute(tParameter);
            }
            else
            {
                throw new InvalidCastException("Command parameter is not T");
            }
        }

        protected override bool CanExecuteInternal(object? parameter)
        {
            if (_canExecute != null)
            {
                if (parameter is T tParameter)
                {
                    return _canExecute(tParameter);
                }
                else if (parameter == null && typeof(T).IsClass)
                {
#pragma warning disable CS8653 // Выражение по умолчанию вводит значение NULL для параметра типа.
                    return _canExecute(default);
#pragma warning restore CS8653
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