using Data.Model;
using Data.Model.Plan;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class CookingContext : DbContext
    {
        private string DbFilename { get; }
        public bool UseLazyLoading { get; }

        public CookingContext(string dbFilename = "cooking.db", bool useLazyLoading = false)
        {
            DbFilename = dbFilename;
            UseLazyLoading = useLazyLoading;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbFilename}")
#if DEBUG
                          //.UseLoggerFactory(new ConsoleLoggerFactory())
                          //.EnableSensitiveDataLogging()
#endif
                          ;

            if (UseLazyLoading)
            {
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Day>().ToTable("Day");

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


            modelBuilder.Entity<Recipe>()
                .HasMany(x => x.Ingredients)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(x => x.Ingredient)
                .WithMany()
                .HasForeignKey(x => x.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull);


            modelBuilder.Entity<Week>()
                .HasMany(x => x.Days)
                .WithOne(x => x.Week)
                .HasForeignKey(x => x.WeekID)
                .OnDelete(DeleteBehavior.Cascade);

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

        public DbSet<Week> Weeks { get; set; }
        public DbSet<Day> Days { get; set; }


        public DbSet<Recipe> Recipies { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Garnish> Garnishes { get; set; }
    }
}
