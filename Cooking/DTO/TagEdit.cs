using Cooking.Data.Model;
using System.ComponentModel;
using Validar;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// Dto for tag edit.
    /// </summary>
    [InjectValidation]
    public class TagEdit : Entity, INotifyPropertyChanged
    {
        public static readonly TagEdit Any = new TagEdit()
        {
            CanBeRemoved = false
        };

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? Name { get; set; }
        public TagType Type { get; set; }
        public string? Color { get; set; }

        public bool IsChecked { get; set; }
        public bool CanBeRemoved { get; set; } = true;
    }
}