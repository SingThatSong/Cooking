using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Recepies;
using Data.Context;
using Data.Model;
using Data.Model.Plan;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Cooking.Pages.Garnishes
{
    public partial class GarnishesViewModel : INotifyPropertyChanged
    {
        public GarnishesViewModel()
        {
            Tags = new Lazy<ObservableCollection<GarnishDTO>>(GetTags);
            AddTagCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(AddTag));
            DeleteTagCommand = new Lazy<DelegateCommand<GarnishDTO>>(() => new DelegateCommand<GarnishDTO>(cat => DeleteTag(cat.ID)));
            ViewTagCommand = new Lazy<DelegateCommand<GarnishDTO>>(() => new DelegateCommand<GarnishDTO>(tag =>
            {
                if (Application.Current.MainWindow.DataContext is MainWindowViewModel mainWindowViewModel)
                {
                    mainWindowViewModel.SelectedMenuItem = mainWindowViewModel.MenuItems[1] as HamburgerMenuIconItem;
                    ((mainWindowViewModel.SelectedMenuItem.Tag as RecepiesView).DataContext as RecepiesViewModel).FilterText = $"~{tag.Name}";
                }
            }));

            EditTagCommand = new Lazy<DelegateCommand<GarnishDTO>>(
                () => new DelegateCommand<GarnishDTO>(async (tag) => {

                    var viewModel = new GarnishEditViewModel(Mapper.Map<GarnishDTO>(tag));

                    var dialog = new CustomDialog()
                    {
                        Title = "Редактирование тега",
                        Content = new GarnishEditView()
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
                            var existing = context.Garnishes.Find(tag.ID);
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
                    var category = await context.Garnishes.FindAsync(recipeId);
                    context.Garnishes.Remove(category);
                    context.SaveChanges();
                }

                Tags.Value.Remove(Tags.Value.Single(x => x.ID == recipeId));
            }
        }

        public async void AddTag()
        {
            var viewModel = new GarnishEditViewModel();

            var dialog = new CustomDialog()
            {
                Title = "Новый тег",
                Content = new GarnishEditView()
                {
                    DataContext = viewModel
                }
            };

            await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
            await dialog.WaitUntilUnloadedAsync();

            if (viewModel.DialogResultOk)
            {
                var category = Mapper.Map<Garnish>(viewModel.Tag);
                using (var context = new CookingContext())
                {
                    context.Add(category);
                    context.SaveChanges();
                }
                viewModel.Tag.ID = category.ID;
                Tags.Value.Add(viewModel.Tag);
            }
        }

        public Lazy<ObservableCollection<GarnishDTO>> Tags { get; }
        private ObservableCollection<GarnishDTO> GetTags()
        {
            try
            {
                using (var context = new CookingContext())
                {
                    var originalList = context.Garnishes.ToList();
                    return new ObservableCollection<GarnishDTO>(
                        originalList.Select(x => Mapper.Map<GarnishDTO>(x))
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
        public Lazy<DelegateCommand<GarnishDTO>> ViewTagCommand { get; }
        public Lazy<DelegateCommand<GarnishDTO>> EditTagCommand { get; }
        public Lazy<DelegateCommand<GarnishDTO>> DeleteTagCommand { get; }

        public bool IsEditing { get; set; }
    }
}