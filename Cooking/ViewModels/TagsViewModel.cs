using AutoMapper;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Views;
using Cooking.ServiceLayer;
using Cooking.WPF.Helpers;
using Cooking.Data.Model;
using Prism.Regions;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cooking.WPF.Views
{
    [AddINotifyPropertyChangedInterface]
    public partial class TagsViewModel
    {
        private readonly IRegionManager regionManager;
        private readonly DialogService dialogUtils;
        private readonly TagService tagService;
        private readonly IMapper mapper;
        private readonly ILocalization localization;

        public ObservableCollection<TagEdit>? Tags { get; private set; }
        public bool IsEditing { get; set; }

        public DelegateCommand AddTagCommand { get; }
        public DelegateCommand<TagEdit> ViewTagCommand { get; }
        public AsyncDelegateCommand<TagEdit> EditTagCommand { get; }
        public DelegateCommand<Guid> DeleteTagCommand { get; }
        public DelegateCommand LoadedCommand { get; }

        public TagsViewModel(IRegionManager regionManager, DialogService dialogUtils, TagService tagService, IMapper mapper, ILocalization localization)
        {
            this.regionManager = regionManager;
            this.dialogUtils = dialogUtils;
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
            List<TagEdit> dbVals = tagService.GetProjected<TagEdit>(mapper);
            Tags = new ObservableCollection<TagEdit>(dbVals);
        }

        private void ViewTag(TagEdit tag)
        {
            var parameters = new NavigationParameters()
            {
                { nameof(RecepiesViewModel.FilterText), $"{Consts.TagSymbol}\"{tag.Name}\"" }
            };
            regionManager.RequestNavigate(Consts.MainContentRegion, nameof(Recepies), parameters);
        }

        private async Task EditTag(TagEdit tag)
        {
            var viewModel = new TagEditViewModel(dialogUtils, tagService, localization, mapper.Map<TagEdit>(tag));
            await dialogUtils.ShowCustomMessageAsync<TagEditView, TagEditViewModel>(localization.GetLocalizedString("EditTag"), viewModel).ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                await tagService.UpdateAsync(mapper.Map<Tag>(viewModel.Tag)).ConfigureAwait(false);
                TagEdit existingTag = Tags.Single(x => x.ID == tag.ID);
                mapper.Map(viewModel.Tag, existingTag);
            }
        }


        public async void DeleteTag(Guid recipeId) => await dialogUtils.ShowYesNoDialog(localization.GetLocalizedString("SureDelete"),
                                                                                        localization.GetLocalizedString("CannotUndo"),
                                                                                        successCallback: () => OnTagDeleted(recipeId))
                                                                       .ConfigureAwait(false);

        private async void OnTagDeleted(Guid recipeId)
        {
            await tagService.DeleteAsync(recipeId).ConfigureAwait(true);
            Tags!.Remove(Tags.Single(x => x.ID == recipeId));
        }

        public async void AddTag()
        {
            TagEditViewModel viewModel = await dialogUtils.ShowCustomMessageAsync<TagEditView, TagEditViewModel>(localization.GetLocalizedString("NewTag")).ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                Guid id = await tagService.CreateAsync(mapper.Map<Tag>(viewModel.Tag)).ConfigureAwait(false);
                viewModel.Tag.ID = id;
                Tags!.Add(viewModel.Tag);
            }
        }

    }
}