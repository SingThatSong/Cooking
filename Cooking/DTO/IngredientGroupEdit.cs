using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    [SuppressMessage("Usage", "CA2227:Свойства коллекций должны быть доступны только для чтения")]
    public class IngredientGroupEdit
    {
        public Guid ID { get; set; }
        public string? Name { get; set; }
        public ObservableCollection<RecipeIngredientEdit>? Ingredients { get; set; }
    }
}
