using Data.Model;
using System.ComponentModel;

namespace Cooking.DTO
{
    public class GarnishEdit : Entity, INotifyPropertyChanged
    {
        public string? Name { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}