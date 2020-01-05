﻿// <auto-generated />
using Cooking.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace Data.Migrations
{
    [DbContext(typeof(CookingContext))]
    partial class CookingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0-preview3.19153.1");

            modelBuilder.Entity("Data.Model.Ingredient", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int?>("TypeID");

                    b.HasKey("ID");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("Data.Model.IngredientsGroup", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<Guid?>("RecipeID");

                    b.HasKey("ID");

                    b.HasIndex("RecipeID");

                    b.ToTable("IngredientsGroup");
                });

            modelBuilder.Entity("Data.Model.Plan.Day", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("Date");

                    b.Property<int>("DayOfWeek");

                    b.Property<Guid?>("DinnerID");

                    b.Property<bool>("DinnerWasCooked");

                    b.Property<Guid?>("WeekID");

                    b.HasKey("ID");

                    b.HasIndex("DinnerID");

                    b.HasIndex("WeekID");

                    b.ToTable("Day");
                });

            modelBuilder.Entity("Data.Model.Plan.Garnish", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("Garnishes");
                });

            modelBuilder.Entity("Data.Model.Plan.Week", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("End");

                    b.Property<DateTime>("Start");

                    b.HasKey("ID");

                    b.ToTable("Weeks");
                });

            modelBuilder.Entity("Data.Model.Recipe", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CalorieType");

                    b.Property<string>("Description");

                    b.Property<int>("Difficulty");

                    b.Property<string>("ImagePath");

                    b.Property<string>("Name");

                    b.Property<int>("PortionsCount");

                    b.Property<int>("Rating");

                    b.Property<string>("SourceUrl");

                    b.HasKey("ID");

                    b.ToTable("Recipies");
                });

            modelBuilder.Entity("Data.Model.RecipeIngredient", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<double?>("Amount");

                    b.Property<Guid?>("IngredientId");

                    b.Property<Guid?>("IngredientsGroupID");

                    b.Property<int?>("MeasureUnitID");

                    b.Property<int>("Order");

                    b.Property<Guid?>("RecipeID");

                    b.HasKey("ID");

                    b.HasIndex("IngredientId");

                    b.HasIndex("IngredientsGroupID");

                    b.HasIndex("RecipeID");

                    b.ToTable("RecipeIngredients");
                });

            modelBuilder.Entity("Data.Model.RecipeTag", b =>
                {
                    b.Property<Guid>("RecipeId");

                    b.Property<Guid>("TagId");

                    b.HasKey("RecipeId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("RecipeTag");
                });

            modelBuilder.Entity("Data.Model.Tag", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Color");

                    b.Property<string>("Name");

                    b.Property<int>("Type");

                    b.HasKey("ID");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Data.Model.IngredientsGroup", b =>
                {
                    b.HasOne("Data.Model.Recipe")
                        .WithMany("IngredientGroups")
                        .HasForeignKey("RecipeID")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Data.Model.Plan.Day", b =>
                {
                    b.HasOne("Data.Model.Recipe", "Dinner")
                        .WithMany()
                        .HasForeignKey("DinnerID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Data.Model.Plan.Week", "Week")
                        .WithMany("Days")
                        .HasForeignKey("WeekID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Data.Model.RecipeIngredient", b =>
                {
                    b.HasOne("Data.Model.Ingredient", "Ingredient")
                        .WithMany()
                        .HasForeignKey("IngredientId");

                    b.HasOne("Data.Model.IngredientsGroup")
                        .WithMany("Ingredients")
                        .HasForeignKey("IngredientsGroupID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data.Model.Recipe")
                        .WithMany("Ingredients")
                        .HasForeignKey("RecipeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Data.Model.RecipeTag", b =>
                {
                    b.HasOne("Data.Model.Recipe", "Recipe")
                        .WithMany("Tags")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data.Model.Tag", "Tag")
                        .WithMany("Recipies")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
