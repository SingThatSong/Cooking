using Data.Model;
using PropertyChanged;
using System;
using Validar;

namespace Cooking.DTO
{
    [InjectValidation]
    [AddINotifyPropertyChangedInterface]
    public class RecipeIngredientEdit
    {
        public Guid ID { get; set; }
        public IngredientEdit? Ingredient { get; set; }

        // Validation of Amount and MeasureUnit depends on each other, so we have to notify interface about changes
        [AlsoNotifyFor(nameof(MeasureUnit))]
        public double? Amount { get; set; }
        [AlsoNotifyFor(nameof(Amount))]
        public MeasureUnit? MeasureUnit { get; set; }

        public int Order { get; set; }
    }
}