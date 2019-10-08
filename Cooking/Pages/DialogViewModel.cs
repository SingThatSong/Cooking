using Cooking.Commands;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;
using PropertyChanged;
using System.Threading.Tasks;

namespace Cooking.Pages.Recepies
{
    [AddINotifyPropertyChangedInterface]
    public partial class DialogViewModel
    {
        public AsyncDelegateCommand CloseCommand { get; }
        
        public DialogViewModel()
        {
            CloseCommand = new AsyncDelegateCommand(Close, executeOnce: true, freezeWhenBusy: true);
        }

        protected virtual async Task Close()
        {
            var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this).ConfigureAwait(false);
            await DialogCoordinator.Instance.HideMetroDialogAsync(this, current).ConfigureAwait(false);
        }
    }
}