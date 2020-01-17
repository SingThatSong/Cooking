using Cooking.Data.Model;
using PropertyChanged;
using System.IO;

namespace Cooking.WPF.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class RecipeSelectDto : Entity
    {
        public string? Name { get; set; }
        public CalorieType CalorieType { get; set; }

        [AlsoNotifyFor(nameof(FullPath))]
        public string? ImagePath { get; set; }

        public string? FullPath => ImagePath != null && File.Exists(Path.GetFullPath(ImagePath))
                                ? Path.GetFullPath(ImagePath) 
                                : null;
        public int Rating { get; set; }
    }
}
