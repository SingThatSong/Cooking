using System.Windows.Controls;
using System.Windows.Input;

namespace Cooking.Pages.Recepies
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class RecipeEditView
    {
        public RecipeEditView()
        {
            InitializeComponent();

            // Для того, чтобы окно могло работать с нажатием клавиш на клавиатуре
            // https://stackoverflow.com/a/21352864
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(Focused);
        }

        private void RichTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (e.Changes.Count > 1 && e.UndoAction != UndoAction.Clear)
            {
                RecipeScroll.ScrollToEnd();
            }
        }
    }
}
