using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.MainPage;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.ComponentModel;

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
            LastCooked = new Lazy<string>(() =>
            {
                var daysFromLastCook = new LastDayCooked().DaysFromLasCook(Recipe.ID);
                return daysFromLastCook == int.MaxValue ? "Новый рецепт" : $"Дней с последнего приготовления: {daysFromLastCook}";
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> CloseCommand { get; }
        
        public RecipeDTO Recipe { get; set; }

        public Lazy<string> LastCooked { get; }
    }
}