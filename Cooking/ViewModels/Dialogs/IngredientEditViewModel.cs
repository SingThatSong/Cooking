using Cooking.Data.Model;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// View model for ingredient edit dialog.
    /// </summary>
    public partial class IngredientEditViewModel : OkCancelViewModel, INotifyPropertyChanged
    {
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientEditViewModel"/> class.
        /// </summary>
        /// <param name="ingredientService">Ingredient service dependency.</param>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="localization">Localization service dependency.</param>
        /// <param name="ingredient">Ingredient to edit. Null means.</param>
        public IngredientEditViewModel(IngredientService ingredientService,
                                       DialogService dialogService,
                                       ILocalization localization,
                                       IngredientEdit? ingredient = null)
            : base(dialogService)
        {
            this.localization = localization;
            Ingredient = ingredient ?? new IngredientEdit();
            AllIngredientNames = ingredientService.GetNames();
            Ingredient.PropertyChanged += (src, e) =>
            {
                if (e.PropertyName == nameof(Ingredient.Name))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SimilarIngredients)));
                    NameChanged = true;
                }
            };
            LoadedCommand = new DelegateCommand(OnLoaded);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets ingreditne to edit.
        /// </summary>
        public IngredientEdit Ingredient { get; set; }

        /// <summary>
        /// Gets similar ingredients list to check on duplicates.
        /// </summary>
        public IEnumerable<string>? SimilarIngredients => string.IsNullOrWhiteSpace(Ingredient?.Name)
                                                        ? null
                                                        : AllIngredientNames.OrderBy(x => IngredientCompare(x, Ingredient.Name)).Take(3);

        /// <summary>
        /// Gets all types of ingredients to select from.
        /// </summary>
        public ReadOnlyCollection<IngredientType> IngredientTypes => IngredientType.AllValues;

        /// <summary>
        /// Gets command to execute on loaded event.
        /// </summary>
        public DelegateCommand LoadedCommand { get; }

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
             && AllIngredientNames.Any(x => x.ToUpperInvariant() == Ingredient.Name.ToUpperInvariant()))
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
        protected override bool CanOk()
        {
            if (Ingredient is INotifyDataErrorInfo dataErrorInfo)
            {
                return !dataErrorInfo.HasErrors;
            }
            else
            {
                return true;
            }
        }

        private int IngredientCompare(string str1, string str2)
             => StringCompare.LevensteinDistance(
                        string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                        string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
                );

        // WARNING: this is a crunch
        // When we open ingredient creation dialog second+ time, validation cannot see Ingredient being a required property, but when we change it's value - everything is ok
        // There is no such behaviour when using navigation, so it seems it's something Mahapps-related
        private void OnLoaded()
        {
            string? nameBackup = Ingredient.Name;
            IngredientType? typeBackup = Ingredient.Type;

            Ingredient.Name = "123";
            Ingredient.Name = nameBackup;
            Ingredient.Type = IngredientType.Spice;
            Ingredient.Type = IngredientType.Vegetables;

            Ingredient.Type = typeBackup;
        }
    }
}