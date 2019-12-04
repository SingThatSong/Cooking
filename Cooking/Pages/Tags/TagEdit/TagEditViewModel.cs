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

namespace Cooking.Pages.Tags
{
    public partial class TagEditViewModel : OkCancelViewModel
    {
        private bool NameChanged { get; set; }

        public TagEditViewModel() : this(null) { }

        public TagEditViewModel(TagEdit? category = null)
        {
            Tag = category ?? new TagEdit();
            AllTagNames = TagService.GetTagNames();
            Tag.PropertyChanged += Tag_PropertyChanged;
        }

        private void Tag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Tag.Name))
            {
                NameChanged = true;
            }
        }

        protected override async Task Ok()
        {
            if (NameChanged)
            {
                if (AllTagNames.Any(x => TagCompare(Tag.Name, x) == 0))
                {
                    var result = await DialogCoordinator.Instance.ShowMessageAsync(
                                        this,
                                        "Такой тег уже существует",
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

        [AlsoNotifyFor(nameof(SimilarTags))]
        public TagEdit Tag { get; set; }
        private List<string> AllTagNames { get; set; }

        public IEnumerable<string>? SimilarTags => string.IsNullOrWhiteSpace(Tag?.Name)
            ? null 
            : AllTagNames.OrderBy(x => TagCompare(x, Tag.Name)).Take(3);

        private int TagCompare(string str1, string str2)
         => StringCompare.DiffLength(
                    string.Join(" ", str1.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name)),
                    string.Join(" ", str2.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).OrderBy(name => name))
            );
    }
}