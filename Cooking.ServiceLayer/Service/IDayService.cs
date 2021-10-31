using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cooking.Data.Model.Plan;

namespace Cooking.ServiceLayer;

/// <summary>
/// Interface for <see cref="DayService"/>.
/// </summary>
public interface IDayService
{
    /// <summary>
    /// Set dinner for a new day.
    /// </summary>
    /// <param name="weekday">Date to identify a week.</param>
    /// <param name="dinnerID">Dinner to set to the new day.</param>
    /// <param name="dayOfWeek">New day's weekday.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task CreateDinnerAsync(DateTime weekday, Guid dinnerID, DayOfWeek dayOfWeek);

    /// <summary>
    /// Create new week.
    /// </summary>
    /// <param name="weekday">First day of week.</param>
    /// <param name="selectedRecepies">Recipies to add to the week.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task CreateWeekAsync(DateTime weekday, Dictionary<DayOfWeek, (Guid? RecipeID, Guid? GarnishID)> selectedRecepies);

    /// <summary>
    /// Calculate day offset from monday (e.g. tuesday will return 1).
    /// </summary>
    /// <param name="day">Day of week to calculate distance from monday.</param>
    /// <returns>Day offset from monday.</returns>
    int DaysFromMonday(DayOfWeek day);

    /// <summary>
    /// Delete whole week from database.
    /// </summary>
    /// <param name="weekday">First day of a period to which deleted days should belong.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task DeleteWeekAsync(DateTime weekday);

    /// <summary>
    /// Get first day of week for a given day.
    /// </summary>
    /// <param name="date">Any day on week.</param>
    /// <returns>DateTime of monday.</returns>
    DateTime FirstDayOfWeek(DateTime date);

    /// <summary>
    /// Get a date when recipe was last cooked.
    /// </summary>
    /// <param name="recipeID">Id of recipe to search last cooked date.</param>
    /// <returns>Date when recipe was last (most recently) cooked or null of recipe was never cooked.</returns>
    DateTime? GetLastCookedDate(Guid recipeID);

    /// <summary>
    /// Load week by date.
    /// </summary>
    /// <param name="weekday">Day of week that belongs to required week.</param>
    /// <returns>Week which contains provided day or null if no such week exists.</returns>
    Task<List<Day>?> GetWeekAsync(DateTime weekday);

    /// <summary>
    /// Get shopping list for selected week.
    /// </summary>
    /// <param name="periodStart">First day of a period to fetch shopping list.</param>
    /// <param name="periodEnd">Last day of a period to fetch shopping list.</param>
    /// <returns>Shopping list for a week as a collection of ingredient groups.</returns>
    List<ShoppingListIngredientsGroup> GetWeekShoppingList(DateTime periodStart, DateTime periodEnd);

    /// <summary>
    /// Load last cooked dates for all recipies to fill the cache.
    /// </summary>
    void InitCache();

    /// <summary>
    /// Check if all week's existing days' dinners were marked as cooked.
    /// </summary>
    /// <param name="weekday">Day of week to determine week itself.</param>
    /// <returns>True if week is filled and false if not.</returns>
    Task<bool> IsWeekFilledAsync(DateTime weekday);

    /// <summary>
    /// Get last day of week for a given day.
    /// </summary>
    /// <param name="date">Any day on week.</param>
    /// <returns>DateTime of sunday.</returns>
    DateTime LastDayOfWeek(DateTime date);

    /// <summary>
    /// Move day to next week.
    /// </summary>
    /// <param name="dayID">Day to move.</param>
    /// <param name="selectedWeekday">Weekday to move day to.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task MoveDayToNextWeekAsync(Guid dayID, DayOfWeek selectedWeekday);

    /// <summary>
    /// Set dinner for an existing day.
    /// </summary>
    /// <param name="dayID">ID of an existing day to which dinner should be set.</param>
    /// <param name="dinnerID">ID of a dinner to be set.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task SetDinnerAsync(Guid dayID, Guid dinnerID);

    /// <summary>
    /// Toggle dinner was cooked on a given day.
    /// </summary>
    /// <param name="dayID">Id of the day of the dinner.</param>
    /// <param name="wasCooked">Indicator of whether dinner was cooked.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SetDinnerWasCookedAsync(Guid dayID, bool wasCooked);
}
