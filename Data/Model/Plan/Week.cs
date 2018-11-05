using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Model.Plan
{
    public class Week
    {
        public Guid? ID { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Guid? MondayID { get; set; }
        public Day Monday { get; set; }

        public Guid? TuesdayID { get; set; }
        public Day Tuesday { get; set; }

        public Guid? WednesdayID { get; set; }
        public Day Wednesday { get; set; }

        public Guid? ThursdayID { get; set; }
        public Day Thursday { get; set; }

        public Guid? FridayID { get; set; }
        public Day Friday { get; set; }

        public Guid? SaturdayID { get; set; }
        public Day Saturday { get; set; }

        public Guid? SundayID { get; set; }
        public Day Sunday { get; set; }
    }
}
