using AutoMapper;
using Cooking.Data.Model;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Prism.Regions;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// View model for ingredients list.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class IngredientListViewModel
    {
        // Dependencies
        private readonly IRegionManager regionManager;
        private readonly DialogService dialogService;
        private readonly IngredientService ingredientService;
        private readonly IMapper mapper;
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientListViewModel"/> class.
        /// </summary>
        /// <param name="regionManager">Region manager for Prism navigation.</param>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="ingredientService">Ingredient service dependency.</param>
        /// <param name="mapper">Mapper dependency.</param>
        /// <param name="localization">Localization provider dependency.</param>
        public IngredientListViewModel(IRegionManager regionManager,
                                       DialogService dialogService,
                                       IngredientService ingredientService,
                                       IMapper mapper,
                                       ILocalization localization)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            this.ingredientService = ingredientService;
            this.mapper = mapper;
            this.localization = localization;
            LoadedCommand = new AsyncDelegateCommand(OnLoaded, executeOnce: true);
            AddIngredientCommand = new DelegateCommand(AddIngredient);
            DeleteIngredientCommand = new DelegateCommand<Guid>(DeleteIngredient);
            EditIngredientCommand = new DelegateCommand<IngredientEdit>(EditIngredient);
            ViewIngredientCommand = new DelegateCommand<IngredientEdit>(ViewIngredient);
        }

        public ObservableCollection<IngredientEdit>? Ingredients { get; private set; }
        public bool IsEditing { get; set; }

        /// <summary>
        /// Gets command to execute on loaded event.
        /// </summary>
        public AsyncDelegateCommand LoadedCommand { get; }
        public DelegateCommand AddIngredientCommand { get; }
        public DelegateCommand<Guid> DeleteIngredientCommand { get; }
        public DelegateCommand<IngredientEdit> ViewIngredientCommand { get; }
        public DelegateCommand<IngredientEdit> EditIngredientCommand { get; }

        private Task OnLoaded()
        {
            Debug.WriteLine("IngredientsViewModel.OnLoaded");
            List<IngredientEdit> dataDb = ingredientService.GetAllProjected<IngredientEdit>(mapper);
            Ingredients = new ObservableCollection<IngredientEdit>(dataDb);

            return Task.CompletedTask;
        }

        private void ViewIngredient(IngredientEdit ingredient)
        {
            var parameters = new NavigationParameters()
            {
                { nameof(RecipeListViewModel.FilterText), $"{Consts.IngredientSymbol}\"{ingredient.Name}\"" }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(RecipeListView), parameters);
        }

        private async void DeleteIngredient(Guid ingredientId) => await dialogService.ShowYesNoDialog(localization.GetLocalizedString("SureDelete", Ingredients!.Single(x => x.ID == ingredientId).Name ?? string.Empty),
                                                                                                   localization.GetLocalizedString("CannotUndo"),
                                                                                                   successCallback: () => OnIngredientDeleted(ingredientId))
                                                                               ;

        private async void AddIngredient() => await dialogService.ShowOkCancelDialog<IngredientEditView, IngredientEditViewModel>(localization.GetLocalizedString("NewIngredient"),
                                                                                                                               successCallback: OnNewIngredientCreated)
                                                              ;

        private async void EditIngredient(IngredientEdit ingredient)
        {
            var viewModel = new IngredientEditViewModel(ingredientService, dialogService, localization, mapper.Map<IngredientEdit>(ingredient));
            await dialogService.ShowOkCancelDialog<IngredientEditView, IngredientEditViewModel>(localization.GetLocalizedString("EditIngredient"), viewModel, successCallback: OnIngredientEdited)
                             ;
        }

        private async void OnIngredientDeleted(Guid id)
        {
            await ingredientService.DeleteAsync(id);
            Ingredients!.Remove(Ingredients.Single(x => x.ID == id));
        }

        private async void OnIngredientEdited(IngredientEditViewModel viewModel)
        {
            await ingredientService.UpdateAsync(mapper.Map<Ingredient>(viewModel.Ingredient));
            IngredientEdit existing = Ingredients.Single(x => x.ID == viewModel.Ingredient.ID);
            mapper.Map(viewModel.Ingredient, existing);
        }

        private async void OnNewIngredientCreated(IngredientEditViewModel viewModel)
        {
            await ingredientService.CreateAsync(mapper.Map<Ingredient>(viewModel.Ingredient));
            Ingredients!.Add(viewModel.Ingredient);
        }
    }
}