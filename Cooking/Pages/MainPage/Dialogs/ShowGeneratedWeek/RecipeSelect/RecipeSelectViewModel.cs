using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Cooking.Pages.Recepies
{
    public partial class RecipeSelectViewModel : INotifyPropertyChanged
    {
        public RecipeSelectViewModel()
        {
            Recipies = new Lazy<ObservableCollection<RecipeDTO>>(GetRecipies);
            RecipiesSource = new CollectionViewSource() { Source = Recipies.Value };
            RecipiesSource.Filter += RecipiesSource_Filter;
            OkCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(() => {
                    DialogResultOk = true;
                    CloseCommand.Value.Execute();
                }));

            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));
            SelectRecipeCommand = new Lazy<DelegateCommand<RecipeDTO>>(
                () => new DelegateCommand<RecipeDTO>(recipe => {
                    foreach(var r in Recipies.Value.Where(x => x.IsSelected))
                    {
                        r.IsSelected = false;
                    }

                    recipe.IsSelected = true;
                }));

            ViewRecipeCommand = new Lazy<DelegateCommand<RecipeDTO>>(() => new DelegateCommand<RecipeDTO>(ViewRecipe));
        }

        private void RecipiesSource_Filter(object sender, FilterEventArgs e)
        {
            if (RecipeFilter == null)
                return;

            if(e.Item is RecipeDTO recipe)
            {
                e.Accepted = RecipeFilter.FilterRecipe(recipe);
            }
        }

        private string filterText;
        private RecipeFilter RecipeFilter { get; set; }
        public string FilterText
        {
            get => filterText;
            set
            {
                if(filterText != value)
                {
                    filterText = value;
                    RecipeFilter = new RecipeFilter(value);
                    RecipiesSource.View.Refresh();
                }
            }
        }
        
        public async void ViewRecipe(RecipeDTO recipe)
        {
            using (var context = new CookingContext())
            {
                var existing = context.Recipies
                                      .Where(x => x.ID == recipe.ID)
                                      .Include(x => x.IngredientGroups)
                                          .ThenInclude(x => x.Ingredients)
                                              .ThenInclude(x => x.Ingredient)
                                      .Include(x => x.Ingredients)
                                          .ThenInclude(x => x.Ingredient)
                                      .Include(x => x.Tags)
                                          .ThenInclude(x => x.Tag)
                                      .Single();

                Mapper.Map(existing, recipe);
            }

            var viewModel = new RecipeViewModel(recipe);

            var dialog = new CustomDialog()
            {
                Content = new RecipeView()
                {
                    DataContext = viewModel
                }
            };

            await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
            await dialog.WaitUntilUnloadedAsync();
        }

        public CollectionViewSource RecipiesSource { get; }

        public Lazy<ObservableCollection<RecipeDTO>> Recipies { get; }
        private ObservableCollection<RecipeDTO> GetRecipies()
        {
            try
            {
                using (var Context = new CookingContext())
                {
                    var originalList = Context.Recipies
                                              .Include(x => x.Ingredients)
                                                  .ThenInclude(x => x.Ingredient)
                                              .Include(x => x.Tags)
                                                  .ThenInclude(x => x.Tag)
                                              .ToList();

                    return new ObservableCollection<RecipeDTO>(
                        originalList.OrderBy(x => x.Name).Select(x =>
                        {
                            var dto = Mapper.Map<RecipeDTO>(x);
                            return dto;
                        })
                    );
                }
            }
            catch
            {
                return new ObservableCollection<RecipeDTO>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public Lazy<DelegateCommand<RecipeDTO>> SelectRecipeCommand { get; }
        public Lazy<DelegateCommand<RecipeDTO>> ViewRecipeCommand { get; }
        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }
        public bool DialogResultOk { get; private set; }
    }
}