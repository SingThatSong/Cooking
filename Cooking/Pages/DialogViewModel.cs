using Cooking.Commands;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;
using PropertyChanged;

namespace Cooking.Pages.Recepies
{
    [AddINotifyPropertyChangedInterface]
    public partial class DialogViewModel
    {
        public DelegateCommand CloseCommand { get; }
        
        public DialogViewModel()
        {
            CloseCommand = new DelegateCommand(Close, executeOnce: true);
        }

        protected virtual async void Close()
        {
            var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
            await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
        }
    }
}