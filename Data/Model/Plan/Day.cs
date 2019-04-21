﻿using System;

namespace Data.Model.Plan
{
    public class Day
    {
        public Guid? ID { get; set; }
        public Guid? DinnerID { get; set; }
        public virtual Recipe Dinner { get; set; }
        public bool DinnerWasCooked { get; set; }

        public DateTime? Date { get; set; }
    }
}