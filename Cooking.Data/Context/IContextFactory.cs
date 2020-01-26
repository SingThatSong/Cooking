namespace Cooking.Data.Context
{
    /// <summary>
    /// Factory for <see cref="CookingContext"/> creation.
    /// </summary>
    public interface IContextFactory
    {
        /// <summary>
        /// Method to create <see cref="CookingContext"/>.
        /// </summary>
        /// <param name="useLazyLoading">Use lazy loading in this context.</param>
        /// <returns>Instance of <see cref="CookingContext"/>.</returns>
        CookingContext Create(bool useLazyLoading = false);
    }
}
