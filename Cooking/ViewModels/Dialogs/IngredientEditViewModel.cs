using Cooking.DTO;
using Cooking.Helpers;
using Cooking.WPF.Helpers;
using Data.Model;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages
{
    public partial class IngredientEditViewModel : OkCancelViewModel, INotifyPropertyChanged
    {
        private readonly ILocalization localization;

        // State
        private bool NameChanged { get; set; }
        private List<string> AllIngredientNames { get; set; }
        public IngredientEdit Ingredient { get; set; }
        public IEnumerable<string>? SimilarIngredients => string.IsNullOrWhiteSpace(Ingredient?.Name)
                                                        ? null
                                                        : AllIngredientNames.OrderBy(x => IngredientCompare(x, Ingredient.Name)).Take(3);
        public ReadOnlyCollection<IngredientType> IngredientTypes => IngredientType.AllValues;

        public IngredientEditViewModel(IngredientService ingredientService, DialogService dialogService, ILocalization localization, IngredientEdit? category = null) : base(dialogService)
        {
            this.localization = localization;
            Ingredient = category ?? new IngredientEdit();
            AllIngredientNames = ingredientService.GetSearchNames();
            Ingredient.PropertyChanged += (src, e) =>
            {
                if (e.PropertyName == nameof(Ingredient.Name))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SimilarIngredients)));
                    NameChanged = true;
                }
            };
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
                                                    successCallback: () => saveAnyway = true).ConfigureAwait(false);

                if (!saveAnyway)
                {
                    return;
                }
            }

            await base.Ok().ConfigureAwait(false);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private int IngredientCompare(string str1, string str2)
             => StringCompare.DiffLength(
                        string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                        string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
                );
    }
}