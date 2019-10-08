using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cooking.Pages.Recepies
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class RecipeView
    {
        public RecipeView()
        {
            InitializeComponent();

            // Для того, чтобы окно могло работать с нажатием клавиш на клавиатуре
            // https://stackoverflow.com/a/21352864
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }

        private void RichTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (e.Changes.Count > 1 && e.UndoAction != UndoAction.Clear)
            {
                RecipeEdit.ScrollToEnd();
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
