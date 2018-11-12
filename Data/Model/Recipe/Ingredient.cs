using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Data.Model
{
    public class Ingredient
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        [NotMapped]
        public IngredientType Type { get; set; }

        public int? TypeID
        {
            get => Type?.ID;
            set
            {
                Type = IngredientType.AllValues.SingleOrDefault(x => x.ID == value);
            }
        }
    }
}
