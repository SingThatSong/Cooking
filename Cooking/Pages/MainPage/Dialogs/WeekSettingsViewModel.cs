using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.MainPage.Dialogs.Model;
using Cooking.Pages.MainPage.Dialogs.Model.CalorieTypeSelect;
using Cooking.Pages.Recepies;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.Pages.MainPage.Dialogs
{
    public class WeekSettingsViewModel
    {
        public WeekSettingsViewModel(DateTime weekStart, DateTime weekEnd)
        {
            WeekStart = weekStart;
            WeekEnd = weekEnd;

            Days = new List<DayPlan>();
            Days.Add(new DayPlan());
            Days.Add(new DayPlan() { DayName = "Пн" });
            Days.Add(new DayPlan() { DayName = "Вт" });
            Days.Add(new DayPlan() { DayName = "Ср" });
            Days.Add(new DayPlan() { DayName = "Чт" });
            Days.Add(new DayPlan() { DayName = "Пт" });
            Days.Add(new DayPlan() { DayName = "Сб" });
            Days.Add(new DayPlan() { DayName = "Вс" });

            Days[0].PropertyChanged += WeekSettingsViewModel_PropertyChanged;

            OkCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    IsDialogResultOK = true;
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

            AddMainIngredientCommand = new Lazy<DelegateCommand<DayPlan>>(
                () => new DelegateCommand<DayPlan>(async (day) => {

                    List<TagDTO> mainIngredients;

                    using (var context = new CookingContext())
                    {
                        mainIngredients = context.Tags.Where(x => x.Type == TagType.MainIngredient).Select(x => Mapper.Map<TagDTO>(x)).ToList();
                    }

                    mainIngredients.Insert(0, TagDTO.Any);
                    mainIngredients[0].IsChecked = false;

                    var viewModel = new TagSelectEditViewModel(day.NeededMainIngredients, TagType.MainIngredient, mainIngredients);

                    var dialog = new CustomDialog()
                    {
                        Title = "Категории Main Ingredient",
                        Content = new TagSelectView()
                        {
                            DataContext = viewModel
                        }
                    };
                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
                    await dialog.WaitUntilUnloadedAsync();

                    if (viewModel.DialogResultOk)
                    {
                        var list = viewModel.MainIngredients.Where(x => x.IsChecked).ToList();

                        if(list.Any(x => x != TagDTO.Any))
                        {
                            list.Remove(TagDTO.Any);
                        }

                        day.NeededMainIngredients = new ObservableCollection<TagDTO>(list);
                    }
                }));

            AddDishTypesCommand = new Lazy<DelegateCommand<DayPlan>>(
                () => new DelegateCommand<DayPlan>(async (day) => {

                    List<TagDTO> dishTypes;

                    using (var context = new CookingContext())
                    {
                        dishTypes = context.Tags.Where(x => x.Type == TagType.DishType).Select(x => Mapper.Map<TagDTO>(x)).ToList();
                    }

                    dishTypes.Insert(0, TagDTO.Any);
                    dishTypes[0].IsChecked = false;

                    var viewModel = new TagSelectEditViewModel(day.NeededDishTypes, TagType.DishType, dishTypes);

                    var dialog = new CustomDialog()
                    {
                        Title = "Категории Dish Type",
                        Content = new TagSelectView()
                        {
                            DataContext = viewModel
                        }
                    };
                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
                    await dialog.WaitUntilUnloadedAsync();

                    if (viewModel.DialogResultOk)
                    {
                        var list = viewModel.DishTypes.Where(x => x.IsChecked).ToList();

                        if (list.Any(x => x != TagDTO.Any))
                        {
                            list.Remove(TagDTO.Any);
                        }

                        day.NeededDishTypes = new ObservableCollection<TagDTO>(list);
                    }
                }));

            AddCalorieTypesCommand = new Lazy<DelegateCommand<DayPlan>>(
                () => new DelegateCommand<DayPlan>(async (day) => {

                    var viewModel = new CalorieTypeSelectEditViewModel(day.CalorieTypes);

                    var dialog = new CustomDialog()
                    {
                        Title = "Категории калорийности",
                        Content = new CalorieTypeSelectView()
                        {
                            DataContext = viewModel
                        }
                    };

                    await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
                    await dialog.WaitUntilUnloadedAsync();

                    if (viewModel.DialogResultOk)
                    {
                        var list = viewModel.AllValues.Where(x => x.IsSelected).ToList();

                        if (list.Any(x => x != CalorieTypeSelection.Any))
                        {
                            list.Remove(CalorieTypeSelection.Any);
                        }

                        day.CalorieTypes = new ObservableCollection<CalorieTypeSelection>(list);
                    }
                }));
        }

        private void WeekSettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(DayPlan.CalorieTypes):
                    Days.ForEach(x => x.CalorieTypes = (sender as DayPlan).CalorieTypes);
                    break;
                case nameof(DayPlan.IsSelected):
                    Days.ForEach(x => x.IsSelected = (sender as DayPlan).IsSelected);
                    break;
                case nameof(DayPlan.MaxComplexity):
                    Days.ForEach(x => x.MaxComplexity = (sender as DayPlan).MaxComplexity);
                    break;
                case nameof(DayPlan.MinRating):
                    Days.ForEach(x => x.MinRating = (sender as DayPlan).MinRating);
                    break;
                case nameof(DayPlan.NeededDishTypes):
                    Days.ForEach(x => x.NeededDishTypes = (sender as DayPlan).NeededDishTypes);
                    break;
                case nameof(DayPlan.NeededMainIngredients):
                    Days.ForEach(x => x.NeededMainIngredients = (sender as DayPlan).NeededMainIngredients);
                    break;
                case nameof(DayPlan.OnlyNewRecipies):
                    Days.ForEach(x => x.OnlyNewRecipies = (sender as DayPlan).OnlyNewRecipies);
                    break;
            }
        }

        public bool IsDialogResultOK { get; set; }

        public DateTime WeekStart { get; }
        public DateTime WeekEnd { get; }

        public List<DayPlan> Days { get; }
        
        public Lazy<DelegateCommand<DayPlan>> AddMainIngredientCommand { get; }
        public Lazy<DelegateCommand<DayPlan>> AddDishTypesCommand { get; }
        public Lazy<DelegateCommand<DayPlan>> AddCalorieTypesCommand { get; }
        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }

        public int? MinRatingAll { get; set; }
        public int? MaxComplexityAll { get; set; }
        public bool OnlyNewRecipiesAll { get; set; }
        public bool IsSelectedAll { get; set; } = true;
        public ObservableCollection<TagDTO> NeededMainIngredientsAll { get; set; } = new ObservableCollection<TagDTO>() { TagDTO.Any };
        public ObservableCollection<TagDTO> NeededDishTypesAll { get; set; } = new ObservableCollection<TagDTO>() { TagDTO.Any };
    }
}
