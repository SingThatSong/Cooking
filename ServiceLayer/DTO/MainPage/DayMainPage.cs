using Data.Model;
using System;

namespace Cooking.ServiceLayer.MainPage
{
    public class DayMainPage : Entity
    {
        public RecipeSlim Dinner { get; set; }
        public bool DinnerWasCooked { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}