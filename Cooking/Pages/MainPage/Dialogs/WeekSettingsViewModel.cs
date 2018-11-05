using AutoMapper;
using Cooking.DTO;
using Cooking.Pages.MainPage.Dialogs.Model;
using Cooking.Pages.MainPage.Dialogs.Model.CalorieTypeSelect;
using Cooking.Pages.Recepies;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
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
            Days.Add(new DayPlan() { DayName = "Пн" });
            Days.Add(new DayPlan() { DayName = "Вт" });
            Days.Add(new DayPlan() { DayName = "Ср" });
            Days.Add(new DayPlan() { DayName = "Чт" });
            Days.Add(new DayPlan() { DayName = "Пт" });
            Days.Add(new DayPlan() { DayName = "Сб" });
            Days.Add(new DayPlan() { DayName = "Вс" });

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

                    var viewModel = new TagSelectEditViewModel(day.NeededMainIngredients, mainIngredients);

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
                        var list = viewModel.AllTags.Where(x => x.IsChecked).ToList();

                        if(list.Any(x => x != TagDTO.Any))
                        {
                            list.Remove(TagDTO.Any);
                        }

                        day.NeededMainIngredients = new ObservableCollection<TagDTO>(list);
                    }
                }));

            AddDishTypesCommand = new Lazy<DelegateCommand<DayPlan>>(
                () => new DelegateCommand<DayPlan>(async (day) => {

                    List<TagDTO> mainIngredients;

                    using (var context = new CookingContext())
                    {
                        mainIngredients = context.Tags.Where(x => x.Type == TagType.DishType).Select(x => Mapper.Map<TagDTO>(x)).ToList();
                    }

                    mainIngredients.Insert(0, TagDTO.Any);
                    mainIngredients[0].IsChecked = false;

                    var viewModel = new TagSelectEditViewModel(day.NeededDishTypes, mainIngredients);

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
                        var list = viewModel.AllTags.Where(x => x.IsChecked).ToList();

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

        public bool IsDialogResultOK { get; set; }

        public DateTime WeekStart { get; }
        public DateTime WeekEnd { get; }

        public List<DayPlan> Days { get; }
        
        public Lazy<DelegateCommand<DayPlan>> AddMainIngredientCommand { get; }
        public Lazy<DelegateCommand<DayPlan>> AddDishTypesCommand { get; }
        public Lazy<DelegateCommand<DayPlan>> AddCalorieTypesCommand { get; }
        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }
    }
}
