using Cooking.DTO;
using Cooking.Helpers;
using Cooking.WPF.Helpers;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages
{
    public partial class GarnishEditViewModel : OkCancelViewModel, INotifyPropertyChanged
    {
        private readonly ILocalization localization;

        // State
        public GarnishEdit Garnish { get; set; }
        public string? NameCaption => localization.GetLocalizedString("Name");
        private bool NameChanged { get; set; }
        public IEnumerable<string>? SimilarGarnishes => string.IsNullOrWhiteSpace(Garnish?.Name)
            ? null
            : AllGarnishNames.OrderBy(x => GarnishCompare(x, Garnish.Name)).Take(3);
        private List<string> AllGarnishNames { get; set; }

        public GarnishEditViewModel(GarnishEdit? garnish, 
                                    GarnishService garnishService, 
                                    DialogService dialogService,
                                    ILocalization localization) : base (dialogService)
        {
            Garnish = garnish ?? new GarnishEdit();
            this.localization = localization;
            AllGarnishNames = garnishService.GetSearchNames();
            Garnish.PropertyChanged += (src, e) =>
            {
                if (e.PropertyName == nameof(Garnish.Name))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SimilarGarnishes)));
                    NameChanged = true;
                }
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected override async Task Ok()
        {
            if (NameChanged && Garnish.Name != null && AllGarnishNames.Any(x => x.ToUpperInvariant() == Garnish.Name.ToUpperInvariant()))
            {
                bool saveAnyway = false;
                await DialogService.ShowYesNoDialog(localization.GetLocalizedString("GarnishAlreadyExists"),
                                                    localization.GetLocalizedString("SaveAnyway"), 
                                                    successCallback: () => saveAnyway = true).ConfigureAwait(false);

                if (!saveAnyway)
                {
                    return;
                }
            }

            await base.Ok().ConfigureAwait(false);
        }

        private int GarnishCompare(string str1, string str2)
         => StringCompare.DiffLength(
                    string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                    string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
            );
    }
}