using Data.Model;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    [SuppressMessage("Usage", "CA2227:Свойства коллекций должны быть доступны только для чтения")]
    public class RecipeEdit : Entity
    {
        [AlsoNotifyFor(nameof(FullPath))]
        public string? ImagePath { get; set; }

        public string? FullPath => ImagePath != null && File.Exists(Path.GetFullPath(ImagePath))
                                ? Path.GetFullPath(ImagePath)
                                : null;


        public List<IngredientGroupEdit>? IngredientGroups { get; set; }
        public List<RecipeIngredientEdit>? Ingredients { get; set; }
        public List<TagEdit>? Tags { get; set; }

        public int LastCooked { get; set; } // => RecipeService.DaysFromLasCook(ID);

        public Uri? SourceUrl { get; set; }
        public string? Description { get; set; }
        public int PortionsCount { get; set; }
        public int? Difficulty { get; set; }
        public int? Rating { get; set; }

        public string? Name { get; set; }
        public CalorieType CalorieType { get; set; }
    }
}
