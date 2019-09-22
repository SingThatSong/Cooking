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

namespace Cooking.Pages.Garnishes
{
    public partial class GarnishEditViewModel : OkCancelViewModel, INotifyPropertyChanged
    {
        private bool NameChanged { get; set; }

        public GarnishEditViewModel() : this(null)
        {

        }

        public GarnishEditViewModel(GarnishDTO category)
        {
            Garnish = category ?? new GarnishDTO();
            AllGarnishNames = GarnishService.GetSearchNames();
            Garnish.PropertyChanged += Garnish_PropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected override async void Ok()
        {
            if (NameChanged)
            {
                if (AllGarnishNames.Contains(Garnish.Name))
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
                                        });

                    if (result == MessageDialogResult.Negative)
                    {
                        return;
                    }
                }
            }

            base.Ok();
        }

        private void Garnish_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Garnish.Name))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SimilarGarnishes)));
                NameChanged = true;
            }
        }

        public GarnishDTO Garnish { get; set; }
        private List<string> AllGarnishNames { get; set; }

        public IEnumerable<string> SimilarGarnishes => string.IsNullOrWhiteSpace(Garnish?.Name)
            ? null 
            : AllGarnishNames.OrderBy(x => GarnishCompare(x, Garnish.Name)).Take(3);

        private int GarnishCompare(string str1, string str2)
         => StringCompare.DiffLength(
                    string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                    string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
            );
    }
}