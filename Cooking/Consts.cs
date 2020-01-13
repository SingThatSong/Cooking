namespace Cooking
{
    public static class Consts
    {
        public const string TagSymbol = "#";
        public const string IngredientSymbol = "$";
        public const string ImageFolder = "Images";

        public const string MainContentRegion = "PageDataRegion";

        public const string ReloadWeekParameter = "ReloadWeek";

        public readonly static string SearchHelpText = $"Поиск... Пример: Название {IngredientSymbol}\"ингредиент1\" and {IngredientSymbol}\"ингредиент2\" and {TagSymbol}\"тег1\" and {TagSymbol}\"тег2\"";

        public const string DbFilenameConfigParameter = "dbName";
        public const string LanguageConfigParameter = "culture";
        public const string DefaultCulture = "ru-RU";


        public const string SettingsFilename = "appsettings.json";
    }
}
