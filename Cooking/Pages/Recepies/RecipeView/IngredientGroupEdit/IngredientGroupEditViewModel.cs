using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cooking.Pages.Recepies
{
    public partial class IngredientGroupEditViewModel : INotifyPropertyChanged
    {
        public bool DialogResultOk { get; set; }

        public IngredientGroupEditViewModel() : this(null) { }
        public IngredientGroupEditViewModel(IngredientGroupMain? ingredientGroup = null)
        {
            OkCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(() => {
                    DialogResultOk = true;
                    CloseCommand.Value.Execute();
                }));

            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));
            IngredientGroup = ingredientGroup ?? new IngredientGroupMain();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }

        public IngredientGroupMain IngredientGroup { get; }
    }
}