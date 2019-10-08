using System;
using System.Collections.Generic;

namespace Data.Model
{
    public class Tag
    {
        public Tag()
        {
            ID = Guid.NewGuid();
        }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public TagType Type { get; set; }
        public string Color { get; set; }

        public virtual List<RecipeTag> Recipies { get; set; }
    }
}
