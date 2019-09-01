﻿using Cooking.ServiceLayer.MainPage;
using System.ComponentModel;

namespace Cooking.DTO
{
    public class DayMain : DayMainPage, INotifyPropertyChanged
    {
        public new RecipeSelect Dinner { get; set; }
        public new bool DinnerWasCooked { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}