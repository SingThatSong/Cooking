using Cooking.Data.Model;
using Cooking.Data.Model.Plan;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace Cooking.Data.Context
{
    /// <summary>
    /// Single database context for the project.
    /// </summary>
    public class CookingContext : DbContext
    {
#if TRACE
        private static readonly LoggerFactory MyLoggerFactory =
                new LoggerFactory(new[]
                {
                    new DebugLoggerProvider()
                });
#endif

        private readonly SqliteConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CookingContext"/> class.
        /// </summary>
        /// <param name="dbFilename">Database file name.</param>
        public CookingContext(string dbFilename)
        {
            DbFilename = dbFilename;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CookingContext"/> class using existing connection.
        /// </summary>
        /// <param name="connection">Existing connection for database.</param>
        public CookingContext(SqliteConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CookingContext"/> class.
        /// Cooking context for migrations.
        /// </summary>
        /// <param name="dbContextOptions">Prepared context options.</param>
        internal CookingContext(DbContextOptions<CookingContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        /// <summary>
        /// Gets or sets days repository.
        /// </summary>
        public DbSet<Day> Days { get; set; }

        /// <summary>
        /// Gets or sets recipies repository.
        /// </summary>
        public DbSet<Recipe> Recipies { get; set; }

        /// <summary>
        /// Gets or sets ingredients repository.
        /// </summary>
        public DbSet<Ingredient> Ingredients { get; set; }

        /// <summary>
        /// Gets or sets ingredients in recipe repository.
        /// </summary>
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

        /// <summary>
        /// Gets or sets tags repository.
        /// </summary>
        public DbSet<Tag> Tags { get; set; }

        private string DbFilename { get; }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (DbFilename != null)
            {
                optionsBuilder.UseSqlite($"Data Source={DbFilename}");
            }

            if (connection != null)
            {
                optionsBuilder.UseSqlite(connection);
            }

#if TRACE
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
#endif
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Day>()
                        .ToTable("Day");

            modelBuilder.Entity<Day>()
                        .HasOne(x => x.Dinner)
                        .WithMany()
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recipe>()
                        .HasMany(x => x.IngredientGroups)
                        .WithOne()
                        .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<IngredientsGroup>()
                        .HasMany(x => x.Ingredients)
                        .WithOne()
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recipe>()
                        .HasMany(x => x.Ingredients)
                        .WithOne()
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recipe>()
                        .HasMany(r => r.Tags)
                        .WithMany(t => t.Recipies);

            modelBuilder.Entity<Recipe>()
                        .HasMany(r => r.Garnishes);

            modelBuilder.Entity<RecipeIngredient>()
                        .HasOne(x => x.Ingredient)
                        .WithMany()
                        .HasForeignKey(x => x.IngredientID)
                        .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<RecipeIngredient>()
                        .HasOne(x => x.MeasureUnit)
                        .WithMany()
                        .HasForeignKey(x => x.MeasureUnitID);
        }
    }
}
