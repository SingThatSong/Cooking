using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Cooking.WPF.Validation;
using Cooking.WPF.Views;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    /// View model for editing tags.
    /// </summary>
    public partial class TagEditViewModel : OkCancelViewModel
    {
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagEditViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="tagService">Tag service dependency.</param>
        /// <param name="localization">Localization provider dependency.</param>
        /// <param name="tag">Tag to edit. Null means new tag creation.</param>
        public TagEditViewModel(DialogService dialogService, TagService tagService, ILocalization localization, TagEdit? tag = null)
            : base(dialogService)
        {
            this.localization = localization;
            Tag = tag ?? new TagEdit();
            AllTagNames = tagService.GetTagNames();
            LoadedCommand = new DelegateCommand(OnLoaded);
            AddIconCommand = new AsyncDelegateCommand(AddIcon);
        }

        /// <summary>
        /// Gets or sets tag to edit.
        /// </summary>
        [AlsoNotifyFor(nameof(SimilarTags))]
        public TagEdit Tag { get; set; }

        /// <summary>
        /// Gets similar tags to avoid duplicates.
        /// </summary>
        public IEnumerable<string>? SimilarTags => string.IsNullOrWhiteSpace(Tag?.Name)
            ? null
            : AllTagNames.OrderBy(x => TagCompare(x, Tag.Name)).Take(Consts.HowManyAlternativesToShow);

        /// <summary>
        /// Gets localized caption for IsInMenu.
        /// </summary>
        public string? IsInMenuCaption => localization.GetLocalizedString("IsInMenu");

        /// <summary>
        /// Gets localized caption for MenuIcon.
        /// </summary>
        public string? MenuIconCaption => localization.GetLocalizedString("MenuIcon");
        
        /// <summary>
        /// Gets localized caption for Category.
        /// </summary>
        public string? CategoryCaption => localization.GetLocalizedString("Category");

        /// <summary>
        /// Gets localized caption for Color.
        /// </summary>
        public string? ColorCaption => localization.GetLocalizedString("Color");

        /// <summary>
        /// Gets localized caption for Name.
        /// </summary>
        public string? NameCaption => localization.GetLocalizedString("Name");

        /// <summary>
        /// Gets localized caption for ColorPicker's Manual.
        /// </summary>
        public string? ColorPickerManualCaption => localization.GetLocalizedString("ColorPicker_Manual");

        /// <summary>
        /// Gets localized caption for ColorPicker's AvailableColors.
        /// </summary>
        public string? ColorPickerAvailableColorsCaption => localization.GetLocalizedString("ColorPicker_AvailableColors");

        /// <summary>
        /// Gets localized caption for ColorPicker's Recent.
        /// </summary>
        public string? ColorPickerRecentCaption => localization.GetLocalizedString("ColorPicker_Recent");

        /// <summary>
        /// Gets localized caption for ColorPicker's Frequent.
        /// </summary>
        public string? ColorPickerFrequentCaption => localization.GetLocalizedString("ColorPicker_Frequent");

        /// <summary>
        /// Gets localized caption for ColorPicker's Standart.
        /// </summary>
        public string? ColorPickerStandartCaption => localization.GetLocalizedString("ColorPicker_Standart");

        /// <summary>
        /// Gets command to execute on loaded event.
        /// </summary>
        public DelegateCommand LoadedCommand { get; }

        /// <summary>
        /// Gets command to specify tag icon.
        /// </summary>
        public AsyncDelegateCommand AddIconCommand { get; }

        private bool NameChanged { get; set; }
        private List<string> AllTagNames { get; set; }

        /// <inheritdoc/>
        protected override bool CanOk() => Tag.IsValid();

        /// <inheritdoc/>
        protected override async Task Ok()
        {
            if (NameChanged && Tag.Name != null && AllTagNames.Any(x => TagCompare(Tag.Name, x) == 0))
            {
                bool okAnyway = false;

                await DialogService.ShowYesNoDialog(
                   localization.GetLocalizedString("TagAlreadyExists"),
                   localization.GetLocalizedString("SaveAnyway"),
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

            await base.Ok();
        }

        private async Task AddIcon()
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
            }
        }

        // WARNING: this is a crunch
        // When we open ingredient creation dialog second+ time, validation cannot see Ingredient being a required property, but when we change it's value - everything is ok
        // There is no such behaviour when using navigation, so it seems it's something Mahapps-related
        private void OnLoaded()
        {
            string? nameBackup = Tag.Name;
            TagType typeBackup = Tag.Type;

            Tag.Name = "123";
            Tag.Type = TagType.DishType;

            Tag.Name = nameBackup;
            Tag.Type = typeBackup;
            Tag.PropertyChanged += Tag_PropertyChanged;
        }

        private int TagCompare(string str1, string str2)
         => StringCompare.LevensteinDistance(
                    string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                    string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
            );
    }
}