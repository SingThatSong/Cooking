using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Cooking.DTO
{
    public class IngredientGroupDTO : INotifyPropertyChanged
    {
        public Guid? ID { get; set; }

        public string Name { get; set; }
        public ObservableCollection<RecipeIngredientDTO> Ingredients { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
