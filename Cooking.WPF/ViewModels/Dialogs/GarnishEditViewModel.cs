using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    ///  View model for editing garnishes.
    /// </summary>
    public partial class GarnishEditViewModel : OkCancelViewModel
    {
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="GarnishEditViewModel"/> class.
        /// </summary>
        /// <param name="garnish">Garnish to edit.</param>
        /// <param name="garnishService">Garnish service dependency.</param>
        /// <param name="dialogService">Dialog service dependency.</param>
        /// <param name="localization">Localization dependency.</param>
        public GarnishEditViewModel(GarnishEdit? garnish,
                                    GarnishService garnishService,
                                    DialogService dialogService,
                                    ILocalization localization)
            : base(dialogService)
        {
            Garnish = garnish ?? new GarnishEdit();
            this.localization = localization;
            AllGarnishNames = garnishService.GetNames();
            LoadedCommand = new DelegateCommand(OnLoaded);
        }

        /// <summary>
        /// Gets or sets garnish to be edited.
        /// </summary>
        public GarnishEdit Garnish { get; set; }

        /// <summary>
        /// Gets localized name caption.
        /// </summary>
        public string? NameCaption => localization.GetLocalizedString("Name");

        /// <summary>
        /// Gets localized suggestion caption.
        /// </summary>
        public string? MaybeYouWantCaption => localization.GetLocalizedString("MaybeYouWant");

        /// <summary>
        /// Gets similar garnishes list to check on duplicates.
        /// </summary>
        public IEnumerable<string>? SimilarGarnishes => string.IsNullOrWhiteSpace(Garnish?.Name)
            ? null
            : AllGarnishNames.OrderBy(x => GarnishCompare(x, Garnish.Name)).Take(Consts.HowManyAlternativesToShow);

        /// <summary>
        /// Gets command to execute on loaded event.
        /// </summary>
        public DelegateCommand LoadedCommand { get; }

        private bool NameChanged { get; set; }

        private List<string> AllGarnishNames { get; set; }

        /// <inheritdoc/>
        protected override async Task Ok()
        {
            // Check if garnish is already exists
            if (NameChanged
             && Garnish.Name != null
             && AllGarnishNames.Any(x => string.Equals(x, Garnish.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                bool saveAnyway = false;
                await DialogService.ShowYesNoDialog(localization.GetLocalizedString("GarnishAlreadyExists"),
                                                    localization.GetLocalizedString("SaveAnyway"),
                                                    successCallback: () => saveAnyway = true);

                if (!saveAnyway)
                {
                    return;
                }
            }

            await base.Ok();
        }

        /// <inheritdoc/>
        protected override bool CanOk()
        {
            if (Garnish is INotifyDataErrorInfo dataErrorInfo)
            {
                return !dataErrorInfo.HasErrors;
            }
            else
            {
                return true;
            }
        }

        // WARNING: this is a crunch
        // When we open ingredient creation dialog second+ time, validation cannot see Ingredient being a required property, but when we change it's value - everything is ok
        // There is no such behaviour when using navigation, so it seems it's something Mahapps-related
        private void OnLoaded()
        {
            string? backup = Garnish.Name;
            Garnish.Name = "123";
            Garnish.Name = backup;
            Garnish.PropertyChanged += (src, e) =>
            {
                if (e.PropertyName == nameof(Garnish.Name))
                {
                    OnPropertyChanged(nameof(SimilarGarnishes));
                    NameChanged = true;
                }
            };
        }

        private int GarnishCompare(string str1, string str2)
         => StringCompare.LevensteinDistance(
                    string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                    string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
            );
    }
}