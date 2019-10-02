using Cooking.ServiceLayer.MainPage;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class WeekMain : WeekMainPage
    {
        [AlsoNotifyFor(nameof(Monday), nameof(Tuesday), nameof(Wednesday), nameof(Thursday), nameof(Friday), nameof(Saturday), nameof(Sunday))]
        public new ObservableCollection<DayMain> Days { get; set; }
        public DayMain? Monday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Monday);
        public DayMain? Tuesday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Tuesday);
        public DayMain? Wednesday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Wednesday);
        public DayMain? Thursday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Thursday);
        public DayMain? Friday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Friday);
        public DayMain? Saturday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Saturday);
        public DayMain? Sunday => Days?.FirstOrDefault(x => x.DayOfWeek == DayOfWeek.Sunday);
    }
}