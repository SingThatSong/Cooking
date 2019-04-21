using Data.Model;
using PropertyChanged;
using System;
using System.ComponentModel;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class RecipeIngredientDTO
    {
        public Guid? ID { get; set; }
        public IngredientDTO Ingredient { get; set; }
        public double? Amount { get; set; }
        public MeasureUnit MeasureUnit { get; set; }
        public int Order { get; set; }
    }
}