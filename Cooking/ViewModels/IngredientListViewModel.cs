﻿using AutoMapper;
using Cooking.Data.Model;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Helpers;
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
    [AddINotifyPropertyChangedInterface]
    public partial class IngredientListViewModel
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
        public DelegateCommand<Guid> DeleteIngredientCommand { get; }
        public DelegateCommand<IngredientEdit> ViewIngredientCommand { get; }
        public DelegateCommand<IngredientEdit> EditIngredientCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientListViewModel"/> class.
        /// </summary>
        /// <param name="regionManager"></param>
        /// <param name="dialogUtils"></param>
        /// <param name="ingredientService"></param>
        /// <param name="mapper"></param>
        /// <param name="localization"></param>
        public IngredientListViewModel(IRegionManager regionManager,
                                    DialogService dialogUtils,
                                    IngredientService ingredientService,
                                    IMapper mapper,
                                    ILocalization localization)
        {
            this.regionManager = regionManager;
            this.dialogUtils = dialogUtils;
            this.ingredientService = ingredientService;
            this.mapper = mapper;
            this.localization = localization;
            LoadedCommand = new AsyncDelegateCommand(OnLoaded, executeOnce: true);
            AddIngredientCommand = new DelegateCommand(AddIngredient);
            DeleteIngredientCommand = new DelegateCommand<Guid>(DeleteIngredient);
            EditIngredientCommand = new DelegateCommand<IngredientEdit>(EditIngredient);
            ViewIngredientCommand = new DelegateCommand<IngredientEdit>(ViewIngredient);
        }

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

        private async void DeleteIngredient(Guid ingredientId) => await dialogUtils.ShowYesNoDialog(localization.GetLocalizedString("SureDelete", Ingredients!.Single(x => x.ID == ingredientId).Name ?? string.Empty),
                                                                                                   localization.GetLocalizedString("CannotUndo"),
                                                                                                   successCallback: () => OnIngredientDeleted(ingredientId))
                                                                               ;

        private async void AddIngredient() => await dialogUtils.ShowOkCancelDialog<IngredientEditView, IngredientEditViewModel>(localization.GetLocalizedString("NewIngredient"),
                                                                                                                               successCallback: OnNewIngredientCreated)
                                                              ;

        private async void EditIngredient(IngredientEdit ingredient)
        {
            var viewModel = new IngredientEditViewModel(ingredientService, dialogUtils, localization, mapper.Map<IngredientEdit>(ingredient));
            await dialogUtils.ShowOkCancelDialog<IngredientEditView, IngredientEditViewModel>(localization.GetLocalizedString("EditIngredient"), viewModel, successCallback: OnIngredientEdited)
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