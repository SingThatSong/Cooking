using Cooking.Commands;
using PropertyChanged;
using System.Threading.Tasks;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public partial class DialogViewModel
    {
        protected DialogService DialogService { get; }

        public AsyncDelegateCommand CloseCommand { get; }
        
        public DialogViewModel(DialogService dialogService)
        {
            CloseCommand = new AsyncDelegateCommand(Close);
            DialogService = dialogService;
        }

        protected virtual async Task Close() => await DialogService.HideCurrentDialogAsync().ConfigureAwait(false);
    }
}