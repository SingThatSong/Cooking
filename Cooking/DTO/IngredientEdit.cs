﻿using Data.Model;
using System;
using System.ComponentModel;

namespace Cooking.DTO
{
    public class IngredientEdit : INotifyPropertyChanged
    {
        public Guid ID { get; set; }
        public string? Name { get; set; }
        public IngredientType? Type { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
