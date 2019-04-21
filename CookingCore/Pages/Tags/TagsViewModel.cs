using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Recepies;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Cooking.Pages.Tags
{
    public partial class TagsViewModel : INotifyPropertyChanged
    {
        public TagsViewModel()
        {
            Tags = new Lazy<ObservableCollection<TagDTO>>(GetTags);
            AddTagCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(AddTag));
            DeleteTagCommand = new Lazy<DelegateCommand<TagDTO>>(() => new DelegateCommand<TagDTO>(cat => DeleteTag(cat.ID)));
            ViewTagCommand = new Lazy<DelegateCommand<TagDTO>>(() => new DelegateCommand<TagDTO>(tag =>
            {
                if (Application.Current.MainWindow.DataContext is MainWindowViewModel mainWindowViewModel)
                {
                    mainWindowViewModel.SelectedMenuItem = mainWindowViewModel.MenuItems[1] as HamburgerMenuIconItem;
                    ((mainWindowViewModel.SelectedMenuItem.Tag as RecepiesView).DataContext as RecepiesViewModel).FilterText = $"~{tag.Name}";
                }
            }));

            EditTagCommand = new Lazy<DelegateCommand<TagDTO>>(
                () => new DelegateCommand<TagDTO>(async (tag) => {

                    var viewModel = new TagEditViewModel(Mapper.Map<TagDTO>(tag));

                    var dialog = new CustomDialog()
                    {
                        Title = "Редактирование тега",
                        Content = new TagEditView()
                        {
                            DataContext = viewModel
                        }
                    };
                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
                    await dialog.WaitUntilUnloadedAsync();

                    if (viewModel.DialogResultOk)
                    {
                        using (var context = new CookingContext())
                        {
                            var existing = context.Tags.Find(tag.ID);
                            Mapper.Map(viewModel.Tag, existing);
                            context.SaveChanges();
                        }

                        var existingRecipe = Tags.Value.Single(x => x.ID == tag.ID);
                        Mapper.Map(viewModel.Tag, existingRecipe);
                    }
                }));
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
                using (var context = new CookingContext())
                {
                    var category = await context.Tags.FindAsync(recipeId);
                    context.Tags.Remove(category);
                    context.SaveChanges();
                }

                Tags.Value.Remove(Tags.Value.Single(x => x.ID == recipeId));
            }
        }

        public async void AddTag()
        {
            var viewModel = new TagEditViewModel();

            var dialog = new CustomDialog()
            {
                Title = "Новый тег",
                Content = new TagEditView()
                {
                    DataContext = viewModel
                }
            };

            await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
            await dialog.WaitUntilUnloadedAsync();

            if (viewModel.DialogResultOk)
            {
                var category = Mapper.Map<Tag>(viewModel.Tag);
                using (var context = new CookingContext())
                {
                    context.Add(category);
                    context.SaveChanges();
                }
                viewModel.Tag.ID = category.ID;
                Tags.Value.Add(viewModel.Tag);
            }
        }

        public Lazy<ObservableCollection<TagDTO>> Tags { get; }
        private ObservableCollection<TagDTO> GetTags()
        {
            try
            {
                using (var Context = new CookingContext())
                {
                    var originalList = Context.Tags.ToList();
                    return new ObservableCollection<TagDTO>(
                        originalList.Select(x => Mapper.Map<TagDTO>(x))
                    );
                }
            }
            catch
            {
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public Lazy<DelegateCommand> AddTagCommand { get; }
        public Lazy<DelegateCommand<TagDTO>> ViewTagCommand { get; }
        public Lazy<DelegateCommand<TagDTO>> EditTagCommand { get; }
        public Lazy<DelegateCommand<TagDTO>> DeleteTagCommand { get; }

        public bool IsEditing { get; set; }
    }
}