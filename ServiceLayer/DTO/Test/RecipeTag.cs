using System;

namespace TEST
{
    public class RecipeTag
    {
        public virtual Recipe Recipe { get; set; }
        public virtual Tag Tag { get; set; }
        public Guid RecipeId { get; set; }
        public Guid TagId { get; set; }
    }
}
