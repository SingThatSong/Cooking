using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Tags;
using Cooking.ServiceLayer;
using Data.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Cooking.Pages
{
    public partial class TagSelectViewModel : OkCancelViewModel
    {
        private readonly DialogService dialogUtils;
        private readonly TagService tagService;
        private readonly IMapper mapper;

        public TagSelectViewModel(DialogService dialogUtils, TagService tagService, IMapper mapper) : base(dialogUtils)
        {
            Debug.Assert(dialogUtils != null);
            Debug.Assert(tagService != null);
            Debug.Assert(mapper != null);

            this.dialogUtils = dialogUtils;
            this.tagService = tagService;
            this.mapper = mapper;

            AddTagCommand = new DelegateCommand(AddTag);
        }

        public void SetTags(IEnumerable<TagEdit>? currentTags, IEnumerable<TagEdit>? allTags)
        {
            if (allTags == null)
            {
                AllTags = new ObservableCollection<TagEdit>(tagService.GetProjected<TagEdit>(mapper));
            }
            else
            {
                AllTags = new ObservableCollection<TagEdit>(allTags);
            }

            if (currentTags != null)
            {
                var tagsSelected = AllTags.Where(x => currentTags.Any(ct => ct.ID == x.ID));

                foreach (var tag in tagsSelected)
                {
                    tag.IsChecked = true;
                }
            }
        }

        public async void AddTag()
        {
            var viewModel = await dialogUtils.ShowCustomMessageAsync<TagEditView, TagEditViewModel>("Новый тег").ConfigureAwait(false);
            
            if (viewModel.DialogResultOk)
            {
                var id = await tagService.CreateAsync(mapper.Map<Tag>(viewModel.Tag)).ConfigureAwait(false);
                viewModel.Tag.ID = id;
                AllTags?.Add(viewModel.Tag);
            }
        }

        public ReadOnlyCollection<MeasureUnit> MeasurementUnits => MeasureUnit.AllValues;

        public DelegateCommand AddTagCommand { get; }

        public ObservableCollection<TagEdit>? AllTags { get; private set; }

        public IEnumerable<TagEdit>? MainIngredients => AllTags?.Where(x => x.Type == TagType.MainIngredient);
        public IEnumerable<TagEdit>? DishTypes => AllTags?.Where(x => x.Type == TagType.DishType);
        public IEnumerable<TagEdit>? Occasions => AllTags?.Where(x => x.Type == TagType.Occasion);
        public IEnumerable<TagEdit>? Sources => AllTags?.Where(x => x.Type == TagType.Source);

    }
}