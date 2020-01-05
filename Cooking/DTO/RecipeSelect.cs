using Data.Model;
using PropertyChanged;
using System;
using System.IO;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class RecipeSelectDto
    {
        public Guid ID { get; set; }
        public string? Name { get; set; }
        public CalorieType CalorieType { get; set; }

        [AlsoNotifyFor(nameof(FullPath))]
        public string? ImagePath { get; set; }

        public string? FullPath => ImagePath != null && File.Exists(Path.GetFullPath(ImagePath))
                                ? Path.GetFullPath(ImagePath) 
                                : null;
        public bool IsSelected { get; set; }

        public int Rating { get; set; }
    }
}
