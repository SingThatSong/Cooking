﻿using Cooking.Data.Model;
using Validar;

namespace Cooking.WPF.DTO;

/// <summary>
/// Dto for tag edit.
/// </summary>
[InjectValidation]
public class TagEdit : EntityNotify
{
    /// <summary>
    /// Any tag for selection.
    /// TODO: Refactor out from TagEdit.
    /// </summary>
    public static readonly TagEdit Any = new()
    {
        CanBeRemoved = false
    };

    /// <summary>
    /// Gets or sets tag name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets tag type.
    /// </summary>
    public TagType Type { get; set; }

    /// <summary>
    /// Gets or sets tag color.
    /// </summary>
    public string? Color { get; set; } = "#FFFF0000";

    /// <summary>
    /// Gets or sets a value indicating whether is tag checked.
    /// TODO: Refactor out from TagEdit.
    /// </summary>
    public bool IsChecked { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether canTagBeRemoved
    /// TODO: Refactor out from TagEdit.
    /// </summary>
    public bool CanBeRemoved { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether this tag should be included as menu item.
    /// </summary>
    public bool IsInMenu { get; set; }

    /// <summary>
    /// Gets or sets a menu icon.
    /// </summary>
    public string? MenuIcon { get; set; }
}
