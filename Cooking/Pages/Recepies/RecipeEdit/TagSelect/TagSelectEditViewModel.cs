using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
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
            OkCommand = new Lazy<DelegateCommand>(
               () => new DelegateCommand(async () => {
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
                    MainIngredients = context.Tags
                                             .Where(x => x.Type == TagType.MainIngredient)
                                             .Select(x => Mapper.Map<TagDTO>(x))
                                             .OrderBy(x => x.Name)
                                             .ToList();

                    DishTypes = context.Tags
                                       .Where(x => x.Type == TagType.DishType)
                                       .Select(x => Mapper.Map<TagDTO>(x))
                                       .OrderBy(x => x.Name)
                                       .ToList();

                    Occasions = context.Tags
                                      .Where(x => x.Type == TagType.Occasion)
                                      .Select(x => Mapper.Map<TagDTO>(x))
                                      .OrderBy(x => x.Name)
                                      .ToList();

                    Sources = context.Tags
                                     .Where(x => x.Type == TagType.Source)
                                     .Select(x => Mapper.Map<TagDTO>(x))
                                     .OrderBy(x => x.Name)
                                     .ToList();
                }
            }
            else
            {
                switch (filterTag)
                {
                    case TagType.DishType:
                        DishTypes = tagsFilter.ToList();
                        break;
                    case TagType.MainIngredient:
                        MainIngredients = tagsFilter.ToList();
                        break;
                    case TagType.Occasion:
                        Occasions = tagsFilter.ToList();
                        break;
                    case TagType.Source:
                        Sources = tagsFilter.ToList();
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

        public ReadOnlyCollection<MeasureUnit> MeasurementUnits => MeasureUnit.AllValues;

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }

        public List<TagDTO> MainIngredients { get; set; }
        public List<TagDTO> DishTypes { get; set; }
        public List<TagDTO> Occasions { get; set; }
        public List<TagDTO> Sources { get; set; }

    }
}