using Data.Context;

namespace Cooking.Data.Context
{
    public interface IContextFactory
    {
        CookingContext GetContext(bool useLazyLoading = false);
    }
}
