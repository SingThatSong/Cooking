using AutoMapper;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Views;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.WPF.ViewModels
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
            AddGarnishCommand = new AsyncDelegateCommand(AddGarnishAsync);
            DeleteGarnishCommand = new AsyncDelegateCommand<Guid>(DeleteGarnishAsync);
            EditGarnishCommand = new AsyncDelegateCommand<GarnishEdit>(EditGarnishAsync);
        }

        /// <summary>
        /// Gets list of all garnishes.
        /// </summary>
        public ObservableCollection<GarnishEdit>? Garnishes { get; private set; }

        /// <summary>
        /// Gets command to execute on loaded event.
        /// </summary>
        public AsyncDelegateCommand LoadedCommand { get; }

        /// <summary>
        /// Gets command to add new garnish.
        /// </summary>
        public AsyncDelegateCommand AddGarnishCommand { get; }

        /// <summary>
        /// Gets command to edit garnish.
        /// </summary>
        public AsyncDelegateCommand<GarnishEdit> EditGarnishCommand { get; }

        /// <summary>
        /// Gets command to remove garnish.
        /// </summary>
        public AsyncDelegateCommand<Guid> DeleteGarnishCommand { get; }

        private async Task DeleteGarnishAsync(Guid garnishID) => await dialogService.ShowYesNoDialogAsync(localization.GetLocalizedString(
                                                                                                               "SureDelete",
                                                                                                               Garnishes?.Single(x => x.ID == garnishID).Name ?? string.Empty
                                                                                                          ),
                                                                                                          localization.GetLocalizedString("CannotUndo"),
                                                                                                          successCallback: async () => await OnDeletedAsync(garnishID));

        private async Task AddGarnishAsync() => await dialogService.ShowOkCancelDialogAsync<GarnishEditView, GarnishEditViewModel>(localization.GetLocalizedString("NewGarnish"),
                                                                                                                                   successCallback: async viewModel => await OnNewGarnishCreatedAsync(viewModel));

        private async Task EditGarnishAsync(GarnishEdit garnish)
        {
            var viewModel = new GarnishEditViewModel(mapper.Map<GarnishEdit>(garnish), garnishService, dialogService, localization);
            await dialogService.ShowOkCancelDialogAsync<GarnishEditView, GarnishEditViewModel>(localization.GetLocalizedString("EditGarnish"), viewModel, successCallback: async viewModel => await OnGarnishEditedAsync(viewModel));
        }

        private Task OnLoaded()
        {
            Debug.WriteLine("GarnishesViewModel.OnLoaded");
            List<GarnishEdit> dbValues = garnishService.GetAllProjected<GarnishEdit>();
            Garnishes = new ObservableCollection<GarnishEdit>(dbValues);

            return Task.CompletedTask;
        }

        private async Task OnDeletedAsync(Guid garnishID)
        {
            await garnishService.DeleteAsync(garnishID).ConfigureAwait(true);
            Garnishes!.Remove(Garnishes.Single(x => x.ID == garnishID));
        }

        private async Task OnGarnishEditedAsync(GarnishEditViewModel viewModel)
        {
            await garnishService.UpdateAsync(viewModel.Garnish);
            GarnishEdit? existingGarnish = Garnishes?.Single(x => x.ID == viewModel.Garnish.ID);
            if (existingGarnish != null)
            {
                mapper.Map(viewModel.Garnish, existingGarnish);
            }
        }

        private async Task OnNewGarnishCreatedAsync(GarnishEditViewModel viewModel)
        {
            await garnishService.CreateAsync(viewModel.Garnish);
            Garnishes!.Add(viewModel.Garnish);
        }
    }
}