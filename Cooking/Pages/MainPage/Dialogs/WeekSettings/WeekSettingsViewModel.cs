using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Dialogs;
using Cooking.Pages.ViewModel;
using Data.Model;
using Prism.Regions;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages
{
    public class WeekSettingsViewModel : INavigationAware
    {
        private readonly DialogUtils dialogUtils;

        public List<DayPlan> Days { get; }

        public DelegateCommand<DayPlan> AddMainIngredientCommand { get; }
        public DelegateCommand<DayPlan> AddDishTypesCommand { get; }
        public DelegateCommand<DayPlan> AddCalorieTypesCommand { get; }

        public WeekSettingsViewModel(DialogUtils dialogUtils)
        {
            Days = new List<DayPlan>()
            {
                new DayPlan(),
                new DayPlan { DayName = "Пн", DayOfWeek = DayOfWeek.Monday },
                new DayPlan { DayName = "Вт", DayOfWeek = DayOfWeek.Tuesday },
                new DayPlan { DayName = "Ср", DayOfWeek = DayOfWeek.Wednesday },
                new DayPlan { DayName = "Чт", DayOfWeek = DayOfWeek.Thursday },
                new DayPlan { DayName = "Пт", DayOfWeek = DayOfWeek.Friday },
                new DayPlan { DayName = "Сб", DayOfWeek = DayOfWeek.Saturday },
                new DayPlan { DayName = "Вс", DayOfWeek = DayOfWeek.Sunday }
            };

            Days[0].PropertyChanged += OnHeaderValueChanged;
            
            AddMainIngredientCommand = new DelegateCommand<DayPlan>(async (day) => {
                var tags = await GetTags(TagType.MainIngredient, day.NeededMainIngredients).ConfigureAwait(false);

                if (tags != null)
                {
                    day.NeededMainIngredients = tags;
                }
            });

            AddDishTypesCommand = new DelegateCommand<DayPlan>(async (day) => {

                var tags = await GetTags(TagType.DishType, day.NeededDishTypes).ConfigureAwait(false);

                if (tags != null)
                {
                    day.NeededDishTypes = tags;
                }
            });

            AddCalorieTypesCommand = new DelegateCommand<DayPlan>(async (day) => {

                var viewModel = new CalorieTypeSelectEditViewModel(day.CalorieTypes);
                await dialogUtils.ShowCustomMessageAsync<CalorieTypeSelectView, CalorieTypeSelectEditViewModel>("Категории калорийности", viewModel).ConfigureAwait(false);

                if (viewModel.DialogResultOk)
                {
                    var list = viewModel.AllValues.Where(x => x.IsSelected).ToList();

                    if (list.Any(x => x != CalorieTypeSelection.Any))
                    {
                        list.Remove(CalorieTypeSelection.Any);
                    }
                    else if (!list.Any())
                    {
                        list.Add(CalorieTypeSelection.Any);
                    }

                    day.CalorieTypes = new ObservableCollection<CalorieTypeSelection>(list);
                }
            });
            this.dialogUtils = dialogUtils;
        }

        private async Task<ObservableCollection<TagEdit>> GetTags(TagType type, ObservableCollection<TagEdit> current)
        {
            var dbTags = TagService.GetTagsByType(type);

            var allTags = dbTags.Select(x => MapperService.Mapper.Map<TagEdit>(x)).ToList();

            allTags.Insert(0, TagEdit.Any);
            allTags[0].IsChecked = false;
            allTags.ForEach(x => x.Type = type);

            var viewModel = new TagSelectViewModel(current, allTags, dialogUtils);
            await dialogUtils.ShowCustomMessageAsync<TagSelect, TagSelectViewModel>($"Категории {type}", viewModel).ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                var list = viewModel.AllTags.Where(x => x.IsChecked).ToList();

                if (list.Any(x => x != TagEdit.Any))
                {
                    list.Remove(TagEdit.Any);
                }
                else if (!list.Any())
                {
                    list.Add(TagEdit.Any);
                }

                return new ObservableCollection<TagEdit>(list);
            }
            else
            {
                return new ObservableCollection<TagEdit>();
            }
        }

        private void OnHeaderValueChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is DayPlan dayPlan)
            {
                switch (e.PropertyName)
                {
                    case nameof(DayPlan.CalorieTypes):
                        Days.ForEach(x => x.CalorieTypes = dayPlan.CalorieTypes);
                        break;
                    case nameof(DayPlan.IsSelected):
                        Days.ForEach(x => x.IsSelected = dayPlan.IsSelected);
                        break;
                    case nameof(DayPlan.MaxComplexity):
                        Days.ForEach(x => x.MaxComplexity = dayPlan.MaxComplexity);
                        break;
                    case nameof(DayPlan.MinRating):
                        Days.ForEach(x => x.MinRating = dayPlan.MinRating);
                        break;
                    case nameof(DayPlan.NeededDishTypes):
                        Days.ForEach(x => x.NeededDishTypes = dayPlan.NeededDishTypes);
                        break;
                    case nameof(DayPlan.NeededMainIngredients):
                        Days.ForEach(x => x.NeededMainIngredients = dayPlan.NeededMainIngredients);
                        break;
                    case nameof(DayPlan.OnlyNewRecipies):
                        Days.ForEach(x => x.OnlyNewRecipies = dayPlan.OnlyNewRecipies);
                        break;
                }
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}
