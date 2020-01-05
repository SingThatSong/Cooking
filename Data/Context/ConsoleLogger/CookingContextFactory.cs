using Microsoft.EntityFrameworkCore.Design;

namespace Data.Context
{
    public class CookingContextFactory : IDesignTimeDbContextFactory<CookingContext>
    {
        public CookingContext CreateDbContext(string[] args)
        {
            return new CookingContext();
        }
    }
}
