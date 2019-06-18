using Cooking.Pages.MainPage;
using Data.Model;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class RecipeDTO
    {
        public Guid ID { get; set; }

        public string Name { get; set; }
        public string SourceUrl { get; set; }
        
        [AlsoNotifyFor(nameof(FullPath))]
        public string ImagePath { get; set; }

        public string FullPath => ImagePath != null 
                                ? Path.GetFullPath(ImagePath) 
                                : null;

        public string Description { get; set; }
        public int PortionsCount { get; set; }
        public CalorieType CalorieType { get; set; }

        public int? Difficulty { get; set; }
        public int? Rating { get; set; }

        public ObservableCollection<IngredientGroupDTO> IngredientGroups { get; set; }
        public ObservableCollection<RecipeIngredientDTO> Ingredients { get; set; }
        public ObservableCollection<TagDTO> Tags { get; set; }

        public bool IsSelected { get; set; }
        public int LastCooked => LastDayCooked.DaysFromLasCook(ID);
    }
}
