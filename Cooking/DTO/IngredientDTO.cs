using Cooking.ServiceLayer;
using PropertyChanged;
using System.ComponentModel;

namespace Cooking.DTO
{
    public class IngredientMain : IngredientData, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
