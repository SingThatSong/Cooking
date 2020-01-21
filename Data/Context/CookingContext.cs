using Cooking.Data.Model;
using Cooking.Data.Model.Plan;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

// NULL-warnings, not applicable to DbContext
#pragma warning disable CS8618, CS8603, CS8629, CS8602

namespace Cooking.Data.Context
{
    /// <summary>
    /// Single database context for the project
    /// </summary>
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
            optionsBuilder.UseSqlite($"Data Source={DbFilename}");

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

            SetGuidType<Day>(modelBuilder);
            SetGuidType<Garnish>(modelBuilder);
            SetGuidType<Week>(modelBuilder);

            SetGuidType<Recipe>(modelBuilder);
            SetGuidType<Tag>(modelBuilder);
            SetGuidType<Ingredient>(modelBuilder);
            SetGuidType<IngredientsGroup>(modelBuilder);
            SetGuidType<RecipeIngredient>(modelBuilder);

            // FKs
            SetPropertyGuidType<RecipeTag>(modelBuilder, e => e.RecipeId);
            SetPropertyGuidType<RecipeTag>(modelBuilder, e => e.TagId);
            SetPropertyGuidType<Day>(modelBuilder, e => e.WeekID);
            SetPropertyGuidType<Day>(modelBuilder, e => e.DinnerID);
            SetPropertyGuidType<RecipeIngredient>(modelBuilder, e => e.IngredientId);

        }

        private void SetPropertyGuidType<T>(ModelBuilder modelBuilder, Expression<Func<T, Guid?>> propertyExpression)
            where T : class
        {
            modelBuilder.Entity<T>()
                .Property(propertyExpression)
                .ValueGeneratedNever()
                .HasConversion(
                    g => g.Value.ToByteArray(),
                    b => new Guid(b));
        }

        private void SetPropertyGuidType<T>(ModelBuilder modelBuilder, Expression<Func<T, Guid>> propertyExpression)
            where T : class
        {
            modelBuilder.Entity<T>()
                .Property(propertyExpression)
                .ValueGeneratedNever()
                .HasConversion(
                    g => g.ToByteArray(),
                    b => new Guid(b));
        }

        private void SetGuidType<T>(ModelBuilder modelBuilder) where T : Entity
        {
            modelBuilder.Entity<T>()
                .Property(e => e.ID)
                .ValueGeneratedNever()
                .HasConversion(
                    g => g.ToByteArray(),
                    b => new Guid(b));
        }

        public DbSet<Week> Weeks { get; set; }
        public DbSet<Day> Days { get; set; }


        public DbSet<Recipe> Recipies { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Garnish> Garnishes { get; set; }
    }

#pragma warning restore CS8618, CS8602, CS8603, CS8629
}
