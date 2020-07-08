﻿using Cooking.Data.Model;
using Humanizer;
using PropertyChanged;
using System.Threading;
using Validar;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// DTO for edit ingredient in recipe.
    /// </summary>
    [InjectValidation]
    [AddINotifyPropertyChangedInterface]
    public class RecipeIngredientEdit : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeIngredientEdit"/> class.
        /// </summary>
        public RecipeIngredientEdit()
        {
            Culture = Thread.CurrentThread.CurrentUICulture.Name;
        }

        /// <summary>
        /// Gets or sets ingredient in recipe.
        /// </summary>
        public IngredientEdit? Ingredient { get; set; }

        /// <summary>
        /// Gets or sets amout of ingredient in recipe.
        /// </summary>
        // Validation of Amount and MeasureUnit depends on each other, so we have to notify interface about changes
        [AlsoNotifyFor(nameof(MeasureUnit))]
        public double? Amount { get; set; }

        /// <summary>
        /// Gets or sets measurement unit of <see cref="Amount"/> in recipe.
        /// </summary>
        [AlsoNotifyFor(nameof(Amount))]
        public MeasureUnit? MeasureUnit { get; set; }

        /// <summary>
        /// Gets or sets order in ingredients list.
        /// </summary>
        public int Order { get; set; }
    }
}