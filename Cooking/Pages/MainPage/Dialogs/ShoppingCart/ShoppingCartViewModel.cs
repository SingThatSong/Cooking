using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;

namespace Cooking.Pages.Recepies
{
    public partial class ShoppingCartViewModel
    {
        public ShoppingCartViewModel(string list)
        {
            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

            List = list;
        }

        public Lazy<DelegateCommand> CloseCommand { get; }

        public string List { get; }
    }
}