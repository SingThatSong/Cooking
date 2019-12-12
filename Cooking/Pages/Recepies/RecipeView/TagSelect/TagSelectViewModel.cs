using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Tags;
using Data.Model;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.Pages
{
    public partial class TagSelectViewModel : OkCancelViewModel
    {
        private readonly DialogUtils dialogUtils;

        public TagSelectViewModel()
        {
            throw new NotImplementedException();
        }

        public TagSelectViewModel(IEnumerable<TagEdit>? currentTags, DialogUtils dialogUtils)
        {
            this.dialogUtils = dialogUtils;
            AddTagCommand = new DelegateCommand(AddTag);
            AllTags = new ObservableCollection<TagEdit>(TagService.GetTags().Select(x => MapperService.Mapper.Map<TagEdit>(x)));
            CtorInternal(currentTags);
        }

        public TagSelectViewModel(IEnumerable<TagEdit>? currentTags, IEnumerable<TagEdit> allTags, DialogUtils dialogUtils)
        {
            this.dialogUtils = dialogUtils;
            AddTagCommand = new DelegateCommand(AddTag);
            AllTags = new ObservableCollection<TagEdit>(allTags);
            CtorInternal(currentTags);
        }

        private void CtorInternal(IEnumerable<TagEdit>? currentTags)
        {
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
                var id = await TagService.CreateAsync(viewModel.Tag.MapTo<Tag>()).ConfigureAwait(false);
                viewModel.Tag.ID = id;
                AllTags.Add(viewModel.Tag);
            }
        }

        public ReadOnlyCollection<MeasureUnit> MeasurementUnits => MeasureUnit.AllValues;

        public DelegateCommand AddTagCommand { get; }

        public ObservableCollection<TagEdit> AllTags { get; }

        public IEnumerable<TagEdit> MainIngredients => AllTags.Where(x => x.Type == TagType.MainIngredient);
        public IEnumerable<TagEdit> DishTypes => AllTags.Where(x => x.Type == TagType.DishType);
        public IEnumerable<TagEdit> Occasions => AllTags.Where(x => x.Type == TagType.Occasion);
        public IEnumerable<TagEdit> Sources => AllTags.Where(x => x.Type == TagType.Source);

    }
}