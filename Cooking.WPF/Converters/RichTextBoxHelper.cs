using Bindables;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Cooking.WPF.Converters
{
    /// <summary>
    /// Converter for binding document to RTF string.
    /// </summary>
    public class RichTextBoxHelper
    {
        [AttachedProperty(OnPropertyChanged = nameof(OnDocumentXamlChanged), Options = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
        public static string DocumentXaml { get; set; }

        public static string GetDocumentXaml(DependencyObject richTextBox) => throw new WillBeImplementedByBindablesException();

        public static void SetDocumentXaml(DependencyObject richTextBox, string value)
        {
        }

        public static bool IsEditing { get; set; }

        private static void OnDocumentXamlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (IsEditing)
            {
                return;
            }

            var richTextBox = (RichTextBox)dependencyObject;

            // Parse the XAML to a document (or use XamlReader.Parse())
            var xaml = GetDocumentXaml(dependencyObject);
            var doc = new FlowDocument();
            var range = new TextRange(doc.ContentStart, doc.ContentEnd);

            range.Load(new MemoryStream(Encoding.UTF8.GetBytes(xaml)), DataFormats.Rtf);

            // Set the document
            richTextBox.Document = doc;

            // When the document changes update the source
            richTextBox.TextChanged += (obj2, e2) =>
            {
                if (richTextBox.Document == doc)
                {
                    IsEditing = true;
                    var buffer = new MemoryStream();
                    range.Save(buffer, DataFormats.Rtf);
                    SetDocumentXaml(richTextBox, Encoding.UTF8.GetString(buffer.ToArray()));
                    IsEditing = false;
                }
            };
        }
    }
}
