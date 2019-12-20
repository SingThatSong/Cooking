using Data.Model.Plan;
using ServiceLayer;
using System;

namespace avatest.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Hello World!";
        public Week CurrentWeek => WeekService.GetWeekAsync(DateTime.Now).Result;
    }
}
