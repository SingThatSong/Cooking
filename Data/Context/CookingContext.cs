using Data.Model;
using Data.Model.Plan;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class CookingContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=cooking.db")
                          .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Week>()
                .HasOne(x => x.Monday)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Week>()
                .HasOne(x => x.Tuesday)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Week>()
                .HasOne(x => x.Wednesday)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Week>()
                .HasOne(x => x.Thursday)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Week>()
                .HasOne(x => x.Friday)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Week>()
                .HasOne(x => x.Saturday)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Week>()
                .HasOne(x => x.Sunday)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Day>()
                .HasOne(x => x.Dinner)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);



            //modelBuilder.Entity<Recipe>()
            //    .HasMany(x => x.IngredientsGroups)
            //    .WithOne()
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<IngredientsGroup>()
            //    .HasMany(x => x.Ingredients)
            //    .WithOne()
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recipe>()
                .HasMany(x => x.Ingredients)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recipe>()
                .HasMany(x => x.Tags)
                .WithOne()
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(x => x.Ingredient)
                .WithMany()
                .HasForeignKey(x => x.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }

        public DbSet<Week> Weeks { get; set; }


        public DbSet<Recipe> Recipies { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
