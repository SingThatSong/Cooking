using Data.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Cooking.DTO
{
    public class RecipeDTO : INotifyPropertyChanged
    {
        public Guid ID { get; set; }

        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string FullPath { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }
        public int PortionsCount { get; set; }
        public CalorieType CalorieType { get; set; }

        public ObservableCollection<RecipeIngredientDTO> Ingredients { get; set; }
        public ObservableCollection<TagDTO> Tags { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
