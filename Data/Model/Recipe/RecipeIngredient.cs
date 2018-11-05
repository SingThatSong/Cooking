using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Data.Model
{
    public class RecipeIngredient
    {
        public Guid? ID { get; set; }
        public Guid? IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }
        public double? Amount { get; set; }

        [NotMapped]
        public MeasureUnit MeasureUnit { get; set; }

        public int? MeasureUnitID
        {
            get => MeasureUnit?.ID;
            set
            {
                if (value != null)
                {
                    MeasureUnit = MeasureUnit.AllValues.Single(x => x.ID == value);
                }
                else
                {
                    MeasureUnit = null;
                }
            }
        }
    }
}