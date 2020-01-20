using Cooking.Data.Model;
using Cooking.WPF.Services;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// DTO for user selection of CalorieType in various views.
    /// </summary>
    public class CalorieTypeSelection
    {
        /// <summary>
        /// Special value for not specified calories.
        /// </summary>
        public static readonly CalorieTypeSelection Any = new CalorieTypeSelection();

        private CalorieType calorieType;

        /// <summary>
        /// Gets or sets CalorieType represented by this DTO.
        /// </summary>
        public CalorieType CalorieType
        {
            get => calorieType;
            set
            {
                calorieType = value;
                Name = value.Description();
            }
        }

        /// <summary>
        /// Gets or sets name of CalorieType to show to user.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether given CalorieType is selected.
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
