using System;

namespace Data.Model.Plan
{
    public class Day : Entity
    {
        public Guid? DinnerID { get; set; }
        public virtual Recipe Dinner { get; set; }
        public bool DinnerWasCooked { get; set; }

        public DateTime? Date { get; set; }
        public DayOfWeek DayOfWeek { get; set; }


        public virtual Week Week { get; set; }
        public Guid? WeekID { get; set; }
    }
}