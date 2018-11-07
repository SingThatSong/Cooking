using Cooking.Commands;
using Cooking.DTO;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cooking.Pages.Ingredients
{
    public partial class IngredientEditViewModel : INotifyPropertyChanged
    {
        public bool DialogResultOk { get; set; }

        public IngredientEditViewModel(IngredientDTO category = null)
        {
            OkCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    DialogResultOk = true;
                    CloseCommand.Value.Execute();
                }));

            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

            Ingredient = category ?? new IngredientDTO();
        }

        public List<MeasureUnit> MeasurementUnits => MeasureUnit.AllValues;

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }
        
        public IngredientDTO Ingredient { get; set; }
    }
}