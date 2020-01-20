using System;

namespace Cooking.WPF.Commands
{
    /// <summary>
    /// Generic DelegateCommand where T is a parameter type.
    /// </summary>
    public class DelegateCommand<T> : DelegateCommandBase
    {
        private readonly Func<T, bool>? canExecute;
        private readonly Action<T> execute;

        /// <param name="execute">Method, which is executed on Execute method.</param>
        /// <param name="canExecute">Defines if method can be executed.</param>
        /// <param name="executeOnce">Defines if method could be executed just once (useful for event bindings, such as OnLoading).</param>
        public DelegateCommand(Action<T> execute, Func<T, bool>? canExecute = null, bool executeOnce = false)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            ExecuteOnce = executeOnce;

            CanExecuteSpecified = this.canExecute != null || ExecuteOnce;
        }

        protected override void ExecuteInternal(object? parameter)
        {
            if (parameter is T tParameter)
            {
                execute(tParameter);
            }
            else
            {
                throw new InvalidCastException("Command parameter is not T");
            }
        }

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
#pragma warning disable CS8653 // Выражение по умолчанию вводит значение NULL для параметра типа.
                    return canExecute(default);
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