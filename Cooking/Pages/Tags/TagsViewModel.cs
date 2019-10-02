using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Recepies;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
        public DelegateCommand<TagDTO> EditTagCommand { get; }
        public DelegateCommand<Guid> DeleteTagCommand { get; }
        public DelegateCommand LoadedCommand { get; }

        public TagsViewModel()
        {
            LoadedCommand = new DelegateCommand(OnLoaded, executeOnce: true);

            AddTagCommand = new DelegateCommand(AddTag);
            DeleteTagCommand = new DelegateCommand<Guid>(DeleteTag);
            ViewTagCommand = new DelegateCommand<TagDTO>(ViewTag);
            EditTagCommand = new DelegateCommand<TagDTO>(EditTag);
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
                ((mainWindowViewModel.SelectedMenuItem.Tag as RecepiesView).DataContext as RecepiesViewModel).FilterText = $"~{tag.Name}";
            }
        }

        private async void EditTag(TagDTO tag)
        {
            var viewModel = new TagEditViewModel(tag.MapTo<TagDTO>());
            await new DialogUtils(this).ShowCustomMessageAsync<TagEditView, TagEditViewModel>("Редактирование тега", viewModel);

            if (viewModel.DialogResultOk)
            {
                await TagService.UpdateTagAsync(viewModel.Tag.MapTo<Tag>());
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
                });

            if (result == MessageDialogResult.Affirmative)
            {
                await TagService.DeleteAsync(recipeId);
                Tags.Remove(Tags.Single(x => x.ID == recipeId));
            }
        }

        public async void AddTag()
        {
            var viewModel = await new DialogUtils(this).ShowCustomMessageAsync<TagEditView, TagEditViewModel>("Новый тег");

            if (viewModel.DialogResultOk)
            {
                var id = await TagService.CreateAsync(viewModel.Tag.MapTo<Tag>());
                viewModel.Tag.ID = id;
                Tags.Add(viewModel.Tag);
            }
        }

    }
}