using System;
using System.Threading.Tasks;

namespace Cooking.WPF.Commands
{
    public class AsyncDelegateCommand : AsyncDelegateCommandBase
    {
        private readonly Func<bool>? canExecute;
        private readonly Func<Task> execute;

        public AsyncDelegateCommand(Func<Task> execute,
                                    Func<bool>? canExecute = null,
                                    bool executeOnce = false,
                                    bool freezeWhenBusy = false)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            ExecuteOnce = executeOnce;
            FreezeWhenBusy = freezeWhenBusy;

            CanExecuteSpecified = canExecute != null || FreezeWhenBusy || ExecuteOnce;
        }

        protected override bool CanExecuteAsyncInternal(object? parameter) => canExecute != null ? canExecute() : true;

        protected override async Task ExecuteAsyncInternal(object? parameter) => await execute();
    }
}
