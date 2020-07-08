using AutoMapper;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using NullGuard;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.WPF.Views
{
    /// <summary>
    /// View model for tag selection.
    /// </summary>
    public partial class TagSelectViewModel : OkCancelViewModel
    {
        private readonly TagService tagService;
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagSelectViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="tagService">Tag service dependency.</param>
        /// <param name="localization">Localization provider dependency.</param>
        /// <param name="measureUnitService">MeasureUnitService provider dependency.</param>
        public TagSelectViewModel(DialogService dialogService, TagService tagService, ILocalization localization, MeasureUnitService measureUnitService)
            : base(dialogService)
        {
            this.tagService = tagService;
            this.localization = localization;
            MeasurementUnits = measureUnitService.GetAll();
            AddTagCommand = new DelegateCommand(AddTag);
        }

        /// <summary>
        /// Gets all measurement units to select from.
        /// </summary>
        public List<MeasureUnit> MeasurementUnits { get; }

        /// <summary>
        /// Gets command for adding a tag.
        /// </summary>
        public DelegateCommand AddTagCommand { get; }

        /// <summary>
        /// Gets all tags to choose from.
        /// </summary>
        public ObservableCollection<TagEdit>? AllTags { get; private set; }

        /// <summary>
        /// Gets main ingredient tags.
        /// </summary>
        public IEnumerable<TagEdit>? MainIngredients => AllTags?.Where(x => x.Type == TagType.MainIngredient);

        /// <summary>
        /// Gets dish type tags.
        /// </summary>
        public IEnumerable<TagEdit>? DishTypes => AllTags?.Where(x => x.Type == TagType.DishType);

        /// <summary>
        /// Gets occasion tags.
        /// </summary>
        public IEnumerable<TagEdit>? Occasions => AllTags?.Where(x => x.Type == TagType.Occasion);

        /// <summary>
        /// Gets sources tags.
        /// </summary>
        public IEnumerable<TagEdit>? Sources => AllTags?.Where(x => x.Type == TagType.Source);

        /// <summary>
        /// Pass data parameters so we can use IoC in constructor.
        /// </summary>
        /// <param name="currentTags">Alredy existing tags for editing.</param>
        /// <param name="allTags">All tags to select from.</param>
        public void SetTags(IEnumerable<TagEdit>? currentTags, IEnumerable<TagEdit>? allTags)
        {
            AllTags = new ObservableCollection<TagEdit>(allTags ?? tagService.GetAllProjected<TagEdit>());

            AllTags.CollectionChanged += AllTags_CollectionChanged;

            if (currentTags != null)
            {
                IEnumerable<TagEdit> tagsSelected = AllTags.Where(x => currentTags.Any(ct => ct.ID == x.ID));

                foreach (TagEdit tag in tagsSelected)
                {
                    tag.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// Add new tag.
        /// </summary>
        public async void AddTag()
        {
            TagEditViewModel viewModel = await DialogService.ShowCustomMessageAsync<TagEditView, TagEditViewModel>(localization.GetLocalizedString("NewTag"));

            if (viewModel.DialogResultOk)
            {
                Guid id = await tagService.CreateAsync(viewModel.Tag);
                viewModel.Tag.ID = id;
                AllTags?.Add(viewModel.Tag);
            }
        }

        private void AllTags_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(MainIngredients));
            OnPropertyChanged(nameof(DishTypes));
            OnPropertyChanged(nameof(Occasions));
            OnPropertyChanged(nameof(Sources));
        }
    }
}