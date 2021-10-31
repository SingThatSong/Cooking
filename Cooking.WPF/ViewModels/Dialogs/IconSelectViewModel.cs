using System;
using System.Windows.Data;
using MahApps.Metro.IconPacks;
using WPF.Commands;

namespace Cooking.WPF.ViewModels;

/// <summary>
/// View model for selecting calorie types.
/// </summary>
public class IconSelectViewModel : OkCancelViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IconSelectViewModel"/> class.
    /// </summary>
    /// <param name="dialogService">Dialog service dependency to close dialog.</param>
    public IconSelectViewModel(DialogService dialogService)
        : base(dialogService)
    {
        LoadedCommand = new DelegateCommand(Loaded);
    }

    /// <summary>
    /// Gets command to execute on loaded event.
    /// </summary>
    public DelegateCommand LoadedCommand { get; }

    /// <summary>
    /// Gets all calorie types to select from.
    /// </summary>
    public CollectionViewSource? AllValues { get; private set; }

    /// <summary>
    /// Gets or sets text to filter icons.
    /// </summary>
    public string? FilterText { get; set; }

    /// <summary>
    /// Gets or sets selected icon.
    /// </summary>
    public string? SelectedIcon { get; set; }

#pragma warning disable IDE0051, RCS1213
    private void OnFilterTextChanged() => AllValues?.View?.Refresh();
#pragma warning restore IDE0051, RCS1213

    private void AllValues_Filter(object sender, FilterEventArgs e)
    {
        e.Accepted = string.IsNullOrEmpty(FilterText) || (e.Item as string)?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) == true;
    }

    private void Loaded()
    {
        AllValues = new CollectionViewSource();
        AllValues.Filter += AllValues_Filter;
        AllValues.Source = GetIcons(typeof(PackIconModernKind)).ToList();
    }

    private IEnumerable<string> GetIcons(Type enumType)
    {
        return Enum.GetValues(enumType)
            .OfType<Enum>()
            .Where(k => k.ToString() != "None")
            .Select(value => value.ToString());
    }
}
