using System;
using System.ComponentModel;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Cooking.WPF.Views;
using Validar;
using WPF.Commands;

namespace Cooking.WPF.ViewModels;

/// <summary>
/// View model for editing tags.
/// </summary>
[InjectValidation]
public class TagEditViewModel : OkCancelViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TagEditViewModel"/> class.
    /// </summary>
    /// <param name="dialogService">Dialog service dependency.</param>
    /// <param name="tagService">Tag service dependency.</param>
    /// <param name="tag">Tag to edit. Null means new tag creation.</param>
    public TagEditViewModel(DialogService dialogService, CRUDService<Tag> tagService, TagEdit? tag = null)
        : base(dialogService)
    {
        Tag = tag ?? new TagEdit();
        Tag.PropertyChanged += Tag_PropertyChanged;
        AllTags = tagService.GetAll();
        AddIconCommand = new AsyncDelegateCommand(AddIconAsync);
    }

    /// <summary>
    /// Gets or sets tag to edit.
    /// </summary>
    public TagEdit Tag { get; set; }

    /// <summary>
    /// Gets similar tags to avoid duplicates.
    /// </summary>
    public IEnumerable<string?>? SimilarTags => string.IsNullOrWhiteSpace(Tag?.Name)
        ? null
        : AllTags.OrderBy(x => TagCompare(x.Name, Tag.Name))
                 .Select(x => x.Name)
                 .Take(Consts.HowManyAlternativesToShow);

    /// <summary>
    /// Gets command to specify tag icon.
    /// </summary>
    public AsyncDelegateCommand AddIconCommand { get; }

    private bool NameChanged { get; set; }
    private List<Tag> AllTags { get; }

    /// <inheritdoc/>
    protected override async Task OkAsync()
    {
        if (NameChanged && Tag.Name != null && AllTags.Any(x => x.ID != Tag.ID && TagCompare(Tag.Name, x.Name) == 0))
        {
            bool okAnyway = false;

            await DialogService.ShowLocalizedYesNoDialogAsync(
               "TagAlreadyExists",
               "SaveAnyway",
               successCallback: () => okAnyway = true);

            if (!okAnyway)
            {
                return;
            }
        }

        if (!Tag.IsInMenu)
        {
            Tag.MenuIcon = null;
        }

        await base.OkAsync();
    }

    private async Task AddIconAsync()
    {
        IconSelectViewModel result = await DialogService.ShowCustomMessageAsync<IconSelectView, IconSelectViewModel>();
        if (result.DialogResultOk)
        {
            Tag.MenuIcon = result.SelectedIcon;
        }
    }

    private void Tag_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Tag.Name))
        {
            NameChanged = true;
            OnPropertyChanged(nameof(SimilarTags));
        }
    }

    private int TagCompare(string? str1, string? str2)
    {
        if (str1 == null || str2 == null)
        {
            return int.MaxValue;
        }

        return StringCompare.LevensteinDistance(
                string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
        );
    }
}
