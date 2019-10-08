using Cooking.ServiceLayer;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class IngredientGroupMain
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public ObservableCollection<RecipeIngredientMain> Ingredients { get; set; }
    }
}
