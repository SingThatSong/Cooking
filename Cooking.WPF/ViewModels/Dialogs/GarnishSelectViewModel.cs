using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for tag selection.
    /// </summary>
    public partial class GarnishSelectViewModel : OkCancelViewModel
    {
        private readonly GarnishService garnishService;
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="GarnishSelectViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="garnishService">Tag service dependency.</param>
        /// <param name="localization">Localization provider dependency.</param>
        /// <param name="selectedGarnishes">Loc25 provider dependency.</param>
        public GarnishSelectViewModel(DialogService dialogService,
                                      GarnishService garnishService,
                                      ILocalization localization,
                                      IEnumerable<GarnishEdit> selectedGarnishes)
            : base(dialogService)
        {
            this.garnishService = garnishService;
            this.localization = localization;
            AddGarnishCommand = new DelegateCommand(AddGarnishAsync);
            AllGarnishes = garnishService.GetAllProjected<GarnishEdit>();
            SelectedItems.AddRange(AllGarnishes.Intersect(selectedGarnishes));
        }

        /// <summary>
        /// Gets command for adding a tag.
        /// </summary>
        public DelegateCommand AddGarnishCommand { get; }

        /// <summary>
        /// Gets all tags to choose from.
        /// </summary>
        public List<GarnishEdit> AllGarnishes { get; private set; }

        /// <summary>
        /// Gets or sets all selected tags.
        /// </summary>
        public ObservableCollection<GarnishEdit> SelectedItems { get; set; } = new ObservableCollection<GarnishEdit>();

        /// <summary>
        /// Add new tag.
        /// </summary>
        public async void AddGarnishAsync()
        {
            GarnishEditViewModel viewModel = await DialogService.ShowCustomMessageAsync<GarnishEditView, GarnishEditViewModel>(localization.GetLocalizedString("NewGarnish"));

            if (viewModel.DialogResultOk)
            {
                viewModel.Garnish.ID = await garnishService.CreateAsync(viewModel.Garnish);
                AllGarnishes.Add(viewModel.Garnish);
            }
        }
    }
}