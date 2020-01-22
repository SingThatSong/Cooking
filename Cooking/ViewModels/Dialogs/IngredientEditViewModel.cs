using Cooking.Data.Model;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Helpers;
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
    public partial class IngredientEditViewModel : OkCancelViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ILocalization localization;

        // State
        public bool SimilarIngredientsPresent => SimilarIngredients?.Count() > 0;
        private bool NameChanged { get; set; }
        private List<string> AllIngredientNames { get; set; }
        public IngredientEdit Ingredient { get; set; }
        public IEnumerable<string>? SimilarIngredients => string.IsNullOrWhiteSpace(Ingredient?.Name)
                                                        ? null
                                                        : AllIngredientNames.OrderBy(x => IngredientCompare(x, Ingredient.Name)).Take(3);
        public ReadOnlyCollection<IngredientType> IngredientTypes => IngredientType.AllValues;
        public DelegateCommand LoadedCommand { get; }
        public string? NameCaption => localization.GetLocalizedString("Name");
        public string? TypeCaption => localization.GetLocalizedString("Type");
        public string? MaybeYouWantCaption => localization.GetLocalizedString("MaybeYouWant");

        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientEditViewModel"/> class.
        /// </summary>
        /// <param name="ingredientService"></param>
        /// <param name="dialogService"></param>
        /// <param name="localization"></param>
        /// <param name="category"></param>
        public IngredientEditViewModel(IngredientService ingredientService, DialogService dialogService, ILocalization localization, IngredientEdit? category = null)
            : base(dialogService)
        {
            this.localization = localization;
            Ingredient = category ?? new IngredientEdit();
            AllIngredientNames = ingredientService.GetSearchNames();
            Ingredient.PropertyChanged += (src, e) =>
            {
                if (e.PropertyName == nameof(Ingredient.Name))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SimilarIngredients)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SimilarIngredientsPresent)));
                    NameChanged = true;
                }
            };
            LoadedCommand = new DelegateCommand(OnLoaded);
        }

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
             => StringCompare.DiffLength(
                        string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                        string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
                );
    }
}