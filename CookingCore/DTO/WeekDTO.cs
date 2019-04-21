using Data.Model;
using System;
using System.ComponentModel;

namespace Cooking.DTO
{
    public class WeekDTO : INotifyPropertyChanged
    {
        public Guid? ID { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Guid? MondayID { get; set; }
        public DayDTO Monday { get; set; }

        public Guid? TuesdayID { get; set; }
        public DayDTO Tuesday { get; set; }

        public Guid? WednesdayID { get; set; }
        public DayDTO Wednesday { get; set; }

        public Guid? ThursdayID { get; set; }
        public DayDTO Thursday { get; set; }

        public Guid? FridayID { get; set; }
        public DayDTO Friday { get; set; }

        public Guid? SaturdayID { get; set; }
        public DayDTO Saturday { get; set; }

        public Guid? SundayID { get; set; }
        public DayDTO Sunday { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}