using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Cooking.WPF.Helpers;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.WPF.Views
{
    public partial class GarnishEditViewModel : OkCancelViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ILocalization localization;

        // State
        public bool SimilarGarnishesPresent => SimilarGarnishes?.Count() > 0;
        public GarnishEdit Garnish { get; set; }
        public string? NameCaption => localization.GetLocalizedString("Name");
        public string? MaybeYouWantCaption => localization.GetLocalizedString("MaybeYouWant");
        private bool NameChanged { get; set; }
        public IEnumerable<string>? SimilarGarnishes => string.IsNullOrWhiteSpace(Garnish?.Name)
            ? null
            : AllGarnishNames.OrderBy(x => GarnishCompare(x, Garnish.Name)).Take(3);
        private List<string> AllGarnishNames { get; set; }
        public DelegateCommand LoadedCommand { get; }

        public GarnishEditViewModel(GarnishEdit? garnish,
                                    GarnishService garnishService,
                                    DialogService dialogService,
                                    ILocalization localization)
            : base(dialogService)
        {
            Garnish = garnish ?? new GarnishEdit();
            this.localization = localization;
            AllGarnishNames = garnishService.GetSearchNames();
            Garnish.PropertyChanged += (src, e) =>
            {
                if (e.PropertyName == nameof(Garnish.Name))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SimilarGarnishes)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SimilarGarnishesPresent)));
                    NameChanged = true;
                }
            };
            LoadedCommand = new DelegateCommand(OnLoaded);
        }

        // WARNING: this is a crunch
        // When we open ingredient creation dialog second+ time, validation cannot see Ingredient being a required property, but when we change it's value - everything is ok
        // There is no such behaviour when using navigation, so it seems it's something Mahapps-related
        private void OnLoaded()
        {
            string? backup = Garnish.Name;
            Garnish.Name = "123";
            Garnish.Name = backup;
        }

        protected override async Task Ok()
        {
            // Check if garnish is already exists
            if (NameChanged && Garnish.Name != null && AllGarnishNames.Any(x => x.ToUpperInvariant() == Garnish.Name.ToUpperInvariant()))
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

        private int GarnishCompare(string str1, string str2)
         => StringCompare.DiffLength(
                    string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                    string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
            );
    }
}