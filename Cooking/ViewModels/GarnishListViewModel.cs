using AutoMapper;
using Cooking.Data.Model.Plan;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Helpers;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.WPF.Views
{
    [AddINotifyPropertyChangedInterface]
    public partial class GarnishListViewModel
    {
        // Dependencies
        private readonly DialogService dialogUtils;
        private readonly GarnishService garnishService;
        private readonly IMapper mapper;
        private readonly ILocalization localization;

        // State
        public ObservableCollection<GarnishEdit>? Garnishes { get; private set; }
        public bool IsEditing { get; set; }

        // Commands
        public AsyncDelegateCommand LoadedCommand { get; }
        public DelegateCommand AddGarnishCommand { get; }
        public DelegateCommand<GarnishEdit> EditGarnishCommand { get; }
        public DelegateCommand<Guid> DeleteGarnishCommand { get; }

        public GarnishListViewModel(DialogService dialogUtils, GarnishService garnishService, IMapper mapper, ILocalization localization)
        {
            this.dialogUtils = dialogUtils;
            this.garnishService = garnishService;
            this.mapper = mapper;
            this.localization = localization;
            LoadedCommand = new AsyncDelegateCommand(OnLoaded, executeOnce: true);
            AddGarnishCommand = new DelegateCommand(AddGarnish);
            DeleteGarnishCommand = new DelegateCommand<Guid>(DeleteGarnish);
            EditGarnishCommand = new DelegateCommand<GarnishEdit>(EditGarnish);
        }

        private Task OnLoaded()
        {
            Debug.WriteLine("GarnishesViewModel.OnLoaded");
            List<GarnishEdit> dbValues = garnishService.GetProjected<GarnishEdit>(mapper);
            Garnishes = new ObservableCollection<GarnishEdit>(dbValues);

            return Task.CompletedTask;
        }

        public async void DeleteGarnish(Guid recipeId) => await dialogUtils.ShowYesNoDialog(localization.GetLocalizedString("SureDelete"),
                                                                                            localization.GetLocalizedString("CannotUndo"),
                                                                                            successCallback: () => OnRecipeDeleted(recipeId))
                                                                            ;

        public async void AddGarnish() => await dialogUtils.ShowOkCancelDialog<GarnishEditView, GarnishEditViewModel>(localization.GetLocalizedString("NewGarnish"),
                                                                                                                      successCallback: OnNewGarnishCreated)
                                                           ;

        public async void EditGarnish(GarnishEdit garnish)
        {
            var viewModel = new GarnishEditViewModel(mapper.Map<GarnishEdit>(garnish), garnishService, dialogUtils, localization);
            await dialogUtils.ShowOkCancelDialog<GarnishEditView, GarnishEditViewModel>(localization.GetLocalizedString("EditGarnish"), viewModel, successCallback: OnGarnishEdited)
                             ;
        }

        private async void OnRecipeDeleted(Guid recipeId)
        {
            await garnishService.DeleteAsync(recipeId).ConfigureAwait(true);
            Garnishes!.Remove(Garnishes.Single(x => x.ID == recipeId));
        }

        private async void OnGarnishEdited(GarnishEditViewModel viewModel)
        {
            await garnishService.UpdateAsync(mapper.Map<Garnish>(viewModel.Garnish));
            GarnishEdit existingGarnish = Garnishes.Single(x => x.ID == viewModel.Garnish.ID);
            mapper.Map(viewModel.Garnish, existingGarnish);
        }

        private async void OnNewGarnishCreated(GarnishEditViewModel viewModel)
        {
            await garnishService.CreateAsync(mapper.Map<Garnish>(viewModel.Garnish));
            Garnishes!.Add(viewModel.Garnish);
        }
    }
}