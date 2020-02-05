using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// Logic for <see cref="RecipeView"/>.
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (editRichTextbox != null)
            {
                var r = new TextRange(editRichTextbox.Selection.Start, editRichTextbox.Selection.End);
                dynamic? addedItem = e.AddedItems[0];
                r.ApplyPropertyValue(TextElement.FontSizeProperty, Convert.ToDouble(addedItem!.Content));
            }
        }
    }
}
