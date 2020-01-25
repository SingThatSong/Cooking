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
        /// Gets or sets applicayion database name setting.
        /// </summary>
        public string DbName { get; set; } = "cooking.db";
    }
}
