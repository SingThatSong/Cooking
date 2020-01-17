using Cooking.Data.Model;
using System.ComponentModel;
using Validar;

namespace Cooking.WPF.DTO
{
    [InjectValidation]
    public class TagEdit : Entity, INotifyPropertyChanged
    {
        public static readonly TagEdit Any = new TagEdit()
        {
            CanBeRemoved = false
        };

        public string? Name { get; set; }
        public TagType Type { get; set; }
        public string? Color { get; set; }

        public bool IsChecked { get; set; }
        public bool CanBeRemoved { get; set; } = true;

#pragma warning disable CS0067 // Событие не используется
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
    }
}