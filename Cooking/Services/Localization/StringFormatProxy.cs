using Bindables;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using WPFLocalizeExtension.Engine;

namespace Cooking.WPF
{
    /// <summary>
    /// Copied from https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/16fa1f6168fb803966d8217e5a406685c49d6854/tests/AssemblyTest/StringFormatProxy.cs
    /// and converted to Bindables.Fody
    /// Github issue to include in library https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/issues/214
    ///
    /// Last update 05.09.2018.
    /// </summary>
    public class StringFormatProxy : FrameworkElement
    {
        /// <summary>
        /// Gets or sets string format for proxy.
        /// </summary>
        [DependencyProperty(OnPropertyChanged = nameof(DataChanged))]
        public string StringFormat { get; set; } = "{0}";

        /// <summary>
        /// Gets or sets value format for proxy.
        /// </summary>
        [DependencyProperty(OnPropertyChanged = nameof(DataChanged))]
        [SuppressMessage("Naming", "CA1721:Имена свойств не должны совпадать с именами методов get", Justification = "<Ожидание>")]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets formatted result.
        /// </summary>
        [DependencyProperty]
        public string? Result { get; set; }

        [SuppressMessage("Стиль", "IDE0060", Justification = "args is an argument for OnPropertyChanged injected by Bindables.Fody")]
        [SuppressMessage("Стиль", "CA1801",  Justification = "args is an argument for OnPropertyChanged injected by Bindables.Fody")]
        private static void DataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is StringFormatProxy sfp && sfp.StringFormat != null)
            {
                sfp.Result = string.Format(LocalizeDictionary.Instance.Culture, sfp.StringFormat, sfp.Value);
            }
        }
    }
}
