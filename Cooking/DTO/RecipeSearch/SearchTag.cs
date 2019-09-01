using Data.Model;
using PropertyChanged;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class SearchTag : Entity
    {
        public static readonly SearchTag Any = new SearchTag()
        {
            Name = "Любой",
            CanBeRemoved = false
        };

        public string Name { get; set; }
        public bool IsChecked { get; set; }
        public bool CanBeRemoved { get; set; } = true;
    }
}