using Data.Model;
using System;
using System.ComponentModel;

namespace Cooking.DTO
{
    public class TagDTO : INotifyPropertyChanged
    {
        public static readonly TagDTO Any = new TagDTO()
        {
            Name = "Любой",
            CanBeRemoved = false
        };

        public Guid ID { get; set; }

        public string Name { get; set; }
        public TagType Type { get; set; }
        public string Color { get; set; }

        public bool IsChecked { get; set; }
        public bool CanBeRemoved { get; set; } = true;


        public event PropertyChangedEventHandler PropertyChanged;
    }
}