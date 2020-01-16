using Cooking.Data.Model;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Validar;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    [InjectValidation]
    [SuppressMessage("Usage", "CA2227:Свойства коллекций должны быть доступны только для чтения")]
    public class RecipeEdit : Entity
    {
        [AlsoNotifyFor(nameof(FullPath))]
        public string? ImagePath { get; set; }

        public string? FullPath => ImagePath != null && File.Exists(Path.GetFullPath(ImagePath))
                                ? Path.GetFullPath(ImagePath)
                                : null;


        public ObservableCollection<IngredientGroupEdit>? IngredientGroups { get; set; }
        public ObservableCollection<RecipeIngredientEdit>? Ingredients { get; set; }
        public ObservableCollection<TagEdit>? Tags { get; set; }

        public int LastCooked { get; set; }

        public Uri? SourceUrl { get; set; }
        public string? Description { get; set; }
        public int PortionsCount { get; set; }
        public int? Difficulty { get; set; }
        public int? Rating { get; set; }

        public string? Name { get; set; }
        public CalorieType CalorieType { get; set; }
    }
}
