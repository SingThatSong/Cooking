using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Events;
using Cooking.WPF.Services;
using Cooking.WPF.Validation;
using Prism.Events;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for ingredient edit dialog.
    /// </summary>
    public partial class IngredientEditViewModel : OkCancelViewModel
    {
        private readonly IngredientService ingredientService;
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
        public IngredientEditViewModel(IngredientService ingredientService,
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
            AllIngredientNames = ingredientService.GetNames();
            LoadedCommand = new DelegateCommand(OnLoaded);
            DeleteIngredientCommand = new DelegateCommand<Guid>(DeleteIngredient);
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

        /// <summary>
        /// Gets localized name caption.
        /// </summary>
        public string? NameCaption => localization.GetLocalizedString("Name");

        /// <summary>
        /// Gets localized type caption.
        /// </summary>
        public string? TypeCaption => localization.GetLocalizedString("Type");

        /// <summary>
        /// Gets localized suggestion caption.
        /// </summary>
        public string? MaybeYouWantCaption => localization.GetLocalizedString("MaybeYouWant");

        private bool NameChanged { get; set; }
        private List<string> AllIngredientNames { get; set; }

        /// <inheritdoc/>
        protected override async Task Ok()
        {
            if (NameChanged
             && Ingredient.Name != null
             && AllIngredientNames.Any(x => string.Equals(x, Ingredient.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                bool saveAnyway = false;
                await DialogService.ShowYesNoDialog(localization.GetLocalizedString("IngredientAlreadyExists"),
                                                    localization.GetLocalizedString("SaveAnyway"),
                                                    successCallback: () => saveAnyway = true);

                if (!saveAnyway)
                {
                    return;
                }
            }

            await base.Ok();
        }

        /// <inheritdoc/>
        protected override bool CanOk() => Ingredient.IsValid();

        private int IngredientCompare(string str1, string str2)
             => StringCompare.LevensteinDistance(
                        string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                        string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
                );

        // WARNING: this is a crunch
        // When we open ingredient creation dialog second+ time, validation cannot see Ingredient being a required property, but when we change it's value - everything is ok
        // There is no such behaviour when using navigation, so it seems it's something Mahapps-related
        // SimpleChildWindow does not help
        private void OnLoaded()
        {
            string? nameBackup = Ingredient.Name;
            IngredientType? typeBackup = Ingredient.Type;

            Ingredient.Name = "123";
            Ingredient.Name = nameBackup;
            Ingredient.Type = (IngredientType)999;

            Ingredient.Type = typeBackup;
            Ingredient.PropertyChanged += (src, e) =>
            {
                if (e.PropertyName == nameof(Ingredient.Name))
                {
                    OnPropertyChanged(nameof(SimilarIngredients));
                    NameChanged = true;
                }
            };
        }

        private async void DeleteIngredient(Guid ingredientID) => await DialogService.ShowYesNoDialog(localization.GetLocalizedString("SureDelete", Ingredient.Name),
                                                                                                   localization.GetLocalizedString("CannotUndo"),
                                                                                                   successCallback: () => OnIngredientDeleted(ingredientID));

        private async void OnIngredientDeleted(Guid id)
        {
            await ingredientService.DeleteAsync(id);
            eventAggregator.GetEvent<IngredientDeletedEvent>().Publish(id);
        }
    }
}