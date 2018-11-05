using Cooking.DTO;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.ComponentModel;

namespace Cooking.Pages.Tags
{
    public partial class TagEditViewModel : INotifyPropertyChanged
    {
        public bool DialogResultOk { get; set; }


        public TagEditViewModel(TagDTO category = null)
        {
            OkCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    DialogResultOk = true;
                    CloseCommand.Value.Execute();
                }));

            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

            Tag = category ?? new TagDTO();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }
        public TagDTO Tag { get; set; }
    }
}