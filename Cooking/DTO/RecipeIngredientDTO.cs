using Data.Model;
using System;
using System.ComponentModel;

namespace Cooking.DTO
{
    public class RecipeIngredientDTO : INotifyPropertyChanged
    {
        public Guid? ID { get; set; }
        public IngredientDTO Ingredient { get; set; }
        public double? Amount { get; set; }
        public MeasureUnit MeasureUnit { get; set; }
        public int Order { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}