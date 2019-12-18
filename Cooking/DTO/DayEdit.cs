using Cooking.ServiceLayer.MainPage;
using System.ComponentModel;

namespace Cooking.DTO
{
    public class DayEdit : DayMainPage, INotifyPropertyChanged
    {
        public new RecipeSelectDto? Dinner { get; set; }
        public new bool DinnerWasCooked { get; set; }
#pragma warning disable CS0067
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
    }
}