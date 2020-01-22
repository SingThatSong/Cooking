using System.Diagnostics;
using System.Windows.Controls;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml.
    /// </summary>
    public partial class RecipeView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeView"/> class.
        /// </summary>
        public RecipeView()
        {
            InitializeComponent();
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
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true,
                Verb = "open"
            });
            e.Handled = true;
        }
    }
}
