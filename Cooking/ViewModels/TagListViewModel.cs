﻿using AutoMapper;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Prism.Regions;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.WPF.Views
{
    [AddINotifyPropertyChangedInterface]
    public partial class TagListViewModel
    {
        private readonly IRegionManager regionManager;
        private readonly DialogService dialogService;
        private readonly TagService tagService;
        private readonly IMapper mapper;
        private readonly ILocalization localization;

        public ObservableCollection<TagEdit>? Tags { get; private set; }
        public bool IsEditing { get; set; }

        public DelegateCommand AddTagCommand { get; }
        public DelegateCommand<TagEdit> ViewTagCommand { get; }
        public AsyncDelegateCommand<TagEdit> EditTagCommand { get; }
        public DelegateCommand<Guid> DeleteTagCommand { get; }
        /// <summary>
        /// Gets command to execute on loaded event.
        /// </summary>
        public DelegateCommand LoadedCommand { get; }

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
            LoadedCommand = new DelegateCommand(OnLoaded, executeOnce: true);
            AddTagCommand = new DelegateCommand(AddTag);
            DeleteTagCommand = new DelegateCommand<Guid>(DeleteTag);
            ViewTagCommand = new DelegateCommand<TagEdit>(ViewTag);
            EditTagCommand = new AsyncDelegateCommand<TagEdit>(EditTag);
        }

        private void OnLoaded()
        {
            Debug.WriteLine("TagsViewModel.OnLoaded");
            List<TagEdit> dbVals = tagService.GetAllProjected<TagEdit>(mapper);
            Tags = new ObservableCollection<TagEdit>(dbVals);
        }

        private void ViewTag(TagEdit tag)
        {
            var parameters = new NavigationParameters()
            {
                { nameof(RecipeListViewModel.FilterText), $"{Consts.TagSymbol}\"{tag.Name}\"" }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(RecipeListView), parameters);
        }

        private async Task EditTag(TagEdit tag)
        {
            var viewModel = new TagEditViewModel(dialogService, tagService, localization, mapper.Map<TagEdit>(tag));
            await dialogService.ShowCustomMessageAsync<TagEditView, TagEditViewModel>(localization.GetLocalizedString("EditTag"), viewModel);

            if (viewModel.DialogResultOk)
            {
                await tagService.UpdateAsync(mapper.Map<Tag>(viewModel.Tag));
                TagEdit existingTag = Tags.Single(x => x.ID == tag.ID);
                mapper.Map(viewModel.Tag, existingTag);
            }
        }

        public async void DeleteTag(Guid recipeId) => await dialogService.ShowYesNoDialog(localization.GetLocalizedString("SureDelete", Tags!.Single(x => x.ID == recipeId).Name ?? string.Empty),
                                                                                        localization.GetLocalizedString("CannotUndo"),
                                                                                        successCallback: () => OnTagDeleted(recipeId))
                                                                       ;

        private async void OnTagDeleted(Guid recipeId)
        {
            await tagService.DeleteAsync(recipeId).ConfigureAwait(true);
            Tags!.Remove(Tags.Single(x => x.ID == recipeId));
        }

        public async void AddTag()
        {
            TagEditViewModel viewModel = await dialogService.ShowCustomMessageAsync<TagEditView, TagEditViewModel>(localization.GetLocalizedString("NewTag")).ConfigureAwait(true);

            if (viewModel.DialogResultOk)
            {
                Guid id = await tagService.CreateAsync(mapper.Map<Tag>(viewModel.Tag)).ConfigureAwait(true);
                viewModel.Tag.ID = id;
                Tags!.Add(viewModel.Tag);
            }
        }
    }
}