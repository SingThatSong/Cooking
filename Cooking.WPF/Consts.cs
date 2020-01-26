using Cooking.WPF.Views;

namespace Cooking
{
    /// <summary>
    /// Collection of project's constants.
    /// </summary>
    public static class Consts
    {
        /// <summary>
        /// Symbol for a tag search.
        /// </summary>
        public const string TagSymbol = "#";

        /// <summary>
        /// Symbol for a ingredient search.
        /// </summary>
        public const string IngredientSymbol = "$";

        /// <summary>
        /// Images folder.
        /// </summary>
        public const string ImageFolder = "Images";

        /// <summary>
        /// Prism main content region.
        /// </summary>
        public const string MainContentRegion = "PageDataRegion";

        /// <summary>
        /// Parameter for navigating to <see cref="WeekViewModel"/> indicating whether current week should be reloaded.
        /// </summary>
        public const string ReloadWeekParameter = "ReloadWeek";

        /// <summary>
        /// AppSettings language parameter.
        /// </summary>
        // TODO: remove after introducing object serialization.
        public const string LanguageConfigParameter = "culture";

        /// <summary>
        /// Settings filename.
        /// </summary>
        public const string AppSettingsFilename = "appsettings.json";
    }
}
