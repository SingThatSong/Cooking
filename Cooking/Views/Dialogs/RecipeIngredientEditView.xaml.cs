using Cooking.WPF.DTO;
using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml.
    /// </summary>
    public partial class RecipeIngredientEditView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeIngredientEditView"/> class.
        /// </summary>
        public RecipeIngredientEditView()
        {
            InitializeComponent();

            // Для того, чтобы окно могло работать с нажатием клавиш на клавиатуре
            // https://stackoverflow.com/a/21352864
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(Ingredient);
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
                else
                {
                    if (o is IngredientEdit ingredient && ingredient.Name != null)
                    {
                        return ingredient.Name.Contains(Ingredient.Text, StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        return false;
                    }
                }
            };

            itemsViewOriginal.Refresh();
            Ingredient.IsDropDownOpen = true;

            // https://stackoverflow.com/a/43727449/1134449
            if (!(e.OriginalSource is TextBox textBox))
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
