using Cooking.Data.Model;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// Dto for week editing.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class WeekEdit : Entity
    {
        /// <summary>
        /// Gets or sets week start.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets or sets week end.
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Gets or sets week days.
        /// </summary>
        [AlsoNotifyFor(nameof(Monday), nameof(Tuesday), nameof(Wednesday), nameof(Thursday), nameof(Friday), nameof(Saturday), nameof(Sunday))]
        public ObservableCollection<DayEdit>? Days { get; set; }

        /// <summary>
        /// Gets monday.
        /// </summary>
        public DayEdit? Monday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Monday);

        /// <summary>
        /// Gets tuesday.
        /// </summary>
        public DayEdit? Tuesday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Tuesday);

        /// <summary>
        /// Gets wednesday.
        /// </summary>
        public DayEdit? Wednesday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Wednesday);

        /// <summary>
        /// Gets thursday.
        /// </summary>
        public DayEdit? Thursday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Thursday);

        /// <summary>
        /// Gets friday.
        /// </summary>
        public DayEdit? Friday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Friday);

        /// <summary>
        /// Gets saturday.
        /// </summary>
        public DayEdit? Saturday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Saturday);

        /// <summary>
        /// Gets sunday.
        /// </summary>
        public DayEdit? Sunday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Sunday);
    }
}