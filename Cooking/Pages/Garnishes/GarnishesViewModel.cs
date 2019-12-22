using AutoMapper;
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
        private readonly DialogService dialogUtils;
        private readonly GarnishService garnishService;
        private readonly IMapper mapper;

        [SuppressMessage("Usage", "CA2227:Свойства коллекций должны быть доступны только для чтения", Justification = "<Ожидание>")]
        public ObservableCollection<GarnishEdit>? Garnishes { get; set; }
        public bool IsEditing { get; set; }

        public DelegateCommand AddGarnishCommand { get; }
        public DelegateCommand<GarnishEdit> EditGarnishCommand { get; }
        public DelegateCommand<Guid> DeleteGarnishCommand { get; }
        public DelegateCommand LoadedCommand { get; }

        public GarnishesViewModel(DialogService dialogUtils, GarnishService garnishService, IMapper mapper)
        {
            Debug.Assert(dialogUtils != null);
            Debug.Assert(garnishService != null);
            Debug.Assert(mapper != null);

            this.dialogUtils     = dialogUtils;
            this.garnishService  = garnishService;
            this.mapper          = mapper;
            LoadedCommand        = new DelegateCommand(OnLoaded, executeOnce: true);
            AddGarnishCommand    = new DelegateCommand(AddGarnish);
            DeleteGarnishCommand = new DelegateCommand<Guid>(DeleteGarnish);
            EditGarnishCommand   = new DelegateCommand<GarnishEdit>(EditGarnish);
        }

        private void OnLoaded()
        {
            Debug.WriteLine("GarnishesViewModel.OnLoaded");
            var dbValues = garnishService.GetProjected<GarnishEdit>(mapper);
            Garnishes = new ObservableCollection<GarnishEdit>(dbValues);
        }

        private async void EditGarnish(GarnishEdit garnish)
        {
            var viewModel = new GarnishEditViewModel(mapper.Map<GarnishEdit>(garnish), garnishService, dialogUtils);
            await dialogUtils.ShowCustomMessageAsync<GarnishEditView, GarnishEditViewModel>("Редактирование гарнира", viewModel).ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                await garnishService.UpdateAsync(mapper.Map<Garnish>(viewModel.Garnish)).ConfigureAwait(false);
                var existingGarnish = Garnishes.Single(x => x.ID == garnish.ID);
                mapper.Map(viewModel.Garnish, existingGarnish);
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
                }).ConfigureAwait(true);

            if (result == MessageDialogResult.Affirmative)
            {
                await garnishService.DeleteAsync(recipeId).ConfigureAwait(true);
                Garnishes!.Remove(Garnishes.Single(x => x.ID == recipeId));
            }
        }

        public async void AddGarnish()
        {
            var viewModel = await dialogUtils.ShowCustomMessageAsync<GarnishEditView, GarnishEditViewModel>("Новый гарнир").ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                var id = await garnishService.CreateAsync(mapper.Map<Garnish>(viewModel.Garnish)).ConfigureAwait(true);
                viewModel.Garnish.ID = id;
                Garnishes!.Add(viewModel.Garnish);
            }
        }
    }
}