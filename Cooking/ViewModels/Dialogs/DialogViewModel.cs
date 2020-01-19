using Cooking.WPF.Commands;
using PropertyChanged;
using System.Threading.Tasks;

namespace Cooking.WPF.Views
{
    [AddINotifyPropertyChangedInterface]
    public partial class DialogViewModel
    {
        public DialogViewModel(DialogService dialogService)
        {
            CloseCommand = new AsyncDelegateCommand(Close);
            DialogService = dialogService;
        }

        protected DialogService DialogService { get; }
        public AsyncDelegateCommand CloseCommand { get; }

        protected virtual async Task Close() => await DialogService.HideCurrentDialogAsync();
    }
}