using Cooking.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cooking.Data.Migrations;

/// <summary>
/// Cooking context factory to create contexts for migrations.
/// </summary>
public class CookingContextFactory : IDesignTimeDbContextFactory<CookingContext>
{
    /// <inheritdoc/>
    public CookingContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CookingContext>();
        optionsBuilder.UseSqlite("Data Source=:memory:;Version=3;New=True;");

        return new CookingContext(optionsBuilder.Options);
    }
}
