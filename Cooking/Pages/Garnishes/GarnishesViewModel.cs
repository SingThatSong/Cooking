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
            Garnishes = new Lazy<ObservableCollection<GarnishDTO>>(GetGarnishes);
            AddGarnishCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(AddGarnish));
            DeleteGarnishCommand = new Lazy<DelegateCommand<GarnishDTO>>(() => new DelegateCommand<GarnishDTO>(cat => DeleteGarnish(cat.ID)));

            EditGarnishCommand = new Lazy<DelegateCommand<GarnishDTO>>(
                () => new DelegateCommand<GarnishDTO>(async (tag) => {

                    var viewModel = new GarnishEditViewModel(Mapper.Map<GarnishDTO>(tag));

                    var dialog = new CustomDialog()
                    {
                        Title = "Редактирование гарнира",
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
                            Mapper.Map(viewModel.Garnish, existing);
                            context.SaveChanges();
                        }

                        var existingRecipe = Garnishes.Value.Single(x => x.ID == tag.ID);
                        Mapper.Map(viewModel.Garnish, existingRecipe);
                    }
                }));
        }

        public async void DeleteGarnish(Guid recipeId)
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

                Garnishes.Value.Remove(Garnishes.Value.Single(x => x.ID == recipeId));
            }
        }

        public async void AddGarnish()
        {
            var viewModel = new GarnishEditViewModel();

            var dialog = new CustomDialog()
            {
                Title = "Новый гарнир",
                Content = new GarnishEditView()
                {
                    DataContext = viewModel
                }
            };

            await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
            await dialog.WaitUntilUnloadedAsync();

            if (viewModel.DialogResultOk)
            {
                var category = Mapper.Map<Garnish>(viewModel.Garnish);
                using (var context = new CookingContext())
                {
                    context.Add(category);
                    context.SaveChanges();
                }
                viewModel.Garnish.ID = category.ID;
                Garnishes.Value.Add(viewModel.Garnish);
            }
        }

        public Lazy<ObservableCollection<GarnishDTO>> Garnishes { get; }
        private ObservableCollection<GarnishDTO> GetGarnishes()
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
        
        public Lazy<DelegateCommand> AddGarnishCommand { get; }
        public Lazy<DelegateCommand<GarnishDTO>> EditGarnishCommand { get; }
        public Lazy<DelegateCommand<GarnishDTO>> DeleteGarnishCommand { get; }

        public bool IsEditing { get; set; }
    }
}