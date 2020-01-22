using Cooking.WPF.Commands;
using PropertyChanged;
using System.Threading.Tasks;

namespace Cooking.WPF.Views
{
    [AddINotifyPropertyChangedInterface]
    public partial class DialogViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogViewModel"/> class.
        /// </summary>
        /// <param name="dialogService"></param>
        public DialogViewModel(DialogService dialogService)
        {
            CloseCommand = new AsyncDelegateCommand(Close);
            DialogService = dialogService;
        }

        protected DialogService DialogService { get; }
        public AsyncDelegateCommand CloseCommand { get; }

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        protected virtual async Task Close() => await DialogService.HideCurrentDialogAsync();
    }
}