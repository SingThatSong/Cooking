using Data.Model;
using PropertyChanged;
using System.ComponentModel;

namespace Cooking.DTO
{
    public class GarnishDTO : Entity, INotifyPropertyChanged
    {
        public string? Name { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}