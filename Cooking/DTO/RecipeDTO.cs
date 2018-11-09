using Data.Model;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace Cooking.DTO
{
    public class RecipeDTO : INotifyPropertyChanged
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
        public double Rating { get; set; }
        public int PortionsCount { get; set; }
        public CalorieType CalorieType { get; set; }

        public ObservableCollection<IngredientGroupDTO> IngredientGroups { get; set; }
        public ObservableCollection<RecipeIngredientDTO> Ingredients { get; set; }
        public ObservableCollection<TagDTO> Tags { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
