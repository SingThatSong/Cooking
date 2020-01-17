﻿using Cooking.Data.Model;
using System;
using System.ComponentModel;

namespace Cooking.WPF.DTO
{
    public class DayEdit : Entity, INotifyPropertyChanged
    {
        public bool DinnerWasCooked { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public RecipeSelectDto? Dinner { get; set; }
#pragma warning disable CS0067
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
    }
}