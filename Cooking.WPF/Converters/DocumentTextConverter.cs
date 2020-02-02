using Bindables;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Cooking.WPF.Converters
{
    public class RichTextBoxHelper : DependencyObject
    {
        public static string GetDocumentXaml(DependencyObject obj)
        {
            return (string)obj.GetValue(DocumentXamlProperty);
        }
        public static void SetDocumentXaml(DependencyObject obj, string value)
        {
            obj.SetValue(DocumentXamlProperty, value);
        }

        public static bool IsEditing { get; set; }

        public static readonly DependencyProperty DocumentXamlProperty =
          DependencyProperty.RegisterAttached(
            "DocumentXaml",
            typeof(string),
            typeof(RichTextBoxHelper),
            new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                PropertyChangedCallback = (obj, e) =>
                {
                    if (IsEditing) return;

                    var richTextBox = (RichTextBox)obj;

                    // Parse the XAML to a document (or use XamlReader.Parse())
                    var xaml = GetDocumentXaml(richTextBox);
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
            });
    }
}
