using Cooking.Data.Model;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Validar;

namespace Cooking.WPF.DTO
{
    [AddINotifyPropertyChangedInterface]
    [InjectValidation]
    [SuppressMessage("Usage", "CA2227:Свойства коллекций должны быть доступны только для чтения")]
    public class IngredientGroupEdit : Entity
    {
        public string? Name { get; set; }
        public ObservableCollection<RecipeIngredientEdit>? Ingredients { get; set; }
    }
}
