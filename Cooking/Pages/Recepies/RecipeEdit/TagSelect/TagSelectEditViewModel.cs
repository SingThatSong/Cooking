using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Cooking.Pages.Tags;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Cooking.Pages.Recepies
{
    public partial class TagSelectEditViewModel : INotifyPropertyChanged
    {
        public bool DialogResultOk { get; set; }

        public TagSelectEditViewModel(IEnumerable<TagDTO> tags, TagType? filterTag = null, IEnumerable<TagDTO> tagsFilter = null)
        {
            AddTagCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(AddTag));

            OkCommand = new Lazy<DelegateCommand>(
               () => new DelegateCommand(() => {
                   DialogResultOk = true;
                   CloseCommand.Value.Execute();
               }));

            CloseCommand = new Lazy<DelegateCommand>(
               () => new DelegateCommand(async () => {
                   var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                   await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
               }));

            if (!filterTag.HasValue)
            {
                using (var context = new CookingContext())
                {
                    MainIngredients = new ObservableCollection<TagDTO>(context.Tags
                                             .Where(x => x.Type == TagType.MainIngredient)
                                             .Select(x => Mapper.Map<TagDTO>(x))
                                             .OrderBy(x => x.Name));

                    DishTypes = new ObservableCollection<TagDTO>(context.Tags
                                       .Where(x => x.Type == TagType.DishType)
                                       .Select(x => Mapper.Map<TagDTO>(x))
                                       .OrderBy(x => x.Name));

                    Occasions = new ObservableCollection<TagDTO>(context.Tags
                                      .Where(x => x.Type == TagType.Occasion)
                                      .Select(x => Mapper.Map<TagDTO>(x))
                                      .OrderBy(x => x.Name));

                    Sources = new ObservableCollection<TagDTO>(context.Tags
                                     .Where(x => x.Type == TagType.Source)
                                     .Select(x => Mapper.Map<TagDTO>(x))
                                     .OrderBy(x => x.Name));
                }
            }
            else
            {
                switch (filterTag)
                {
                    case TagType.DishType:
                        DishTypes = new ObservableCollection<TagDTO>(tagsFilter);
                        break;
                    case TagType.MainIngredient:
                        MainIngredients = new ObservableCollection<TagDTO>(tagsFilter);
                        break;
                    case TagType.Occasion:
                        Occasions = new ObservableCollection<TagDTO>(tagsFilter);
                        break;
                    case TagType.Source:
                        Sources = new ObservableCollection<TagDTO>(tagsFilter);
                        break;
                }
            }

            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    if (MainIngredients != null)
                    {
                        var mainIngredient = MainIngredients.FirstOrDefault(x => x.ID == tag.ID);
                        if (mainIngredient != null)
                        {
                            mainIngredient.IsChecked = true;
                            continue;
                        }
                    }

                    if (DishTypes != null)
                    {
                        var dishType = DishTypes.FirstOrDefault(x => x.ID == tag.ID);
                        if (dishType != null)
                        {
                            dishType.IsChecked = true;
                            continue;
                        }
                    }

                    if (Occasions != null)
                    {
                        var occsion = Occasions.FirstOrDefault(x => x.ID == tag.ID);
                        if (occsion != null)
                        {
                            occsion.IsChecked = true;
                            continue;
                        }
                    }

                    if (Sources != null)
                    {
                        var source = Sources.FirstOrDefault(x => x.ID == tag.ID);
                        if (source != null)
                        {
                            source.IsChecked = true;
                            continue;
                        }
                    }
                }
            }

        }

        public async void AddTag()
        {
            var viewModel = new TagEditViewModel();

            var dialog = new CustomDialog()
            {
                Title = "Новый тег",
                Content = new TagEditView()
                {
                    DataContext = viewModel
                }
            };

            var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);

            await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);

            do
            {
                await dialog.WaitUntilUnloadedAsync();
            }
            while (current != await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this));

            if (viewModel.DialogResultOk)
            {
                var category = Mapper.Map<Tag>(viewModel.Tag);
                using (var context = new CookingContext())
                {
                    context.Add(category);
                    context.SaveChanges();
                }
                viewModel.Tag.ID = category.ID;

                switch(category.Type)
                {
                    case TagType.DishType:
                        DishTypes.Add(viewModel.Tag);
                        break;
                    case TagType.MainIngredient:
                        MainIngredients.Add(viewModel.Tag);
                        break;
                    case TagType.Occasion:
                        Occasions.Add(viewModel.Tag);
                        break;
                    case TagType.Source:
                        Sources.Add(viewModel.Tag);
                        break;
                }
            }
        }

        public ReadOnlyCollection<MeasureUnit> MeasurementUnits => MeasureUnit.AllValues;

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }

        public Lazy<DelegateCommand> AddTagCommand { get; }

        public ObservableCollection<TagDTO> MainIngredients { get; set; }
        public ObservableCollection<TagDTO> DishTypes { get; set; }
        public ObservableCollection<TagDTO> Occasions { get; set; }
        public ObservableCollection<TagDTO> Sources { get; set; }

    }
}