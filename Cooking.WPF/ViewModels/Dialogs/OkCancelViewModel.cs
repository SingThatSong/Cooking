using System.ComponentModel;
using System.Threading.Tasks;
using Cooking.WPF.Validation;
using WPF.Commands;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// Base view model for ok/cancel dialogs.
    /// </summary>
    public partial class OkCancelViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OkCancelViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service to be able to close dialog.</param>
        public OkCancelViewModel(DialogService dialogService)
        {
            DialogService = dialogService;
            CloseCommand = new AsyncDelegateCommand(Close);
            OkCommand = new AsyncDelegateCommand(OkAsync, CanOk);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets a value indicating whether result of dialog execution is ok.
        /// </summary>
        public bool DialogResultOk { get; private set; }

        /// <summary>
        /// Gets command for clicking on Ok button.
        /// </summary>
        public AsyncDelegateCommand OkCommand { get; }

        /// <summary>
        /// Gets command for clicking on Cancel button.
        /// </summary>
        public AsyncDelegateCommand CloseCommand { get; }

        /// <summary>
        /// Gets dialog service dependency.
        /// </summary>
        protected DialogService DialogService { get; }

        /// <summary>
        /// Method to invoke <see cref="PropertyChanged"/>.
        /// </summary>
        /// <param name="property">Name of changed property.</param>
        protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        /// <summary>
        /// Determine if ok button can be pressed.
        /// </summary>
        /// <returns>Ture if ok button can be pressed.</returns>
        protected virtual bool CanOk() => this.IsValid(true);

        /// <summary>
        /// Close current dialog and set <see cref="DialogResultOk"/> to true.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        protected virtual async Task OkAsync()
        {
            DialogResultOk = true;
            await Close();
        }

        /// <summary>
        /// Close current dialog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        private async Task Close() => await DialogService.HideCurrentDialogAsync();
    }
}