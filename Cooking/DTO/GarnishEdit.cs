using System;
using System.ComponentModel;
using Validar;

namespace Cooking.WPF.DTO
{
    [InjectValidation]
    public class GarnishEdit : INotifyPropertyChanged
    {
        public Guid ID { get; set; }
        public string? Name { get; set; }
#pragma warning disable CS0067 // Событие не используется
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
    }
}