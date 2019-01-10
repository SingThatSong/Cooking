using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class IngredientGroupDTO
    {
        public Guid? ID { get; set; }

        public string Name { get; set; }
        public ObservableCollection<RecipeIngredientDTO> Ingredients { get; set; }
    }
}
