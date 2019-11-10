using AutoMapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TEST
{
    public class Ingredient : Entity
    {
        public string Name { get; set; }

        //[NotMapped, IgnoreMap]
        //public IngredientType Type { get; set; }

        //public int? TypeID
        //{
        //    get => Type?.ID;
        //    set
        //    {
        //        Type = IngredientType.AllValues.SingleOrDefault(x => x.ID == value);
        //    }
        //}
    }
}
