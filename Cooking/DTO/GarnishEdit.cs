using Data.Model;
using System.ComponentModel;

namespace Cooking.DTO
{
    public class GarnishEdit : Entity, INotifyPropertyChanged
    {
        public string? Name { get; set; }
#pragma warning disable CS0067 // Событие не используется
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
    }
}