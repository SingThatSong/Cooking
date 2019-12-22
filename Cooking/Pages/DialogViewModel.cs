﻿using Cooking.Commands;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using System.Threading.Tasks;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public partial class DialogViewModel
    {
        private readonly DialogService dialogService;

        public AsyncDelegateCommand CloseCommand { get; }
        
        public DialogViewModel(DialogService dialogService)
        {
            CloseCommand = new AsyncDelegateCommand(Close);
            this.dialogService = dialogService;
        }

        protected virtual async Task Close()
        {
            await dialogService.HideCurrentDialogAsync().ConfigureAwait(false);
        }
    }
}