using System;
using System.Collections.Generic;

namespace Data.Model.Plan
{
    public class Week : Entity
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public virtual List<Day>? Days { get; set; }
    }
}
