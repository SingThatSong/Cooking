﻿using Bindables;
using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("Стиль", "RCS1163", Justification = "Parameter is required by AttachedProperty")]
    [SuppressMessage("Стиль", "IDE0060", Justification = "Parameter is required by AttachedProperty")]
    [SuppressMessage("Стиль", "CA1801", Justification = "Parameter is required by AttachedProperty")]
    [SuppressMessage("Стиль", "CA1721", Justification = "Names are intended by AttachedProperty")]
    public sealed class RichTextBoxHelper
    {
        /// <summary>
        /// Gets or sets attached property to serve as a proxy for binding between RichTextBox's document and string in view model.
        /// </summary>
        [AttachedProperty(OnPropertyChanged = nameof(OnDocumentXamlChanged), Options = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
        public static string? DocumentXaml { get; set; }

        private static bool IsEditing { get; set; }

        /// <summary>
        /// Getter for AttachedProperty. Filled in with <see cref="Bindables"/>.
        /// </summary>
        /// <param name="richTextBox">RichTextBox to attach to.</param>
        /// <returns>Nothimg.</returns>
        public static string GetDocumentXaml(DependencyObject richTextBox) => throw new WillBeImplementedByBindablesException();

        /// <summary>
        /// Setter for AttachedProperty. Filled in with <see cref="Bindables"/>.
        /// </summary>
        /// <param name="richTextBox">RichTextBox to attach to.</param>
        /// <param name="value">Value of document.</param>
        public static void SetDocumentXaml(DependencyObject richTextBox, string value)
        {
        }

        private static void OnDocumentXamlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (IsEditing)
            {
                return;
            }

            var richTextBox = (RichTextBox)dependencyObject;

            // Parse the XAML to a document (or use XamlReader.Parse())
            string xaml = GetDocumentXaml(dependencyObject);
            var doc = new FlowDocument();
            var range = new TextRange(doc.ContentStart, doc.ContentEnd);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xaml));
            range.Load(stream, DataFormats.Rtf);

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
