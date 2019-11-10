using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Recepies;
using Data.Model;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cooking.Pages.Tags
{
    [AddINotifyPropertyChangedInterface]
    public partial class TagsViewModel
    {
        public ObservableCollection<TagDTO>? Tags { get; set; }
        public bool IsEditing { get; set; }

        public DelegateCommand AddTagCommand { get; }
        public DelegateCommand<TagDTO> ViewTagCommand { get; }
        public AsyncDelegateCommand<TagDTO> EditTagCommand { get; }
        public DelegateCommand<Guid> DeleteTagCommand { get; }
        public DelegateCommand LoadedCommand { get; }

        public TagsViewModel()
        {
            LoadedCommand = new DelegateCommand(OnLoaded, executeOnce: true);

            AddTagCommand = new DelegateCommand(AddTag);
            DeleteTagCommand = new DelegateCommand<Guid>(DeleteTag);
            ViewTagCommand = new DelegateCommand<TagDTO>(ViewTag);
            EditTagCommand = new AsyncDelegateCommand<TagDTO>(EditTag);
        }

        private void OnLoaded()
        {
            Debug.WriteLine("TagsViewModel.OnLoaded");
            Tags = TagService.GetTags()
                             .MapTo<ObservableCollection<TagDTO>>();
        }

        private void ViewTag(TagDTO tag)
        {
            if (Application.Current.MainWindow.DataContext is MainWindowViewModel mainWindowViewModel)
            {
                mainWindowViewModel.SelectedMenuItem = mainWindowViewModel.MenuItems[1] as HamburgerMenuIconItem;
                if (mainWindowViewModel.SelectedMenuItem != null 
                    && mainWindowViewModel.SelectedMenuItem.Tag is RecepiesView recepiesView
                    && recepiesView.DataContext is RecepiesViewModel recepiesViewModel)
                {
                    recepiesViewModel.FilterText = $"{Consts.TagSymbol}{tag.Name}";
                }
            }
        }

        private async Task EditTag(TagDTO tag)
        {
            var viewModel = new TagEditViewModel(tag.MapTo<TagDTO>());
            await new DialogUtils(this).ShowCustomMessageAsync<TagEditView, TagEditViewModel>("Редактирование тега", viewModel).ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                await TagService.UpdateTagAsync(viewModel.Tag.MapTo<Tag>()).ConfigureAwait(false);
                var existingTag = Tags.Single(x => x.ID == tag.ID);
                viewModel.Tag.MapTo(existingTag);
            }
        }


        public async void DeleteTag(Guid recipeId)
        {
            var result = await DialogCoordinator.Instance.ShowMessageAsync(
                this, 
                "Точно удалить?",
                "Восстановить будет нельзя",
                style: MessageDialogStyle.AffirmativeAndNegative,
                settings: new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Да",
                    NegativeButtonText = "Нет"
                }).ConfigureAwait(true);

            if (result == MessageDialogResult.Affirmative)
            {
                await TagService.DeleteAsync(recipeId).ConfigureAwait(true);
                Tags!.Remove(Tags.Single(x => x.ID == recipeId));
            }
        }

        public async void AddTag()
        {
            var viewModel = await new DialogUtils(this).ShowCustomMessageAsync<TagEditView, TagEditViewModel>("Новый тег").ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                var id = await TagService.CreateAsync(viewModel.Tag.MapTo<Tag>()).ConfigureAwait(false);
                viewModel.Tag.ID = id;
                Tags.Add(viewModel.Tag);
            }
        }

    }
}