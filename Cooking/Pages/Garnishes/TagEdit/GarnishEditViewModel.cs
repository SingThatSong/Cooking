using Cooking.Commands;
using Cooking.DTO;
using Cooking.Helpers;
using Cooking.Pages.Recepies;
using Data.Context;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.Pages.Garnishes
{
    public partial class GarnishEditViewModel : OkCancelViewModel, INotifyPropertyChanged
    {
        public GarnishDTO Garnish { get; set; }
        private bool NameChanged { get; set; }

        public GarnishEditViewModel() : this(null) { }

        public GarnishEditViewModel(GarnishDTO? category)
        {
            Garnish = category ?? new GarnishDTO();
            AllGarnishNames = GarnishService.GetSearchNames();
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
            if (NameChanged)
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