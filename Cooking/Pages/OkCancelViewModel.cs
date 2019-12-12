using Cooking.Commands;
using System.Threading.Tasks;

namespace Cooking.Pages
{
    public partial class OkCancelViewModel : DialogViewModel
    {
        public bool DialogResultOk { get; private set; }
        public AsyncDelegateCommand OkCommand { get; protected set; }

        public OkCancelViewModel() : base()
        {
            OkCommand = new AsyncDelegateCommand(Ok, CanOk, executeOnce: true, freezeWhenBusy: true);
        }

        protected virtual bool CanOk() => true;

        protected virtual async Task Ok()
        {
            DialogResultOk = true;
            await Close().ConfigureAwait(false);
        }
    }
}