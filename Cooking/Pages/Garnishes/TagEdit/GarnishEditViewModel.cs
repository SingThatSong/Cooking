using Cooking.DTO;
using Cooking.Helpers;
using MahApps.Metro.Controls.Dialogs;
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
        public GarnishEdit Garnish { get; set; }
        private bool NameChanged { get; set; }

        public GarnishEditViewModel(GarnishEdit? category, GarnishService garnishService, DialogService dialogService) : base (dialogService)
        {
            Garnish = category ?? new GarnishEdit();
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
            if (NameChanged && Garnish.Name != null)
            {
                if (AllGarnishNames.Any(x => x.ToUpperInvariant() == Garnish.Name.ToUpperInvariant()))
                {
                    var result = await DialogCoordinator.Instance.ShowMessageAsync(
                                        this,
                                        "Такой гарнир уже существует",
                                        "Всё равно сохранить?",
                                        MessageDialogStyle.AffirmativeAndNegative,
                                        new MetroDialogSettings()
                                        {
                                            AffirmativeButtonText = "Да",
                                            NegativeButtonText = "Нет"
                                        }).ConfigureAwait(false);

                    if (result == MessageDialogResult.Negative)
                    {
                        return;
                    }
                }
            }

            await base.Ok().ConfigureAwait(false);
        }
        
        private List<string> AllGarnishNames { get; set; }

        public IEnumerable<string>? SimilarGarnishes => string.IsNullOrWhiteSpace(Garnish?.Name)
            ? null 
            : AllGarnishNames.OrderBy(x => GarnishCompare(x, Garnish.Name)).Take(3);

        private int GarnishCompare(string str1, string str2)
         => StringCompare.DiffLength(
                    string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                    string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
            );
    }
}