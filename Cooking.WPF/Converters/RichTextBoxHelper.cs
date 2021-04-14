using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;

namespace Cooking.WPF.Converters
{
    /// <summary>
    /// Converter for binding document to RTF string.
    /// </summary>
    [SuppressMessage("Style", "RCS1163", Justification = "Parameter is required by AttachedProperty")]
    [SuppressMessage("Style", "IDE0060", Justification = "Parameter is required by AttachedProperty")]
    public sealed class RichTextBoxHelper
    {
        /// <summary>
        /// Gets or sets attached property to serve as a proxy for binding between RichTextBox's document and string in view model.
        /// </summary>
        public static readonly DependencyProperty DocumentXamlProperty = DependencyProperty.RegisterAttached(
                                                                                  "DocumentXaml",
                                                                                  typeof(string),
                                                                                  typeof(RichTextBoxHelper),
                                                                                  new FrameworkPropertyMetadata(defaultValue: string.Empty,
                                                                                                                propertyChangedCallback: OnDocumentXamlChanged,
                                                                                                                flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private enum RichTextboxStatus
        {
            CanBeModified,
            CanNotBeModified
        }

        /// <summary>
        /// Getter for AttachedProperty.
        /// </summary>
        /// <param name="richTextBox">RichTextBox to attach to.</param>
        /// <returns>Nothimg.</returns>
        public static string GetDocumentXaml(DependencyObject richTextBox)
        {
            return (string)richTextBox.GetValue(DocumentXamlProperty);
        }

        /// <summary>
        /// Setter for AttachedProperty.
        /// </summary>
        /// <param name="richTextBox">RichTextBox to attach to.</param>
        /// <param name="value">Value of document.</param>
        public static void SetDocumentXaml(DependencyObject richTextBox, string value)
        {
            richTextBox.SetValue(DocumentXamlProperty, value);
        }

        private static void OnDocumentXamlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var richTextBox = (RichTextBox)dependencyObject;

            if (richTextBox.Tag != null && (RichTextboxStatus)richTextBox.Tag != RichTextboxStatus.CanBeModified)
            {
                return;
            }

            // Get document
            string text = GetDocumentXaml(dependencyObject);
            richTextBox.Document ??= new FlowDocument();

            if (text != null)
            {
                // Set the document
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
                var range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                range.Load(stream, DataFormats.Rtf);
            }

            if (richTextBox.Tag == null)
            {
                richTextBox.Tag = RichTextboxStatus.CanBeModified;

                // When the document changes update the source
                richTextBox.TextChanged += (obj2, e2) =>
                {
                    richTextBox.Tag = RichTextboxStatus.CanNotBeModified;
                    var buffer = new MemoryStream();
                    new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Save(buffer, DataFormats.Rtf);
                    SetDocumentXaml(richTextBox, Encoding.UTF8.GetString(buffer.ToArray()));
                    richTextBox.Tag = RichTextboxStatus.CanBeModified;
                };
            }
        }
    }
}
