using Bindables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Cooking.WPF.Converters
{
    /// <summary>
    /// Proxy for binding for menus and popups.
    /// </summary>
    [DependencyProperty]
    public class BindingProxy : Freezable
    {
        /// <summary>
        /// Gets or sets property for binding proxy.
        /// </summary>
        public object? Data { get; set; }

        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore() => new BindingProxy();
    }
}
