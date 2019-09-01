using Cooking.Commands;
using Cooking.DTO;
using Cooking.Helpers;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Cooking.Pages.Ingredients
{
    public partial class IngredientEditViewModel : INotifyPropertyChanged
    {
        public bool DialogResultOk { get; set; }
        private bool NameChanged { get; set; }

        public IngredientEditViewModel(IngredientMain category = null)
        {
            OkCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    if (NameChanged)
                    {
                        if (AllIngredientNames.Any(x => IngredientCompare(Ingredient.Name, x) == 0))
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

                    DialogResultOk = true;
                    CloseCommand.Value.Execute();
                }));

            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

            Ingredient = category ?? new IngredientMain();
            using (var context = new CookingContext())
            {
                AllIngredientNames = context.Ingredients.AsQueryable().Select(x => x.Name).ToList();
            }

            Ingredient.PropertyChanged += Ingredient_PropertyChanged;
        }

        private void Ingredient_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Ingredient.Name))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SimilarIngredients)));
                NameChanged = true;
            }
        }

        private List<string> AllIngredientNames { get; set; }

        public ReadOnlyCollection<IngredientType> IngredientTypes => IngredientType.AllValues;

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }

        public IEnumerable<string> SimilarIngredients => string.IsNullOrWhiteSpace(Ingredient?.Name)
                                                        ? null
                                                        : AllIngredientNames.OrderBy(x => IngredientCompare(x, Ingredient.Name)).Take(3);

        private int IngredientCompare(string str1, string str2)
             => StringCompare.DiffLength(
                        string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                        string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
                );

        public IngredientMain Ingredient { get; set; }
    }
}