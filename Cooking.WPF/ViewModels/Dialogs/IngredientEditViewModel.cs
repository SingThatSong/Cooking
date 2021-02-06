using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;
using Cooking.WPF.Events;
using Cooking.WPF.Services;
using Cooking.WPF.Validation;
using Prism.Events;
using Validar;
using WPF.Commands;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for ingredient edit dialog.
    /// </summary>
    [InjectValidation]
    public partial class IngredientEditViewModel : OkCancelViewModel
    {
        private readonly CRUDService<Ingredient> ingredientService;
        private readonly ILocalization localization;
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientEditViewModel"/> class.
        /// </summary>
        /// <param name="ingredientService">Ingredient service dependency.</param>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="localization">Localization service dependency.</param>
        /// <param name="eventAggregator">Prism event aggregator.</param>
        /// <param name="ingredient">Ingredient to edit. Null means new ingredient.</param>
        public IngredientEditViewModel(CRUDService<Ingredient> ingredientService,
                                       DialogService dialogService,
                                       ILocalization localization,
                                       IEventAggregator eventAggregator,
                                       IngredientEdit? ingredient = null)
            : base(dialogService)
        {
            this.ingredientService = ingredientService;
            this.localization = localization;
            this.eventAggregator = eventAggregator;
            Ingredient = ingredient ?? new IngredientEdit();

            AllIngredientNames = ingredientService.GetProperty(x => x.Name, filter: x => x.Name != null)
                                                  .ConvertAll(x => x!);

            LoadedCommand = new DelegateCommand(OnLoaded);
            DeleteIngredientCommand = new DelegateCommand<Guid>(DeleteIngredientAsync);
            IngredientTypes = Enum.GetValues(typeof(IngredientType)).Cast<IngredientType>().ToList();
        }

        /// <summary>
        /// Gets or sets ingreditne to edit.
        /// </summary>
        public IngredientEdit Ingredient { get; set; }

        /// <summary>
        /// Gets similar ingredients list to check on duplicates.
        /// </summary>
        public IEnumerable<string>? SimilarIngredients => string.IsNullOrWhiteSpace(Ingredient?.Name)
                                                        ? null
                                                        : AllIngredientNames.OrderBy(x => IngredientCompare(x, Ingredient.Name)).Take(Consts.HowManyAlternativesToShow);

        /// <summary>
        /// Gets all types of ingredients to select from.
        /// </summary>
        public List<IngredientType> IngredientTypes { get; }

        /// <summary>
        /// Gets command to execute on loaded event.
        /// </summary>
        public DelegateCommand LoadedCommand { get; }

        /// <summary>
        /// Gets comand to delete ingredient.
        /// </summary>
        public DelegateCommand<Guid> DeleteIngredientCommand { get; }

        private bool NameChanged { get; set; }
        private List<string> AllIngredientNames { get; set; }

        /// <inheritdoc/>
        protected override async Task OkAsync()
        {
            if (NameChanged
             && Ingredient.Name != null
             && AllIngredientNames.Any(x => string.Equals(x, Ingredient.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                bool saveAnyway = false;
                await DialogService.ShowLocalizedYesNoDialogAsync("IngredientAlreadyExists",
                                                                  "SaveAnyway",
                                                                  successCallback: () => saveAnyway = true);

                if (!saveAnyway)
                {
                    return;
                }
            }

            await base.OkAsync();
        }

        private int IngredientCompare(string str1, string str2)
             => StringCompare.LevensteinDistance(
                        string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                        string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
                );

        private void OnLoaded()
        {
            Ingredient.PropertyChanged += (src, e) =>
            {
                if (e.PropertyName == nameof(Ingredient.Name))
                {
                    OnPropertyChanged(nameof(SimilarIngredients));
                    NameChanged = true;
                }
            };
        }

        private async void DeleteIngredientAsync(Guid ingredientID) => await DialogService.ShowYesNoDialogAsync(localization.GetLocalizedString("SureDelete", Ingredient.Name),
                                                                                                                localization["CannotUndo"],
                                                                                                                successCallback: () => OnIngredientDeletedAsync(ingredientID));

        private async void OnIngredientDeletedAsync(Guid id)
        {
            await ingredientService.DeleteAsync(id);
            eventAggregator.GetEvent<IngredientDeletedEvent>().Publish(id);
        }
    }
}