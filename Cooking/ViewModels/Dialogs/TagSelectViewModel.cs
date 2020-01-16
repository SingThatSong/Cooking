using AutoMapper;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Views;
using Cooking.ServiceLayer;
using Cooking.WPF.Helpers;
using Cooking.Data.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Cooking.WPF.Views
{
    public partial class TagSelectViewModel : OkCancelViewModel
    {
        private readonly DialogService dialogUtils;
        private readonly TagService tagService;
        private readonly IMapper mapper;
        private readonly ILocalization localization;

        public TagSelectViewModel(DialogService dialogUtils, TagService tagService, IMapper mapper, ILocalization localization) : base(dialogUtils)
        {
            Debug.Assert(dialogUtils != null);
            Debug.Assert(tagService != null);
            Debug.Assert(mapper != null);
            Debug.Assert(localization != null);

            this.dialogUtils = dialogUtils;
            this.tagService = tagService;
            this.mapper = mapper;
            this.localization = localization;
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
                IEnumerable<TagEdit> tagsSelected = AllTags.Where(x => currentTags.Any(ct => ct.ID == x.ID));

                foreach (TagEdit tag in tagsSelected)
                {
                    tag.IsChecked = true;
                }
            }
        }

        public async void AddTag()
        {
            TagEditViewModel viewModel = await dialogUtils.ShowCustomMessageAsync<TagEditView, TagEditViewModel>(localization.GetLocalizedString("NewTag")).ConfigureAwait(false);
            
            if (viewModel.DialogResultOk)
            {
                Guid id = await tagService.CreateAsync(mapper.Map<Tag>(viewModel.Tag)).ConfigureAwait(false);
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