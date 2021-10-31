namespace Cooking.Data.Context;

/// <summary>
/// Factory for <see cref="CookingContext"/> creation.
/// </summary>
public interface IContextFactory
{
    /// <summary>
    /// Method to create <see cref="CookingContext"/>.
    /// </summary>
    /// <returns>Instance of <see cref="CookingContext"/>.</returns>
    CookingContext Create();
}
