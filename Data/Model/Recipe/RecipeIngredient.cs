using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Data.Model
{
    public class RecipeIngredient : Entity
    {
        public int Order { get; set; }
        public Guid? IngredientId { get; set; }
        public virtual Ingredient Ingredient { get; set; }
        public double? Amount { get; set; }

        [NotMapped]
        public MeasureUnit? MeasureUnit { get; set; }

        public int? MeasureUnitID
        {
            get => MeasureUnit?.ID;
            set
            {
                MeasureUnit = value != null
                              ? MeasureUnit.AllValues.Single(x => x.ID == value)
                              : null;
            }
        }
    }
}