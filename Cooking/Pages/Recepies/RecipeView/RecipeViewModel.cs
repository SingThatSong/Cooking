using AutoMapper;
using Cooking.DTO;
using Cooking.Pages.Ingredients;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Cooking.Pages.Recepies
{
    public partial class RecipeViewModel : INotifyPropertyChanged
    {
        public RecipeViewModel(RecipeDTO recipe)
        {
            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

            Recipe = recipe;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> CloseCommand { get; }
        
        public RecipeDTO Recipe { get; set; }
    }
}