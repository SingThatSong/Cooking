using System;

namespace Data.Model.Plan
{
    public class Day
    {
        public Guid? ID { get; set; }
        public Guid? DinnerID { get; set; }
        public Recipe Dinner { get; set; }
    }
}