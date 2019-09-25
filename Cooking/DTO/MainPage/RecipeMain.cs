using Data.Model;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class RecipeMain
    {        
        [AlsoNotifyFor(nameof(FullPath))]
        public string ImagePath { get; set; }

        public string FullPath => ImagePath != null 
                                ? Path.GetFullPath(ImagePath) 
                                : null;

        public ObservableCollection<IngredientGroupMain> IngredientGroups { get; set; }
        public ObservableCollection<RecipeIngredientMain> Ingredients { get; set; }
        public ObservableCollection<TagDTO> Tags { get; set; }

        public int LastCooked => RecipeService.DaysFromLasCook(ID);

        public string SourceUrl { get; set; }
        public string Description { get; set; }
        public int PortionsCount { get; set; }
        public int? Difficulty { get; set; }
        public int? Rating { get; set; }

        public string Name { get; set; }
        public CalorieType CalorieType { get; set; }
        public Guid ID { get; set; }
    }
}
