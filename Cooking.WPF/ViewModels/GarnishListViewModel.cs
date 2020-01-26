using AutoMapper;
using Cooking.Data.Model.Plan;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
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
    /// <summary>
    /// View model for a list of all garnishes.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class GarnishListViewModel
    {
        // Dependencies
        private readonly DialogService dialogService;
        private readonly GarnishService garnishService;
        private readonly IMapper mapper;
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="GarnishListViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="garnishService">Garnish service dependency.</param>
        /// <param name="mapper">Mapper dependency.</param>
        /// <param name="localization">Localization provider dependency.</param>
        public GarnishListViewModel(DialogService dialogService, GarnishService garnishService, IMapper mapper, ILocalization localization)
        {
            this.dialogService = dialogService;
            this.garnishService = garnishService;
            this.mapper = mapper;
            this.localization = localization;
            LoadedCommand = new AsyncDelegateCommand(OnLoaded, executeOnce: true);
            AddGarnishCommand = new DelegateCommand(AddGarnish);
            DeleteGarnishCommand = new DelegateCommand<Guid>(DeleteGarnish);
            EditGarnishCommand = new DelegateCommand<GarnishEdit>(EditGarnish);
        }

        /// <summary>
        /// Gets list of all garnishes.
        /// </summary>
        public ObservableCollection<GarnishEdit>? Garnishes { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether current view model is in editing state.
        /// </summary>
        public bool IsEditing { get; set; }

        /// <summary>
        /// Gets command to execute on loaded event.
        /// </summary>
        public AsyncDelegateCommand LoadedCommand { get; }

        /// <summary>
        /// Gets command to add new garnish.
        /// </summary>
        public DelegateCommand AddGarnishCommand { get; }

        /// <summary>
        /// Gets command to edit garnish.
        /// </summary>
        public DelegateCommand<GarnishEdit> EditGarnishCommand { get; }

        /// <summary>
        /// Gets command to remove garnish.
        /// </summary>
        public DelegateCommand<Guid> DeleteGarnishCommand { get; }

        private async void DeleteGarnish(Guid garnishId) => await dialogService.ShowYesNoDialog(localization.GetLocalizedString(
                                                                                                    "SureDelete",
                                                                                                    Garnishes.Single(x => x.ID == garnishId).Name ?? string.Empty
                                                                                               ),
                                                                                               localization.GetLocalizedString("CannotUndo"),
                                                                                               successCallback: () => OnRecipeDeleted(garnishId));

        private async void AddGarnish() => await dialogService.ShowOkCancelDialog<GarnishEditView, GarnishEditViewModel>(localization.GetLocalizedString("NewGarnish"),
                                                                                                                      successCallback: OnNewGarnishCreated);

        private async void EditGarnish(GarnishEdit garnish)
        {
            var viewModel = new GarnishEditViewModel(mapper.Map<GarnishEdit>(garnish), garnishService, dialogService, localization);
            await dialogService.ShowOkCancelDialog<GarnishEditView, GarnishEditViewModel>(localization.GetLocalizedString("EditGarnish"), viewModel, successCallback: OnGarnishEdited);
        }

        private Task OnLoaded()
        {
            Debug.WriteLine("GarnishesViewModel.OnLoaded");
            List<GarnishEdit> dbValues = garnishService.GetAllProjected<GarnishEdit>(mapper);
            Garnishes = new ObservableCollection<GarnishEdit>(dbValues);

            return Task.CompletedTask;
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