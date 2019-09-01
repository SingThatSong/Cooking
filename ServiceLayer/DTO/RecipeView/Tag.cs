using Data.Model;

namespace Cooking.ServiceLayer
{
    public class TagData : Entity
    {
        public string Name { get; set; }
        public TagType Type { get; set; }
        public string Color { get; set; }
    }
}
