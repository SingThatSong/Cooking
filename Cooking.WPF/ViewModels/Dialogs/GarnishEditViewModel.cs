using Cooking.ServiceLayer;
using Cooking.WPF.Commands;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Validar;

namespace Cooking.WPF.ViewModels
{
    /// <summary>
    ///  View model for editing garnishes.
    /// </summary>
    [InjectValidation]
    public partial class GarnishEditViewModel : OkCancelViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GarnishEditViewModel"/> class.
        /// </summary>
        /// <param name="garnish">Garnish to edit.</param>
        /// <param name="garnishService">Garnish service dependency.</param>
        /// <param name="dialogService">Dialog service dependency.</param>
        public GarnishEditViewModel(GarnishEdit? garnish,
                                    GarnishService garnishService,
                                    DialogService dialogService)
            : base(dialogService)
        {
            Garnish = garnish ?? new GarnishEdit();
            AllGarnishNames = garnishService.GetNames();
            LoadedCommand = new DelegateCommand(OnLoaded);
        }

        /// <summary>
        /// Gets or sets garnish to be edited.
        /// </summary>
        public GarnishEdit Garnish { get; set; }

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
        protected override async Task OkAsync()
        {
            // Check if garnish is already exists
            if (NameChanged
             && Garnish.Name != null
             && AllGarnishNames.Any(x => string.Equals(x, Garnish.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                bool saveAnyway = false;
                await DialogService.ShowLocalizedYesNoDialogAsync("GarnishAlreadyExists",
                                                                  "SaveAnyway",
                                                                  successCallback: () => saveAnyway = true);

                if (!saveAnyway)
                {
                    return;
                }
            }

            await base.OkAsync();
        }

        private void OnLoaded()
        {
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