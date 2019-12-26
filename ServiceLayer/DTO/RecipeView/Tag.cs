using Data.Model;

namespace Cooking.ServiceLayer
{
    public sealed class TagData : Entity
    {
        public string? Name { get; set; }
        public TagType Type { get; set; }
        public string? Color { get; set; }
    }
}
