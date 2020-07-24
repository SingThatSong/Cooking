using AutoMapper;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Cooking.WPF.Views;
using Prism.Regions;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for a list of tags.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class TagListViewModel
    {
        private readonly IRegionManager regionManager;
        private readonly DialogService dialogService;
        private readonly TagService tagService;
        private readonly IMapper mapper;
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagListViewModel"/> class.
        /// </summary>
        /// <param name="regionManager">Region manager for Prism navigation.</param>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="tagService">Tag service dependency.</param>
        /// <param name="mapper">Mapper dependency.</param>
        /// <param name="localization">Localization provider dependency.</param>
        public TagListViewModel(IRegionManager regionManager, DialogService dialogService, TagService tagService, IMapper mapper, ILocalization localization)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            this.tagService = tagService;
            this.mapper = mapper;
            this.localization = localization;
            LoadedCommand = new AsyncDelegateCommand(OnLoaded, executeOnce: true);
            AddTagCommand = new AsyncDelegateCommand(AddTagAsync);
            DeleteTagCommand = new AsyncDelegateCommand<Guid>(DeleteTagAsync);
            ViewTagCommand = new DelegateCommand<TagEdit>(ViewTag);
            EditTagCommand = new AsyncDelegateCommand<TagEdit>(EditTagAsync);
        }

        /// <summary>
        /// Gets all tags list.
        /// </summary>
        public ObservableCollection<TagEdit>? Tags { get; private set; }

        /// <summary>
        /// Gets command to create a tag.
        /// </summary>
        public AsyncDelegateCommand AddTagCommand { get; }

        /// <summary>
        /// Gets command to move to recipies list filtered by selected tag.
        /// </summary>
        public DelegateCommand<TagEdit> ViewTagCommand { get; }

        /// <summary>
        /// Gets command to edit a tag.
        /// </summary>
        public AsyncDelegateCommand<TagEdit> EditTagCommand { get; }

        /// <summary>
        /// Gets command to delete a tag.
        /// </summary>
        public AsyncDelegateCommand<Guid> DeleteTagCommand { get; }

        /// <summary>
        /// Gets command to execute on loaded event.
        /// </summary>
        public AsyncDelegateCommand LoadedCommand { get; }

        /// <summary>
        /// Gets localized string for EditTag.
        /// </summary>
        public string? EditTagCaption => localization.GetLocalizedString("EditTag");

        /// <summary>
        /// Gets localized string for Name.
        /// </summary>
        public string? NameCaption => localization.GetLocalizedString("Name");

        private Task OnLoaded()
        {
            Debug.WriteLine("TagsViewModel.OnLoaded");
            List<TagEdit> dbVals = tagService.GetAllProjected<TagEdit>();
            Tags = new ObservableCollection<TagEdit>(dbVals);

            return Task.CompletedTask;
        }

        private void ViewTag(TagEdit tag)
        {
            regionManager.NavigateMain(
                 view: nameof(RecipeListView),
                 parameters: (nameof(RecipeListViewModel.FilterText), $"{Consts.TagSymbol}\"{tag.Name}\""));
        }

        private async Task EditTagAsync(TagEdit tag)
        {
            var viewModel = new TagEditViewModel(dialogService, tagService, localization, mapper.Map<TagEdit>(tag));
            await dialogService.ShowCustomMessageAsync<TagEditView, TagEditViewModel>(localization.GetLocalizedString("EditTag"), viewModel);

            if (viewModel.DialogResultOk)
            {
                await tagService.UpdateAsync(viewModel.Tag);
                TagEdit? existingTag = Tags?.Single(x => x.ID == tag.ID);
                if (existingTag != null)
                {
                    mapper.Map(viewModel.Tag, existingTag);
                }
            }
        }

        private async Task DeleteTagAsync(Guid recipeID) => await dialogService.ShowYesNoDialogAsync(localization.GetLocalizedString("SureDelete", Tags!.Single(x => x.ID == recipeID).Name ?? string.Empty),
                                                                                                     localization.GetLocalizedString("CannotUndo"),
                                                                                                     successCallback: async () => await OnTagDeletedAsync(recipeID));

        private async Task OnTagDeletedAsync(Guid recipeID)
        {
            await tagService.DeleteAsync(recipeID).ConfigureAwait(true);
            Tags!.Remove(Tags.Single(x => x.ID == recipeID));
        }

        private async Task AddTagAsync()
        {
            TagEditViewModel viewModel = await dialogService.ShowCustomMessageAsync<TagEditView, TagEditViewModel>(localization.GetLocalizedString("NewTag")).ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                Guid id = await tagService.CreateAsync(viewModel.Tag).ConfigureAwait(true);
                viewModel.Tag.ID = id;
                Tags!.Add(viewModel.Tag);
            }
        }
    }
}