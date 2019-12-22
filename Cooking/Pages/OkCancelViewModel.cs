using Cooking.Commands;
using System.Threading.Tasks;

namespace Cooking.Pages
{
    public partial class OkCancelViewModel : DialogViewModel
    {
        public bool DialogResultOk { get; private set; }
        public AsyncDelegateCommand OkCommand { get; protected set; }

        public OkCancelViewModel(DialogService dialogService) : base(dialogService)
        {
            OkCommand = new AsyncDelegateCommand(Ok, CanOk);
        }

        protected virtual bool CanOk() => true;

        protected virtual async Task Ok()
        {
            DialogResultOk = true;
            Close();
        }
    }
}