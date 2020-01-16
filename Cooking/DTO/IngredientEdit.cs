using Cooking.Data.Model;
using System;
using System.ComponentModel;
using Validar;

namespace Cooking.WPF.DTO
{
    [InjectValidation]
    public class IngredientEdit : INotifyPropertyChanged
    {
        public Guid ID { get; set; }
        public string? Name { get; set; }
        public IngredientType? Type { get; set; }

#pragma warning disable CS0067 // Событие не используется
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
    }
}
