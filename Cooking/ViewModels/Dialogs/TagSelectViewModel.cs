using AutoMapper;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Helpers;
using NullGuard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.WPF.Views
{
    public partial class TagSelectViewModel : OkCancelViewModel
    {
        private readonly DialogService dialogUtils;
        private readonly TagService tagService;
        private readonly IMapper mapper;
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagSelectViewModel"/> class.
        /// </summary>
        /// <param name="dialogUtils"></param>
        /// <param name="tagService"></param>
        /// <param name="mapper"></param>
        /// <param name="localization"></param>
        public TagSelectViewModel(DialogService dialogUtils, TagService tagService, IMapper mapper, ILocalization localization)
            : base(dialogUtils)
        {
            this.dialogUtils = dialogUtils;
            this.tagService = tagService;
            this.mapper = mapper;
            this.localization = localization;
            AddTagCommand = new DelegateCommand(AddTag);
        }

        public void SetTags([AllowNull] IEnumerable<TagEdit>? currentTags, [AllowNull] IEnumerable<TagEdit>? allTags)
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
            TagEditViewModel viewModel = await dialogUtils.ShowCustomMessageAsync<TagEditView, TagEditViewModel>(localization.GetLocalizedString("NewTag"));

            if (viewModel.DialogResultOk)
            {
                Guid id = await tagService.CreateAsync(mapper.Map<Tag>(viewModel.Tag));
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