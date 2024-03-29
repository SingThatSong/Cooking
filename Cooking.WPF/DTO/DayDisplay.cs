﻿using System;

namespace Cooking.WPF.DTO;

/// <summary>
/// Dto for day displaying.
/// </summary>
public class DayDisplay : EntityNotify
{
    /// <summary>
    /// Gets or sets a value indicating whether dinner on a given day was cooked.
    /// </summary>
    public bool DinnerWasCooked { get; set; }

    /// <summary>
    /// Gets or sets day of week for given day.
    /// </summary>
    public DayOfWeek DayOfWeek { get; set; }

    /// <summary>
    /// Gets or sets dinner that selected to be cooked on given day.
    /// </summary>
    public RecipeListViewDto? Dinner { get; set; }

    /// <summary>
    /// Gets or sets dinner that selected to be cooked on given day.
    /// </summary>
    public RecipeListViewDto? DinnerGarnish { get; set; }
}
