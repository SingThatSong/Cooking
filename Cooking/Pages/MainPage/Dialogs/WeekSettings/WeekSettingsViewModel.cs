using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Recepies;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages.Dialogs
{
    public class WeekSettingsViewModel : OkCancelViewModel
    {
        readonly DialogUtils dialogUtils;

        public DateTime WeekStart { get; }
        public DateTime WeekEnd { get; }

        public List<DayPlan> Days { get; }

        public DelegateCommand<DayPlan> AddMainIngredientCommand { get; }
        public DelegateCommand<DayPlan> AddDishTypesCommand { get; }
        public DelegateCommand<DayPlan> AddCalorieTypesCommand { get; }
        public WeekSettingsViewModel() 
        {
            throw new NotImplementedException();
        }

        public WeekSettingsViewModel(DateTime weekStart, DateTime weekEnd, DialogUtils dialogUtils)
        {
            WeekStart = weekStart;
            WeekEnd = weekEnd;
            this.dialogUtils = dialogUtils;

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
                var tags = await GetTags(TagType.MainIngredient, day.NeededMainIngredients);

                if (tags != null)
                {
                    day.NeededMainIngredients = tags;
                }
            });

            AddDishTypesCommand = new DelegateCommand<DayPlan>(async (day) => {

                var tags = await GetTags(TagType.DishType, day.NeededDishTypes);

                if (tags != null)
                {
                    day.NeededDishTypes = tags;
                }
            });

            AddCalorieTypesCommand = new DelegateCommand<DayPlan>(async (day) => {

                var viewModel = new CalorieTypeSelectEditViewModel(day.CalorieTypes);
                await dialogUtils.ShowCustomMessageAsync<CalorieTypeSelectView, CalorieTypeSelectEditViewModel>("Категории калорийности", viewModel);

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
        }

        private async Task<ObservableCollection<TagDTO>> GetTags(TagType type, ObservableCollection<TagDTO> current)
        {
            var dbTags = TagService.GetTagsByType(type);

            var allTags = dbTags.Select(x => MapperService.Mapper.Map<TagDTO>(x)).ToList();

            allTags.Insert(0, TagDTO.Any);
            allTags[0].IsChecked = false;
            allTags.ForEach(x => x.Type = type);

            var viewModel = new TagSelectEditViewModel(current, allTags, dialogUtils);
            await dialogUtils.ShowCustomMessageAsync<TagSelectView, TagSelectEditViewModel>($"Категории {type}", viewModel);

            if (viewModel.DialogResultOk)
            {
                var list = viewModel.AllTags.Where(x => x.IsChecked).ToList();

                if (list.Any(x => x != TagDTO.Any))
                {
                    list.Remove(TagDTO.Any);
                }
                else if (!list.Any())
                {
                    list.Add(TagDTO.Any);
                }

                return new ObservableCollection<TagDTO>(list);
            }
            else
            {
                return new ObservableCollection<TagDTO>();
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
    }
}
