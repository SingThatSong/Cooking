using Cooking.Commands;
using Cooking.DTO;
using Cooking.Helpers;
using Cooking.Pages.Recepies;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages.Ingredients
{
    public partial class IngredientEditViewModel : OkCancelViewModel, INotifyPropertyChanged
    {
        public IngredientMain Ingredient { get; set; }
        private bool NameChanged { get; set; }

        public IngredientEditViewModel() : this(null) { }

        public IngredientEditViewModel(IngredientMain? category = null)
        {
            Ingredient = category ?? new IngredientMain();
            AllIngredientNames = IngredientService.GetSearchNames();
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
            if (NameChanged)
            {
                if (AllIngredientNames.Any(x => x.ToUpperInvariant() == Ingredient.Name.ToUpperInvariant()))
                {
                    var result = await DialogCoordinator.Instance.ShowMessageAsync(
                                        this,
                                        "Такой ингредиент уже существует",
                                        "Всё равно сохранить?",
                                        MessageDialogStyle.AffirmativeAndNegative,
                                        new MetroDialogSettings()
                                        {
                                            AffirmativeButtonText = "Да",
                                            NegativeButtonText = "Нет"
                                        });

                    if (result == MessageDialogResult.Negative)
                    {
                        return;
                    }
                }
            }

            base.Ok();
        }
        
        private List<string> AllIngredientNames { get; set; }

        public ReadOnlyCollection<IngredientType> IngredientTypes => IngredientType.AllValues;

        public event PropertyChangedEventHandler PropertyChanged;
        
        public IEnumerable<string> SimilarIngredients => string.IsNullOrWhiteSpace(Ingredient?.Name)
                                                        ? null
                                                        : AllIngredientNames.OrderBy(x => IngredientCompare(x, Ingredient.Name)).Take(3);

        private int IngredientCompare(string str1, string str2)
             => StringCompare.DiffLength(
                        string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                        string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
                );

    }
}