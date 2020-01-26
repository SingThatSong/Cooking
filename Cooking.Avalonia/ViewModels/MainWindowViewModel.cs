using Cooking.Data.Model.Plan;
using System.Collections.Generic;

namespace Cooking.Avalonia.ViewModels
{
    /// <summary>
    /// Main view model.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets sample string.
        /// </summary>
        public string Greeting => "Hello World!";

        /// <summary>
        /// Gets sample week.
        /// </summary>
        public List<Day> CurrentWeek => new List<Day>()
        {
            new Day(),
            new Day(),
            new Day(),
            new Day(),
            new Day(),
            new Day()
        };
    }
}
