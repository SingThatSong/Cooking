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
        public virtual Day Monday { get; set; }

        public Guid? TuesdayID { get; set; }
        public virtual Day Tuesday { get; set; }

        public Guid? WednesdayID { get; set; }
        public virtual Day Wednesday { get; set; }

        public Guid? ThursdayID { get; set; }
        public virtual Day Thursday { get; set; }

        public Guid? FridayID { get; set; }
        public virtual Day Friday { get; set; }

        public Guid? SaturdayID { get; set; }
        public virtual Day Saturday { get; set; }

        public Guid? SundayID { get; set; }
        public virtual Day Sunday { get; set; }
    }
}
