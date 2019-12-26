using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    [SuppressMessage("Usage", "CA2227:Свойства коллекций должны быть доступны только для чтения")]
    public class WeekEdit
    {
        public Guid ID { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        [AlsoNotifyFor(nameof(Monday), nameof(Tuesday), nameof(Wednesday), nameof(Thursday), nameof(Friday), nameof(Saturday), nameof(Sunday))]
        public ObservableCollection<DayEdit>? Days { get; set; }
        public DayEdit? Monday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Monday);
        public DayEdit? Tuesday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Tuesday);
        public DayEdit? Wednesday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Wednesday);
        public DayEdit? Thursday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Thursday);
        public DayEdit? Friday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Friday);
        public DayEdit? Saturday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Saturday);
        public DayEdit? Sunday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Sunday);
    }
}