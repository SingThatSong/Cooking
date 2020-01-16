using Cooking.Data.Model.Plan;
using System.Collections.Generic;

namespace avatest.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Hello World!";
        public Week CurrentWeek => new Week()
        {
            Days = new List<Day>()
            { 
                new Day(),
                new Day(),
                new Day(),
                new Day(),
                new Day(),
                new Day()
            }
        };
    }
}
