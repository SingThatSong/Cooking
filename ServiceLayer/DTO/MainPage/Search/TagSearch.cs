using Data.Model;

namespace ServiceLayer.DTO.MainPage
{
    public sealed class TagServiceDto : Entity
    {
        public string Name { get; set; }
        public TagType Type { get; set; }
        public string Color { get; set; }
    }
}
