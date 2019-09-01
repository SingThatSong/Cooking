using Cooking.Pages.MainPage;
using Cooking.ServiceLayer;
using Data.Model;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class RecipeMain : RecipeFull
    {        
        [AlsoNotifyFor(nameof(FullPath))]
        public new string ImagePath { get; set; }

        public string FullPath => ImagePath != null 
                                ? Path.GetFullPath(ImagePath) 
                                : null;

        public new ObservableCollection<IngredientGroupMain> IngredientGroups { get; set; }
        public new ObservableCollection<RecipeIngredientMain> Ingredients { get; set; }
        public new ObservableCollection<TagDTO> Tags { get; set; }

        public int LastCooked => RecipeService.DaysFromLasCook(ID);
    }
}
