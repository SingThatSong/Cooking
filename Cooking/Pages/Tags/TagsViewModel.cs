using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Tags;
using Cooking.ServiceLayer;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using Prism.Regions;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages
{
    [AddINotifyPropertyChangedInterface]
    public partial class TagsViewModel
    {
        private readonly IRegionManager regionManager;
        private readonly DialogService dialogUtils;
        private readonly TagService tagService;
        private readonly IMapper mapper;

        public ObservableCollection<TagEdit>? Tags { get; private set; }
        public bool IsEditing { get; set; }

        public DelegateCommand AddTagCommand { get; }
        public DelegateCommand<TagEdit> ViewTagCommand { get; }
        public AsyncDelegateCommand<TagEdit> EditTagCommand { get; }
        public DelegateCommand<Guid> DeleteTagCommand { get; }
        public DelegateCommand LoadedCommand { get; }

        public TagsViewModel(IRegionManager regionManager, DialogService dialogUtils, TagService tagService, IMapper mapper)
        {
            Debug.Assert(regionManager != null);
            Debug.Assert(dialogUtils != null);
            Debug.Assert(tagService != null);
            Debug.Assert(mapper != null);

            this.regionManager = regionManager;
            this.dialogUtils = dialogUtils;
            this.tagService = tagService;
            this.mapper = mapper;
            LoadedCommand = new DelegateCommand(OnLoaded, executeOnce: true);
            AddTagCommand = new DelegateCommand(AddTag);
            DeleteTagCommand = new DelegateCommand<Guid>(DeleteTag);
            ViewTagCommand = new DelegateCommand<TagEdit>(ViewTag);
            EditTagCommand = new AsyncDelegateCommand<TagEdit>(EditTag);
        }

        private void OnLoaded()
        {
            Debug.WriteLine("TagsViewModel.OnLoaded");
            var dbVals = tagService.GetProjected<TagEdit>(mapper);
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
            var viewModel = new TagEditViewModel(dialogUtils, tagService, mapper.Map<TagEdit>(tag));
            await dialogUtils.ShowCustomMessageAsync<TagEditView, TagEditViewModel>("Редактирование тега", viewModel).ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                await tagService.UpdateAsync(mapper.Map<Tag>(viewModel.Tag)).ConfigureAwait(false);
                var existingTag = Tags.Single(x => x.ID == tag.ID);
                mapper.Map(viewModel.Tag, existingTag);
            }
        }


        public async void DeleteTag(Guid recipeId)
        {
            await dialogUtils.ShowYesNoDialog(
                  "Точно удалить?",
                  "Восстановить будет нельзя",
                  successCallback: () => OnTagDeleted(recipeId)).ConfigureAwait(false);
        }

        private async void OnTagDeleted(Guid recipeId)
        {
            await tagService.DeleteAsync(recipeId).ConfigureAwait(true);
            Tags!.Remove(Tags.Single(x => x.ID == recipeId));
        }

        public async void AddTag()
        {
            var viewModel = await dialogUtils.ShowCustomMessageAsync<TagEditView, TagEditViewModel>("Новый тег").ConfigureAwait(false);

            if (viewModel.DialogResultOk)
            {
                var id = await tagService.CreateAsync(mapper.Map<Tag>(viewModel.Tag)).ConfigureAwait(false);
                viewModel.Tag.ID = id;
                Tags!.Add(viewModel.Tag);
            }
        }

    }
}