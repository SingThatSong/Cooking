using System.Diagnostics.CodeAnalysis;
using Cooking.WPF.ViewModels;
using Cooking.WPF.Views;

namespace Cooking;

/// <summary>
/// Collection of project's constants.
/// </summary>
public static class Consts
{
    /// <summary>
    /// Symbol for a tag search.
    /// </summary>
    public const char TagSymbol = '#';

    /// <summary>
    /// Symbol for a ingredient search.
    /// </summary>
    public const string IngredientSymbol = "$";

    /// <summary>
    /// Symbol for a name search.
    /// </summary>
    public const string NameSymbol = "name";

    /// <summary>
    /// Maximum image width.
    /// </summary>
    public const int ImageWidth = 300;

    /// <summary>
    /// Days in a year.
    /// </summary>
    public const int YearDays = 365;

    /// <summary>
    /// Days in a month.
    /// </summary>
    public const int MonthDays = 30;

    /// <summary>
    /// Days in a week.
    /// </summary>
    public const int WeekDays = 7;

    /// <summary>
    /// How many alternatives to show in recommendations.
    /// </summary>
    public const int HowManyAlternativesToShow = 3;

    /// <summary>
    /// Megabyte constant.
    /// </summary>
    public const int Megabyte = 1024 * 1024;

    /// <summary>
    /// Images folder.
    /// </summary>
    public const string ImageFolder = "Images";

    /// <summary>
    /// Localization folder.
    /// </summary>
    public const string LocalizationFolder = "Localization";

    /// <summary>
    /// Prism main content region.
    /// </summary>
    public const string MainContentRegion = "PageDataRegion";

    /// <summary>
    /// Parameter for navigating to <see cref="WeekViewModel"/> indicating whether current week should be reloaded.
    /// </summary>
    public const string ReloadWeekParameter = "ReloadWeek";

    /// <summary>
    /// Parameter for navigating to <see cref="RecipeListView"/> indicating tag to filter.
    /// </summary>
    public const string TagNameParameter = "TagParameter";

    /// <summary>
    /// Localization file name without extentions.
    /// </summary>
    public const string LocalizationFilename = "local";

    /// <summary>
    /// Settings filename.
    /// </summary>
    public const string AppSettingsFilename = "appsettings.json";

    /// <summary>
    /// Settings filename.
    /// </summary>
    public const string LogFilename = "Log.txt";

    /// <summary>
    /// Localization error given to user.
    /// </summary>
    [SuppressMessage("Usage", "CA2211", Justification = "Hardcoded since we did not find localization.")]
    [SuppressMessage("Usage", "SA1401", Justification = "Hardcoded since we did not find localization.")]
    public static string LocalizationNotFound = $"Current settings culture: {{0}} is not provided. Please check {Consts.LocalizationFolder} folder.";
}
