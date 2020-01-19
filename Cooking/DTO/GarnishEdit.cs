using Cooking.Data.Model;
using System.ComponentModel;
using Validar;

namespace Cooking.WPF.DTO
{
    [InjectValidation]
    public class GarnishEdit : Entity, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string? Name { get; set; }
    }
}