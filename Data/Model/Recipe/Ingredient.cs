using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Data.Model
{
    public class Ingredient
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

    }
}
