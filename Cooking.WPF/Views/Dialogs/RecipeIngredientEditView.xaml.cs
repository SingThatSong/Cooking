using Cooking.WPF.DTO;
using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for <see cref="RecipeIngredientEditView"/>.
    /// </summary>
    public partial class RecipeIngredientEditView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeIngredientEditView"/> class.
        /// </summary>
        public RecipeIngredientEditView()
        {
            InitializeComponent();
        }

        private void Ingredient_TextChanged(object sender, TextChangedEventArgs e)
        {
            var itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(Ingredient.ItemsSource);

            if (string.IsNullOrEmpty(Ingredient.Text))
            {
                itemsViewOriginal.Refresh();
                return;
            }

            itemsViewOriginal.Filter = (o) =>
            {
                if (string.IsNullOrEmpty(Ingredient.Text))
                {
                    return true;
                }

                if (o is IngredientEdit ingredient && ingredient.Name != null)
                {
                    return ingredient.Name.Contains(Ingredient.Text, StringComparison.OrdinalIgnoreCase);
                }

                return false;
            };

            itemsViewOriginal.Refresh();
            Ingredient.IsDropDownOpen = true;

            // https://stackoverflow.com/a/43727449/1134449
            if (e.OriginalSource is not TextBox textBox)
            {
                return;
            }

            if (textBox.Text.Length >= 2)
            {
                return;
            }

            textBox.SelectionLength = 0;
            textBox.SelectionStart = 1;
        }
    }
}
