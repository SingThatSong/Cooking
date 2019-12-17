﻿using System;
using System.Threading.Tasks;

namespace Cooking.Commands
{
    public class AsyncDelegateCommand<T> : AsyncDelegateCommandBase
    {
        private readonly Func<T, bool>? _canExecute;
        private readonly Func<T, Task> _execute;
               
        public AsyncDelegateCommand(Func<T, Task> execute,
                       Func<T, bool>? canExecute = null,
                       bool executeOnce = false, 
                       bool freezeWhenBusy = false)
        {
            _execute = execute;
            _canExecute = canExecute;
            ExecuteOnce = executeOnce;
            CanExecuteSpecified = canExecute != null;
            FreezeWhenBusy = freezeWhenBusy;
        }

        protected override bool CanExecuteAsyncInternal(object? parameter) => _canExecute != null ? _canExecute((T)parameter) : true;

        protected override async Task ExecuteAsyncInternal(object? parameter) => await _execute((T)parameter).ConfigureAwait(false);
    }
}
