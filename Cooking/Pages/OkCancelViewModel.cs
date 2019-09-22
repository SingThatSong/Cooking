using Cooking.Commands;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;

namespace Cooking.Pages.Recepies
{
    public partial class OkCancelViewModel : DialogViewModel
    {
        public bool DialogResultOk { get; private set; }
        public DelegateCommand OkCommand { get; }

        public OkCancelViewModel() : base()
        {
            OkCommand = new DelegateCommand(Ok, CanOk, executeOnce: true);
        }

        protected virtual bool CanOk() => true;

        protected virtual void Ok()
        {
            DialogResultOk = true;
            Close();
        }
    }
}