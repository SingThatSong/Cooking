using Cooking.Data.Model;
using System.ComponentModel;
using Validar;

namespace Cooking.WPF.DTO
{
    [InjectValidation]
    public class IngredientEdit : Entity, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        public string? Name { get; set; }
        public IngredientType? Type { get; set; }
    }
}
