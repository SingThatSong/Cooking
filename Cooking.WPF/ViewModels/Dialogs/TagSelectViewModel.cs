﻿using System.Collections.ObjectModel;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;
using Cooking.WPF.Views;
using WPF.Commands;

namespace Cooking.WPF.ViewModels;

/// <summary>
/// View model for tag selection.
/// </summary>
public partial class TagSelectViewModel : OkCancelViewModel
{
    private readonly CRUDService<Tag> tagService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagSelectViewModel"/> class.
    /// </summary>
    /// <param name="dialogService">Dialog service dependency.</param>
    /// <param name="tagService">Tag service dependency.</param>
    /// <param name="selectedTags">Alredy existing tags for editing.</param>
    /// <param name="allTags">All tags to select from.</param>
    public TagSelectViewModel(DialogService dialogService,
                              CRUDService<Tag> tagService,
                              IEnumerable<TagEdit> selectedTags,
                              IList<TagEdit>? allTags = null)
        : base(dialogService)
    {
        this.tagService = tagService;
        AddTagCommand = new DelegateCommand(AddTagAsync);
        AllTags = new ObservableCollection<TagEdit>(allTags ?? tagService.GetProjected<TagEdit>());
        SelectedItems.AddRange(AllTags.Intersect(selectedTags));

        AllTags.CollectionChanged += AllTags_CollectionChanged;
    }

    /// <summary>
    /// Gets command for adding a tag.
    /// </summary>
    public DelegateCommand AddTagCommand { get; }

    /// <summary>
    /// Gets all tags to choose from.
    /// </summary>
    public ObservableCollection<TagEdit> AllTags { get; }

    /// <summary>
    /// Gets or sets all selected tags.
    /// </summary>
    public ObservableCollection<TagEdit> SelectedItems { get; set; } = new ObservableCollection<TagEdit>();

    /// <summary>
    /// Gets main ingredient tags.
    /// </summary>
    public IEnumerable<TagEdit> MainIngredients => AllTags.Where(x => x.Type == TagType.MainIngredient);

    /// <summary>
    /// Gets dish type tags.
    /// </summary>
    public IEnumerable<TagEdit> DishTypes => AllTags.Where(x => x.Type == TagType.DishType);

    /// <summary>
    /// Gets occasion tags.
    /// </summary>
    public IEnumerable<TagEdit> Occasions => AllTags.Where(x => x.Type == TagType.Occasion);

    /// <summary>
    /// Gets sources tags.
    /// </summary>
    public IEnumerable<TagEdit> Sources => AllTags.Where(x => x.Type == TagType.Source);

    /// <summary>
    /// Add new tag.
    /// </summary>
    public async void AddTagAsync()
    {
        TagEditViewModel viewModel = await DialogService.ShowCustomLocalizedMessageAsync<TagEditView, TagEditViewModel>("NewTag");

        if (viewModel.DialogResultOk)
        {
            viewModel.Tag.ID = await tagService.CreateAsync(viewModel.Tag);
            AllTags.Add(viewModel.Tag);
        }
    }

    private void AllTags_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(MainIngredients));
        OnPropertyChanged(nameof(DishTypes));
        OnPropertyChanged(nameof(Occasions));
        OnPropertyChanged(nameof(Sources));
    }
}
