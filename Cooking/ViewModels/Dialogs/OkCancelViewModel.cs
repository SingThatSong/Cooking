using Cooking.WPF.Commands;
using System.Threading.Tasks;

namespace Cooking.WPF.Views
{
    public partial class OkCancelViewModel : DialogViewModel
    {
        public bool DialogResultOk { get; private set; }
        public AsyncDelegateCommand OkCommand { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OkCancelViewModel"/> class.
        /// </summary>
        /// <param name="dialogService"></param>
        public OkCancelViewModel(DialogService dialogService)
            : base(dialogService)
        {
            OkCommand = new AsyncDelegateCommand(Ok, CanOk);
        }

        protected virtual bool CanOk() => true;

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        protected virtual async Task Ok()
        {
            DialogResultOk = true;
            await Close();
        }
    }
}