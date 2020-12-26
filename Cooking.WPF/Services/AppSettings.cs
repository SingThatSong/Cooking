namespace Cooking.WPF.Services
{
    /// <summary>
    /// Application settings for use with IOptions.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets application culture setting.
        /// </summary>
        public string Culture { get; set; } = "ru-RU";

        /// <summary>
        /// Gets or sets application database name setting.
        /// </summary>
        public string DbName { get; set; } = "cooking.db";

        /// <summary>
        /// Gets or sets application theme (light or dark).
        /// </summary>
        public string Theme { get; set; } = "BaseDark";

        /// <summary>
        /// Gets or sets applicayion accent (color).
        /// </summary>
        public string Accent { get; set; } = "Teal";

        /// <summary>
        /// Gets or sets a value indicating whether is window maximized.
        /// </summary>
        public bool IsWindowMaximized { get; set; }

        /// <summary>
        /// Gets or sets window width.
        /// </summary>
        public double? WindowWidth { get; set; }

        /// <summary>
        /// Gets or sets window height.
        /// </summary>
        public double? WindowHeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show or hide suggestion about previous week.
        /// </summary>
        public bool ShowLastWeekSuggestion { get; set; } = true;
    }
}
