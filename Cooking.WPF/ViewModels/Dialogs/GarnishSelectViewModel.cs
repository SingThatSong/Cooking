﻿using System.Collections.ObjectModel;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;

namespace Cooking.WPF.ViewModels;

/// <summary>
/// View model for tag selection.
/// </summary>
public partial class GarnishSelectViewModel : OkCancelViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GarnishSelectViewModel"/> class.
    /// </summary>
    /// <param name="dialogService">Dialog service dependency.</param>
    /// <param name="garnishService">Tag service dependency.</param>
    /// <param name="selectedGarnishes">Loc25 provider dependency.</param>
    public GarnishSelectViewModel(DialogService dialogService,
                                  CRUDService<Recipe> garnishService,
                                  IEnumerable<RecipeEdit> selectedGarnishes)
        : base(dialogService)
    {
        AllGarnishes = garnishService.GetMapped<RecipeEdit>(x => x.Tags.Any(t => t.Name == "Гарниры")).OrderBy(x => x.Name).ToList();
        SelectedItems.AddRange(AllGarnishes.Intersect(selectedGarnishes));
    }

    /// <summary>
    /// Gets all tags to choose from.
    /// </summary>
    public List<RecipeEdit> AllGarnishes { get; private set; }

    /// <summary>
    /// Gets or sets all selected tags.
    /// </summary>
    public ObservableCollection<RecipeEdit> SelectedItems { get; set; } = new ObservableCollection<RecipeEdit>();
}
