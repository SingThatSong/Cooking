using Data.Model;
using PropertyChanged;
using System;
using System.ComponentModel;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class TagEdit : INotifyPropertyChanged
    {
        public static readonly TagEdit Any = new TagEdit()
        {
            Name = "Любой",
            CanBeRemoved = false
        };

        public Guid ID { get; set; }

        public string? Name { get; set; }
        public TagType Type { get; set; }
        public string? Color { get; set; }

        public bool IsChecked { get; set; }
        public bool CanBeRemoved { get; set; } = true;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}