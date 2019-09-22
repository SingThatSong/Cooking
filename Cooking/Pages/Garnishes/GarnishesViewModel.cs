using Cooking.Commands;
using Cooking.DTO;
using Data.Model.Plan;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.Pages.Garnishes
{
    [AddINotifyPropertyChangedInterface]
    public partial class GarnishesViewModel
    {
        public ObservableCollection<GarnishDTO> Garnishes { get; }
        public bool IsEditing { get; set; }

        public DelegateCommand AddGarnishCommand { get; }
        public DelegateCommand<GarnishDTO> EditGarnishCommand { get; }
        public DelegateCommand<GarnishDTO> DeleteGarnishCommand { get; }

        public GarnishesViewModel()
        {
            Garnishes = GarnishService.GetGarnishes<ServiceLayer.GarnishDTO>()
                                      .MapTo<ObservableCollection<GarnishDTO>>();

            AddGarnishCommand = new DelegateCommand(AddGarnish);
            DeleteGarnishCommand = new DelegateCommand<GarnishDTO>(cat => DeleteGarnish(cat.ID));
            EditGarnishCommand = new DelegateCommand<GarnishDTO>(EditGarnish);
        }

        private async void EditGarnish(GarnishDTO garnish)
        {
            var viewModel = new GarnishEditViewModel(garnish.MapTo<GarnishDTO>());
            await new DialogUtils(this).ShowCustomMessageAsync<GarnishEditView, GarnishEditViewModel>("Редактирование гарнира", viewModel);

            if (viewModel.DialogResultOk)
            {
                await GarnishService.UpdateGarnishAsync(viewModel.Garnish.MapTo<Garnish>());
                var existingRecipe = Garnishes.Single(x => x.ID == garnish.ID);
                viewModel.Garnish.MapTo(existingRecipe);
            }
        }

        public async void DeleteGarnish(Guid recipeId)
        {
            var result = await DialogCoordinator.Instance.ShowMessageAsync(
                this, 
                "Точно удалить?",
                "Восстановить будет нельзя",
                style: MessageDialogStyle.AffirmativeAndNegative,
                settings: new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Да",
                    NegativeButtonText = "Нет"
                });

            if (result == MessageDialogResult.Affirmative)
            {
                await GarnishService.DeleteGarnishAsync(recipeId);
                Garnishes.Remove(Garnishes.Single(x => x.ID == recipeId));
            }
        }

        public async void AddGarnish()
        {
            var viewModel = await new DialogUtils(this).ShowCustomMessageAsync<GarnishEditView, GarnishEditViewModel>("Новый гарнир");

            if (viewModel.DialogResultOk)
            {
                var id = await GarnishService.CreateGarnishAsync(viewModel.Garnish.MapTo<Garnish>());
                viewModel.Garnish.ID = id;
                Garnishes.Add(viewModel.Garnish);
            }
        }
    }
}