using Cooking.ServiceLayer;
using Cooking.ServiceLayer.Projections;
using Data.Model;
using PropertyChanged;
using System;
using System.IO;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class RecipeSelect : RecipeSlim
    {        
        [AlsoNotifyFor(nameof(FullPath))]
        public new string ImagePath { get; set; }

        public string? FullPath => ImagePath != null 
                                ? Path.GetFullPath(ImagePath) 
                                : null;
        public bool IsSelected { get; set; }
    }
}
