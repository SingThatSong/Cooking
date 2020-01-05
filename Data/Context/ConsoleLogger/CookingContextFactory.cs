using Microsoft.EntityFrameworkCore.Design;

namespace Cooking.Data.Context
{
    public class CookingContextFactory : IDesignTimeDbContextFactory<CookingContext>
    {
        public CookingContext CreateDbContext(string[] args)
        {
            return new CookingContext();
        }
    }
}
