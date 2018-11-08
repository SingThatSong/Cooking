using Cooking.DTO;
using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cooking.Pages.Ingredients
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class RecipeIngredientEditView
    {
        public RecipeIngredientEditView()
        {
            InitializeComponent();

            // Для того, чтобы окно могло работать с нажатием клавиш на клавиатуре
            // https://stackoverflow.com/a/21352864
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }

        private void ComboBox_KeyUp(object sender, KeyEventArgs e)
        {


            var Cmb = sender as ComboBox;

            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(Cmb.ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (String.IsNullOrEmpty(Cmb.Text)) return true;
                else
                {
                    if (((IngredientDTO)o).Name.Contains(Cmb.Text)) return true;
                    else return false;
                }
            });

            itemsViewOriginal.Refresh();
            Cmb.IsDropDownOpen = true;

            // https://stackoverflow.com/a/43727449/1134449
            var textBox = e.OriginalSource as TextBox;
            if (textBox == null) return;
            if (textBox.Text.Length >= 2) return;
            textBox.SelectionLength = 0;
            textBox.SelectionStart = 1;
        }

        private void ComboBox_KeyUp_1(object sender, KeyEventArgs e)
        {

        }
    }
}
