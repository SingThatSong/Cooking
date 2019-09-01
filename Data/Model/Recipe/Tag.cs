using System.Collections.Generic;

namespace Data.Model
{
    public class Tag : Entity
    {
        public string Name { get; set; }
        public TagType Type { get; set; }
        public string Color { get; set; }

        public virtual List<RecipeTag> Recipies { get; set; }
    }
}
