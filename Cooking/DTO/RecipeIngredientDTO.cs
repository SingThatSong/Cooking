﻿using Cooking.ServiceLayer;
using Data.Model;
using PropertyChanged;
using System;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class RecipeIngredientMain
    {
        public Guid ID { get; set; }
        public IngredientMain Ingredient { get; set; }
        public double? Amount { get; set; }
        public MeasureUnit MeasureUnit { get; set; }
        public int Order { get; set; }
    }
}