using Cooking.Commands;
using Cooking.DTO;

using Data.Model.Plan;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public partial class GarnishesViewModel
    {
        [SuppressMessage("Usage", "CA2227:Свойства коллекций должны быть доступны только для чтения", Justification = "<Ожидание>")]
        public ObservableCollection<GarnishEdit>? Garnishes { get; set; }
        public bool IsEditing { get; set; }

        public DelegateCommand AddGarnishCommand { get; }
        public DelegateCommand<GarnishEdit> EditGarnishCommand { get; }
        public DelegateCommand<Guid> DeleteGarnishCommand { get; }
        public DelegateCommand LoadedCommand { get; }

        public GarnishesViewModel()
        {
            LoadedCommand = new DelegateCommand(OnLoaded, executeOnce: true);
            AddGarnishCommand = new DelegateCommand(AddGarnish);
            DeleteGarnishCommand = new DelegateCommand<Guid>(DeleteGarnish);
            EditGarnishCommand = new DelegateCommand<GarnishEdit>(EditGarnish);
        }

        private void OnLoaded()
        {
            Debug.WriteLine("GarnishesViewModel.OnLoaded");
            Garnishes = GarnishService.GetGarnishes<ServiceLayer.GarnishDTO>()
                                      .MapTo<ObservableCollection<GarnishEdit>>();
        }

        private async void EditGarnish(GarnishEdit garnish)
        {
            var viewModel = new GarnishEditViewModel(garnish.MapTo<GarnishEdit>());
            await new DialogUtils(this).ShowCustomMessageAsync<GarnishEditView, GarnishEditViewModel>("Редактирование гарнира", viewModel).ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                await GarnishService.UpdateGarnishAsync(viewModel.Garnish.MapTo<Garnish>()).ConfigureAwait(false);
                var existingGarnish = Garnishes.Single(x => x.ID == garnish.ID);
                viewModel.Garnish.MapTo(existingGarnish);
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
                }).ConfigureAwait(false);

            if (result == MessageDialogResult.Affirmative)
            {
                await GarnishService.DeleteAsync(recipeId).ConfigureAwait(false);
                Garnishes!.Remove(Garnishes.Single(x => x.ID == recipeId));
            }
        }

        public async void AddGarnish()
        {
            var viewModel = await new DialogUtils(this).ShowCustomMessageAsync<GarnishEditView, GarnishEditViewModel>("Новый гарнир").ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                var id = await GarnishService.CreateGarnishAsync(viewModel.Garnish.MapTo<Garnish>()).ConfigureAwait(false);
                viewModel.Garnish.ID = id;
                Garnishes!.Add(viewModel.Garnish);
            }
        }
    }
}