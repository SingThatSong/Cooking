using System;
using System.ComponentModel;

namespace Cooking.DTO
{
    public class DayDTO : INotifyPropertyChanged
    {
        public Guid? ID { get; set; }
        public Guid? DinnerID { get; set; }
        public RecipeDTO Dinner { get; set; }
        public bool DinnerWasCooked { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}