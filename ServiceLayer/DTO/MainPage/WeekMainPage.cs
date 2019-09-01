using Data.Model;
using System;
using System.Collections.Generic;

namespace Cooking.ServiceLayer.MainPage
{
    public class WeekMainPage : Entity
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public List<DayMainPage> Days { get; set; }
    }
}