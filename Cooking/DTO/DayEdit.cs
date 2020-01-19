using Cooking.Data.Model;
using System;
using System.ComponentModel;

namespace Cooking.WPF.DTO
{
    public class DayEdit : Entity, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public bool DinnerWasCooked { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public RecipeSelectDto? Dinner { get; set; }
    }
}