using Cooking.Data.Model;
using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Cooking.WPF.Services;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.WPF.Views
{
    public partial class TagEditViewModel : OkCancelViewModel
    {
        private readonly ILocalization localization;

        private bool NameChanged { get; set; }
        public string? CategoryCaption => localization.GetLocalizedString("Category");
        public string? ColorCaption => localization.GetLocalizedString("Color");
        public string? NameCaption => localization.GetLocalizedString("Name");
        public string? ColorPickerManualCaption => localization.GetLocalizedString("ColorPicker_Manual");
        public string? ColorPickerAvailableColorsCaption => localization.GetLocalizedString("ColorPicker_AvailableColors");
        public string? ColorPickerRecentCaption => localization.GetLocalizedString("ColorPicker_Recent");
        public string? ColorPickerFrequentCaption => localization.GetLocalizedString("ColorPicker_Frequent");
        public string? ColorPickerStandartCaption => localization.GetLocalizedString("ColorPicker_Standart");

        /// <summary>
        /// Gets command to execute on loaded event.
        /// </summary>
        public DelegateCommand LoadedCommand { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="TagEditViewModel"/> class.
        /// </summary>
        /// <param name="dialogService"></param>
        /// <param name="tagService"></param>
        /// <param name="localization">Localization provider dependency.</param>
        /// <param name="category"></param>
        public TagEditViewModel(DialogService dialogService, TagService tagService, ILocalization localization, TagEdit? category = null)
            : base(dialogService)
        {
            this.localization = localization;
            Tag = category ?? new TagEdit();
            AllTagNames = tagService.GetTagNames();
            Tag.PropertyChanged += Tag_PropertyChanged;
            LoadedCommand = new DelegateCommand(OnLoaded);
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
        }

        private void Tag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Tag.Name))
            {
                NameChanged = true;
            }
        }

        /// <inheritdoc/>
        protected override bool CanOk()
        {
            if (Tag is INotifyDataErrorInfo dataErrorInfo)
            {
                return !dataErrorInfo.HasErrors;
            }
            else
            {
                return true;
            }
        }

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

            await base.Ok();
        }

        [AlsoNotifyFor(nameof(SimilarTags))]
        public TagEdit Tag { get; set; }
        private List<string> AllTagNames { get; set; }

        public IEnumerable<string>? SimilarTags => string.IsNullOrWhiteSpace(Tag?.Name)
            ? null
            : AllTagNames.OrderBy(x => TagCompare(x, Tag.Name)).Take(3);

        private int TagCompare(string str1, string str2)
         => StringCompare.LevensteinDistance(
                    string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                    string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
            );
    }
}