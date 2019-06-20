using Cooking.Commands;
using Cooking.DTO;
using Cooking.Helpers;
using Data.Context;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cooking.Pages.Garnishes
{
    public partial class GarnishEditViewModel : INotifyPropertyChanged
    {
        public bool DialogResultOk { get; set; }
        private bool NameChanged { get; set; }

        public GarnishEditViewModel(GarnishDTO category = null)
        {
            OkCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    if (NameChanged)
                    {
                        if (AllTagNames.Any(x => TagCompare(Tag.Name, x) == 0))
                        {
                            var result = await DialogCoordinator.Instance.ShowMessageAsync(
                                                this, 
                                                "Такой тег уже существует", 
                                                "Всё равно сохранить?", 
                                                MessageDialogStyle.AffirmativeAndNegative, 
                                                new MetroDialogSettings() {
                                                    AffirmativeButtonText = "Да",
                                                    NegativeButtonText = "Нет"
                                                });

                            if (result == MessageDialogResult.Negative)
                            {
                                return;
                            }
                        }
                    }

                    DialogResultOk = true;
                    CloseCommand.Value.Execute();
                }));

            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    Tag.PropertyChanged -= Tag_PropertyChanged;
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

            Tag = category ?? new GarnishDTO();
            using (var context = new CookingContext())
            {
                AllTagNames = context.Garnishes.AsQueryable().Select(x => x.Name).ToList();
            }

            Tag.PropertyChanged += Tag_PropertyChanged;
        }

        private void Tag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Tag.Name))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SimilarTags)));
                NameChanged = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }

        public GarnishDTO Tag { get; set; }
        private List<string> AllTagNames { get; set; }

        public IEnumerable<string> SimilarTags => string.IsNullOrWhiteSpace(Tag?.Name)
            ? null 
            : AllTagNames.OrderBy(x => TagCompare(x, Tag.Name)).Take(3);

        private int TagCompare(string str1, string str2)
         => StringCompare.DiffLength(
                    string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                    string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
            );
    }
}