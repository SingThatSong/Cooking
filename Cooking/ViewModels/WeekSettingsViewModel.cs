using AutoMapper;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Views;
using Cooking.WPF.ViewModels;
using Cooking.ServiceLayer;
using Cooking.WPF.Helpers;
using Cooking.Data.Model;
using Prism.Ioc;
using Prism.Regions;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFLocalizeExtension.Engine;

namespace Cooking.WPF.Views
{
    [AddINotifyPropertyChangedInterface]
    public class WeekSettingsViewModel : INavigationAware
    {
        private readonly DialogService dialogUtils;
        private readonly IRegionManager regionManager;
        private readonly TagService tagService;
        private readonly IContainerExtension container;
        private readonly RecipeService recipeService;
        private readonly ILocalization localization;
        private NavigationContext? navigationContext;

        public DateTime WeekStart { get; private set; }
        public List<DayPlan> Days { get; }

        public DelegateCommand<DayPlan> AddMainIngredientCommand { get; }
        public DelegateCommand<DayPlan> AddDishTypesCommand { get; }
        public DelegateCommand<DayPlan> AddCalorieTypesCommand { get; }
        public DelegateCommand OkCommand { get; }
        public DelegateCommand CloseCommand { get; }

        public WeekSettingsViewModel(DialogService dialogUtils,
                                     IRegionManager regionManager,
                                     TagService tagService,
                                     IContainerExtension container,
                                     RecipeService recipeService,
                                     ILocalization localization)
        {
            this.dialogUtils = dialogUtils;
            this.regionManager = regionManager;
            this.tagService = tagService;
            this.container = container;
            this.recipeService = recipeService;
            this.localization = localization;
            Days = new List<DayPlan>()
            {
                new DayPlan(),
                new DayPlan { DayName = localization.GetLocalizedString("Monday_Short"), DayOfWeek = DayOfWeek.Monday },
                new DayPlan { DayName = localization.GetLocalizedString("Tuesday_Short"), DayOfWeek = DayOfWeek.Tuesday },
                new DayPlan { DayName = localization.GetLocalizedString("Wednesday_Short"), DayOfWeek = DayOfWeek.Wednesday },
                new DayPlan { DayName = localization.GetLocalizedString("Thursday_Short"), DayOfWeek = DayOfWeek.Thursday },
                new DayPlan { DayName = localization.GetLocalizedString("Friday_Short"), DayOfWeek = DayOfWeek.Friday },
                new DayPlan { DayName = localization.GetLocalizedString("Saturday_Short"), DayOfWeek = DayOfWeek.Saturday },
                new DayPlan { DayName = localization.GetLocalizedString("Sunday_Short"), DayOfWeek = DayOfWeek.Sunday }
            };

            Days[0].PropertyChanged += OnHeaderValueChanged;
            AddMainIngredientCommand = new DelegateCommand<DayPlan>(AddMainIngredient);
            AddDishTypesCommand = new DelegateCommand<DayPlan>(AddDishTypes);
            AddCalorieTypesCommand = new DelegateCommand<DayPlan>(AddCalorieTypes);
            CloseCommand = new DelegateCommand(Close, CanClose);
            OkCommand = new DelegateCommand(Ok);
        }

        private void Ok()
        {
            IEnumerable<DayPlan> selectedDays = Days.Skip(1).Where(x => x.IsSelected);
            GenerateRecipies(selectedDays);

            var parameters = new NavigationParameters
            {
                { nameof(GeneratedWeekViewModel.Days), selectedDays },
                { nameof(GeneratedWeekViewModel.WeekStart), WeekStart }
            };

            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(GeneratedWeekView), parameters);
        }

