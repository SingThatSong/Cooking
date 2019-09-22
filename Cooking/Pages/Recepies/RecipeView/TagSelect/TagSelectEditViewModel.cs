using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Tags;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Cooking.Pages.Recepies
{
    public partial class TagSelectEditViewModel : OkCancelViewModel
    {
        private readonly DialogUtils dialogUtils;

        public TagSelectEditViewModel()
        {
            throw new NotImplementedException();
        }

        public TagSelectEditViewModel(IEnumerable<TagDTO> currentTags, DialogUtils dialogUtils)
        {
            this.dialogUtils = dialogUtils;
            AddTagCommand = new DelegateCommand(AddTag);
            AllTags = new ObservableCollection<TagDTO>(TagService.GetTags().Select(x => MapperService.Mapper.Map<TagDTO>(x)));
            CtorInternal(currentTags);
        }

        public TagSelectEditViewModel(IEnumerable<TagDTO> currentTags, TagType filterTag, IEnumerable<TagDTO> allTags, DialogUtils dialogUtils)
        {
            this.dialogUtils = dialogUtils;
            AddTagCommand = new DelegateCommand(AddTag);
            AllTags = new ObservableCollection<TagDTO>(allTags);
            CtorInternal(currentTags);
        }

        private void CtorInternal(IEnumerable<TagDTO> currentTags)
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
            var viewModel = await dialogUtils.ShowCustomMessageAsync<TagEditView, TagEditViewModel>("Новый тег");
            
            if (viewModel.DialogResultOk)
            {
                var id = await TagService.CreateAsync(viewModel.Tag.MapTo<Tag>());
                viewModel.Tag.ID = id;
                AllTags.Add(viewModel.Tag);
            }
        }

        public ReadOnlyCollection<MeasureUnit> MeasurementUnits => MeasureUnit.AllValues;

        public DelegateCommand AddTagCommand { get; }

        public ObservableCollection<TagDTO> AllTags { get; }

        public IEnumerable<TagDTO> MainIngredients => AllTags.Where(x => x.Type == TagType.MainIngredient);
        public IEnumerable<TagDTO> DishTypes => AllTags.Where(x => x.Type == TagType.DishType);
        public IEnumerable<TagDTO> Occasions => AllTags.Where(x => x.Type == TagType.Occasion);
        public IEnumerable<TagDTO> Sources => AllTags.Where(x => x.Type == TagType.Source);

    }
}