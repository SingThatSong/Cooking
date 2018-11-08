using AutoMapper;
using Cooking.Commands;
using Cooking.DTO;
using Data.Context;
using Data.Model;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Cooking.Pages.Recepies
{
    public partial class TagSelectEditViewModel : INotifyPropertyChanged
    {
        public bool DialogResultOk { get; set; }

        public TagSelectEditViewModel(IEnumerable<TagDTO> tags, List<TagDTO> allTags = null)
        {
            OkCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    DialogResultOk = true;
                    CloseCommand.Value.Execute();
                }));

            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

            if (allTags == null)
            {
                using (var context = new CookingContext())
                {
                    AllTags = context.Tags.Select(x => Mapper.Map<TagDTO>(x)).ToList();
                }
            }
            else
            {
                AllTags = allTags;
            }

            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    AllTags.Single(x => x.ID == tag.ID).IsChecked = true;
                }
            }
        }

        public ReadOnlyCollection<MeasureUnit> MeasurementUnits => MeasureUnit.AllValues;

        public event PropertyChangedEventHandler PropertyChanged;

        public Lazy<DelegateCommand> OkCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }

        public List<TagDTO> AllTags { get; set; }

    }
}