using System.Windows;

namespace Cooking.WPF.Converters;

/// <summary>
/// Proxy for binding for menus and popups.
/// </summary>
public class BindingProxy : Freezable
{
    /// <summary>
    /// DependencyProperty for <see cref="Data"/>.
    /// </summary>
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
                                                                nameof(Data),
                                                                typeof(object),
                                                                typeof(BindingProxy));

    /// <summary>
    /// Gets or sets property for binding proxy.
    /// </summary>
    public object? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    /// <inheritdoc/>
    protected override Freezable CreateInstanceCore() => new BindingProxy();
}
