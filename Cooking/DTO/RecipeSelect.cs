using Cooking.ServiceLayer.Projections;
using PropertyChanged;
using System.IO;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class RecipeSelectDto : RecipeSlim
    {        
        [AlsoNotifyFor(nameof(FullPath))]
        public new string? ImagePath { get; set; }

        public string? FullPath => ImagePath != null && File.Exists(Path.GetFullPath(ImagePath))
                                ? Path.GetFullPath(ImagePath) 
                                : null;
        public bool IsSelected { get; set; }
    }
}
