using System;
using System.ComponentModel;

namespace Cooking.DTO
{
    public class IngredientDTO : INotifyPropertyChanged
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
