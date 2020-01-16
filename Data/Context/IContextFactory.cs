namespace Cooking.Data.Context
{
    public interface IContextFactory
    {
        CookingContext Create(bool useLazyLoading = false);
    }
}
