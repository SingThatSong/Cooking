using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Ingredients;
using Cooking.WPF.Helpers;
using Cooking.Data.Model;
using Prism.Regions;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public partial class IngredientsViewModel
    {
        // Dependencies
        private readonly IRegionManager regionManager;
        private readonly DialogService dialogUtils;
        private readonly IngredientService ingredientService;
        private readonly IMapper mapper;
        private readonly ILocalization localization;

        // State
        public ObservableCollection<IngredientEdit>? Ingredients { get; private set; }
        public bool IsEditing { get; set; }

        // Commands
        public AsyncDelegateCommand LoadedCommand { get; }
        public DelegateCommand AddIngredientCommand { get; }
        public DelegateCommand<Guid> DeleteCategoryCommand { get; }
        public DelegateCommand<IngredientEdit> ViewIngredientCommand { get; }
        public DelegateCommand<IngredientEdit> EditIngredientCommand { get; }

        public IngredientsViewModel(IRegionManager regionManager, 
                                    DialogService dialogUtils, 
                                    IngredientService ingredientService,
                                    IMapper mapper,
                                    ILocalization localization)
        {
            Debug.Assert(regionManager != null);
            Debug.Assert(dialogUtils != null);
            Debug.Assert(ingredientService != null);
            Debug.Assert(mapper != null);
            Debug.Assert(localization != null);

            this.regionManager     = regionManager;
            this.dialogUtils       = dialogUtils;
            this.ingredientService = ingredientService;
            this.mapper            = mapper;
            this.localization      = localization;
            LoadedCommand          = new AsyncDelegateCommand(OnLoaded, executeOnce: true);
            AddIngredientCommand   = new DelegateCommand(AddIngredient);
            DeleteCategoryCommand  = new DelegateCommand<Guid>(DeleteIngredient);
            EditIngredientCommand  = new DelegateCommand<IngredientEdit>(EditIngredient);
            ViewIngredientCommand  = new DelegateCommand<IngredientEdit>(ViewIngredient);
        }

        #region Top-level functions
        private Task OnLoaded()
        {
            Debug.WriteLine("IngredientsViewModel.OnLoaded");
            List<IngredientEdit> dataDb = ingredientService.GetProjected<IngredientEdit>(mapper);
            Ingredients = new ObservableCollection<IngredientEdit>(dataDb);

            return Task.CompletedTask;
        }

        private void ViewIngredient(IngredientEdit ingredient)
        {
            var parameters = new NavigationParameters()
            {
                { nameof(RecepiesViewModel.FilterText), $"{Consts.IngredientSymbol}\"{ingredient.Name}\"" }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(Recepies), parameters);
        }

        public async void DeleteIngredient(Guid recipeId) => await dialogUtils.ShowYesNoDialog(localization.GetLocalizedString("SureDelete"), 
                                                                                               localization.GetLocalizedString("CannotUndo"), 
                                                                                               successCallback: () => OnIngredientDeleted(recipeId))
                                                                               .ConfigureAwait(false);

        public async void AddIngredient() => await dialogUtils.ShowOkCancelDialog<IngredientEditView, IngredientEditViewModel>(localization.GetLocalizedString("NewIngredient"), 
                                                                                                                               successCallback: OnNewIngredientCreated)
                                                              .ConfigureAwait(false);

        public async void EditIngredient(IngredientEdit ingredient)
        {
            var viewModel = new IngredientEditViewModel(ingredientService, dialogUtils, localization, mapper.Map<IngredientEdit>(ingredient));
            await dialogUtils.ShowOkCancelDialog<IngredientEditView, IngredientEditViewModel>(localization.GetLocalizedString("EditIngredient"), viewModel, successCallback: OnIngredientEdited)
                             .ConfigureAwait(false);
        }
        #endregion

        #region Callbacks
        private async void OnIngredientDeleted(Guid id)
        {
            await ingredientService.DeleteAsync(id).ConfigureAwait(false);
            Ingredients!.Remove(Ingredients.Single(x => x.ID == id));
        }

        private async void OnIngredientEdited(IngredientEditViewModel viewModel)
        {
            await ingredientService.UpdateAsync(mapper.Map<Ingredient>(viewModel.Ingredient)).ConfigureAwait(false);
            IngredientEdit existing = Ingredients.Single(x => x.ID == viewModel.Ingredient.ID);
            mapper.Map(viewModel.Ingredient, existing);
        }

        private async void OnNewIngredientCreated(IngredientEditViewModel viewModel)
        {
            await ingredientService.CreateAsync(mapper.Map<Ingredient>(viewModel.Ingredient)).ConfigureAwait(false);
            Ingredients!.Add(viewModel.Ingredient);
        }
        #endregion
    }
}