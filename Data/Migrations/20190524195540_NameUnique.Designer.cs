﻿// <auto-generated />
using Cooking.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cooking.Data.Migrations
{
    [DbContext(typeof(CookingContext))]
    [Migration("20190524195540_NameUnique")]
    partial class NameUnique
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {

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
                    b.Property<Guid?>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<Guid?>("RecipeID");

                    b.HasKey("ID");

                    b.HasIndex("RecipeID");

                    b.ToTable("IngredientsGroup");
                });

            modelBuilder.Entity("Data.Model.Plan.Day", b =>
                {
                    b.Property<Guid?>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("Date");

                    b.Property<Guid?>("DinnerID");

                    b.Property<bool>("DinnerWasCooked");

                    b.HasKey("ID");

                    b.HasIndex("DinnerID");

                    b.ToTable("Day");
                });

            modelBuilder.Entity("Data.Model.Plan.Week", b =>
                {
                    b.Property<Guid?>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("End");

                    b.Property<Guid?>("FridayID");

                    b.Property<Guid?>("MondayID");

                    b.Property<Guid?>("SaturdayID");

                    b.Property<DateTime>("Start");

                    b.Property<Guid?>("SundayID");

                    b.Property<Guid?>("ThursdayID");

                    b.Property<Guid?>("TuesdayID");

                    b.Property<Guid?>("WednesdayID");

                    b.HasKey("ID");

                    b.HasIndex("FridayID")
                        .IsUnique();

                    b.HasIndex("MondayID")
                        .IsUnique();

                    b.HasIndex("SaturdayID")
                        .IsUnique();

                    b.HasIndex("SundayID")
                        .IsUnique();

                    b.HasIndex("ThursdayID")
                        .IsUnique();

                    b.HasIndex("TuesdayID")
                        .IsUnique();

                    b.HasIndex("WednesdayID")
                        .IsUnique();

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

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Recipies");
                });

            modelBuilder.Entity("Data.Model.RecipeIngredient", b =>
                {
                    b.Property<Guid?>("ID")
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
                });

            modelBuilder.Entity("Data.Model.Plan.Week", b =>
                {
                    b.HasOne("Data.Model.Plan.Day", "Friday")
                        .WithOne()
                        .HasForeignKey("Data.Model.Plan.Week", "FridayID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data.Model.Plan.Day", "Monday")
                        .WithOne()
                        .HasForeignKey("Data.Model.Plan.Week", "MondayID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data.Model.Plan.Day", "Saturday")
                        .WithOne()
                        .HasForeignKey("Data.Model.Plan.Week", "SaturdayID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data.Model.Plan.Day", "Sunday")
                        .WithOne()
                        .HasForeignKey("Data.Model.Plan.Week", "SundayID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data.Model.Plan.Day", "Thursday")
                        .WithOne()
                        .HasForeignKey("Data.Model.Plan.Week", "ThursdayID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data.Model.Plan.Day", "Tuesday")
                        .WithOne()
                        .HasForeignKey("Data.Model.Plan.Week", "TuesdayID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data.Model.Plan.Day", "Wednesday")
                        .WithOne()
                        .HasForeignKey("Data.Model.Plan.Week", "WednesdayID")
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

        }
    }
}
