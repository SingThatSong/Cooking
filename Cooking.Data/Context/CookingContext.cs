using Cooking.Data.Model;
using Cooking.Data.Model.Plan;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="CookingContext"/> class.
        /// </summary>
        /// <param name="dbFilename">Database file name.</param>
        /// <param name="useLazyLoading">Use lazy loading in this context.</param>
        public CookingContext(string dbFilename, bool useLazyLoading = false)
        {
            DbFilename = dbFilename;
            UseLazyLoading = useLazyLoading;
        }

        public static readonly LoggerFactory _myLoggerFactory =
                new LoggerFactory(new[] {
                    new DebugLoggerProvider()
                });

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
        /// Gets a value indicating whether context uses lazy loading.
        /// </summary>
        public bool UseLazyLoading { get; }

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

        /// <summary>
        /// Gets or sets garnishes repository.
        /// </summary>
        public DbSet<Garnish> Garnishes { get; set; }

        private string DbFilename { get; }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbFilename}");

            if (UseLazyLoading)
            {
                optionsBuilder.UseLazyLoadingProxies();
            }

#if DEBUG
            optionsBuilder.UseLoggerFactory(_myLoggerFactory);
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
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Recipe>()
                .HasMany(x => x.IngredientGroups)
                .WithOne()
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<IngredientsGroup>()
                .HasMany(x => x.Ingredients)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ingredient>()
                .Ignore(x => x.Type);

            modelBuilder.Entity<Recipe>()
                .HasMany(x => x.Ingredients)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(x => x.Ingredient)
                .WithMany()
                .HasForeignKey(x => x.IngredientId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<RecipeIngredient>()
                .Ignore(x => x.MeasureUnit);

            // Recipe-Tag many-to-many relationship
            modelBuilder.Entity<RecipeTag>()
                .HasKey(bc => new { bc.RecipeId, bc.TagId });
            modelBuilder.Entity<RecipeTag>()
                .HasOne(bc => bc.Recipe)
                .WithMany(b => b.Tags)
                .HasForeignKey(bc => bc.RecipeId);
            modelBuilder.Entity<RecipeTag>()
                .HasOne(bc => bc.Tag)
                .WithMany(b => b.Recipies)
                .HasForeignKey(bc => bc.TagId);
        }
    }
}