        private void GenerateRecipies(IEnumerable<DayPlan> selectedDays)
        {
            Debug.WriteLine("MainPageViewModel.GenerateRecipies");
            foreach (DayPlan day in selectedDays)
            {
                var requiredTags = new List<Guid>();

                if (!day.NeededDishTypes.Contains(TagEdit.Any))
                {
                    requiredTags.AddRange(day.NeededDishTypes.Select(x => x.ID));
                }

                if (!day.NeededMainIngredients.Contains(TagEdit.Any))
                {
                    requiredTags.AddRange(day.NeededMainIngredients.Select(x => x.ID));
                }

                var requiredCalorieTyoes = new List<CalorieType>();
                if (!day.CalorieTypes.Contains(CalorieTypeSelection.Any))
                {
                    requiredCalorieTyoes.AddRange(day.CalorieTypes.Select(x => x.CalorieType));
                }

                day.RecipeAlternatives = recipeService.GetRecipiesByParameters(requiredTags, requiredCalorieTyoes, day.MaxComplexity, day.MinRating, day.OnlyNewRecipies);

                IEnumerable<ServiceLayer.Projections.RecipeSlim?> selectedRecipies = selectedDays.Where(x => x.Recipe != null).Select(x => x.Recipe);
                var recipiesNotSelectedYet = day.RecipeAlternatives.Where(x => !selectedRecipies.Any(selected => selected!.ID == x.ID)).ToList();

                if (recipiesNotSelectedYet.Count > 0)
                {
                    day.Recipe = recipiesNotSelectedYet.OrderByDescending(x => recipeService.DaysFromLasCook(x.ID)).First();
                }
                else
                {
                    day.Recipe = null;
                }
            }
        }

        private bool CanClose() => navigationContext?.NavigationService.Journal.CanGoBack ?? false;
        private void Close() => navigationContext!.NavigationService.Journal.GoBack();

        private async void AddCalorieTypes(DayPlan day)
        {
            var viewModel = new CalorieTypeSelectViewModel(dialogUtils, day.CalorieTypes);
            await dialogUtils.ShowCustomMessageAsync<CalorieTypeSelectView, CalorieTypeSelectViewModel>(localization.GetLocalizedString("CalorieTyoes"), viewModel);

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
        }

        private async void AddDishTypes(DayPlan day)
        {
            ObservableCollection<TagEdit>? tags = await GetTags(TagType.DishType, day.NeededDishTypes);

            if (tags != null)
            {
                day.NeededDishTypes = tags;
            }
        }

        private async void AddMainIngredient(DayPlan day)
        {
            ObservableCollection<TagEdit>? tags = await GetTags(TagType.MainIngredient, day.NeededMainIngredients);

            if (tags != null)
            {
                day.NeededMainIngredients = tags;
            }
        }

        private async Task<ObservableCollection<TagEdit>?> GetTags(TagType type, ObservableCollection<TagEdit> current)
        {
            List<TagEdit> allTags = tagService.GetTagsByType<TagEdit>(type, container.Resolve<IMapper>());

            allTags.Insert(0, TagEdit.Any);
            allTags[0].IsChecked = false;
            allTags.ForEach(x => x.Type = type);

            TagSelectViewModel viewModel = container.Resolve<TagSelectViewModel>();
            viewModel.SetTags(current, allTags);
            await dialogUtils.ShowCustomMessageAsync<TagSelectView, TagSelectViewModel>(string.Format(localization.CurrentCulture, localization.GetLocalizedString("CategoriesOf") ?? "{0}", type), viewModel);

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
                return null;
            }
        }

        [SuppressPropertyChangedWarnings]
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
            this.navigationContext = navigationContext;
            WeekStart = (DateTime)navigationContext.Parameters[nameof(WeekStart)];
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            bool returned = navigationContext.NavigationService.Journal.CurrentEntry.Uri.OriginalString == "ShowGeneratedWeekView";

            if (returned)
            {
                // if returned from ShowGeneratedWeekView - return old view, othrewise return new
                return true;
            }
            else
            {
                // We started new week creation - cached view should be deleted from region
                UserControl view = navigationContext.NavigationService.Region.Views.Cast<UserControl>().FirstOrDefault(x => x.DataContext == this);
                navigationContext.NavigationService.Region.Remove(view);
                return false;
            }
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}
