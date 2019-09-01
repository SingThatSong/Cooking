using PropertyChanged;
using System;

namespace Cooking.Pages.Recepies
{
    [AddINotifyPropertyChangedInterface]
    public class SelectDay
    {
        public DayOfWeek WeekDay { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}