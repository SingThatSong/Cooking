using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Tags;

using Data.Model;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Ioc;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public partial class TagsViewModel
    {
        private readonly IContainerExtension container;

        public ObservableCollection<TagEdit>? Tags { get; private set; }
        public bool IsEditing { get; set; }

        public DelegateCommand AddTagCommand { get; }
        public DelegateCommand<TagEdit> ViewTagCommand { get; }
        public AsyncDelegateCommand<TagEdit> EditTagCommand { get; }
        public DelegateCommand<Guid> DeleteTagCommand { get; }
        public DelegateCommand LoadedCommand { get; }

        public TagsViewModel(IContainerExtension container)
        {
            LoadedCommand = new DelegateCommand(OnLoaded, executeOnce: true);

            AddTagCommand = new DelegateCommand(AddTag);
            DeleteTagCommand = new DelegateCommand<Guid>(DeleteTag);
            ViewTagCommand = new DelegateCommand<TagEdit>(ViewTag);
            EditTagCommand = new AsyncDelegateCommand<TagEdit>(EditTag);
            this.container = container;
        }

        private void OnLoaded()
        {
            Debug.WriteLine("TagsViewModel.OnLoaded");
            Tags = TagService.GetTags()
                             .MapTo<ObservableCollection<TagEdit>>();
        }

        private void ViewTag(TagEdit tag)
        {
            var recepiesView = container.Resolve<Recepies>();
            if (recepiesView.DataContext is RecepiesViewModel recepiesViewModel)
            {
                recepiesViewModel.FilterText = $"{Consts.TagSymbol}\"{tag.Name}\"";

                var mainWindowViewModel = container.Resolve<MainWindow>().DataContext as MainWindowViewModel;
                mainWindowViewModel!.SelectMenuItemByViewType(recepiesView.GetType());
            }
        }

        private async Task EditTag(TagEdit tag)
        {
            var viewModel = new TagEditViewModel(tag.MapTo<TagEdit>());
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
                Tags!.Add(viewModel.Tag);
            }
        }

    }
}