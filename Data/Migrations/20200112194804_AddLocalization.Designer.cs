﻿// <auto-generated />
using Cooking.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cooking.Data.Migrations
{
    [DbContext(typeof(CookingContext))]
    [Migration("20200112194804_AddLocalization")]
    partial class AddLocalization
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0");

            modelBuilder.Entity("Data.Model.Ingredient", b =>
                {
                    b.Property<byte[]>("ID")
                        .HasColumnType("BLOB");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("TypeID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("Data.Model.IngredientsGroup", b =>
                {
                    b.Property<byte[]>("ID")
                        .HasColumnType("BLOB");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("RecipeID")
                        .HasColumnType("BLOB");

                    b.HasKey("ID");

                    b.HasIndex("RecipeID");

                    b.ToTable("IngredientsGroup");
                });

            modelBuilder.Entity("Data.Model.Plan.Day", b =>
                {
                    b.Property<byte[]>("ID")
                        .HasColumnType("BLOB");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("DinnerID")
                        .HasColumnType("BLOB");

                    b.Property<bool>("DinnerWasCooked")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("WeekID")
                        .HasColumnType("BLOB");

                    b.HasKey("ID");

                    b.HasIndex("DinnerID");

                    b.HasIndex("WeekID");

                    b.ToTable("Day");
                });

            modelBuilder.Entity("Data.Model.Plan.Garnish", b =>
                {
                    b.Property<byte[]>("ID")
                        .HasColumnType("BLOB");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Garnishes");
                });

            modelBuilder.Entity("Data.Model.Plan.Week", b =>
                {
                    b.Property<byte[]>("ID")
                        .HasColumnType("BLOB");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("End")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Start")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Weeks");
                });

            modelBuilder.Entity("Data.Model.Recipe", b =>
                {
                    b.Property<byte[]>("ID")
                        .HasColumnType("BLOB");

                    b.Property<int>("CalorieType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int>("Difficulty")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ImagePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("PortionsCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rating")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SourceUrl")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Recipies");
                });

            modelBuilder.Entity("Data.Model.RecipeIngredient", b =>
                {
                    b.Property<byte[]>("ID")
                        .HasColumnType("BLOB");

                    b.Property<double?>("Amount")
                        .HasColumnType("REAL");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("IngredientId")
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("IngredientsGroupID")
                        .HasColumnType("BLOB");

                    b.Property<int?>("MeasureUnitID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("RecipeID")
                        .HasColumnType("BLOB");

                    b.HasKey("ID");

                    b.HasIndex("IngredientId");

                    b.HasIndex("IngredientsGroupID");

                    b.HasIndex("RecipeID");

                    b.ToTable("RecipeIngredients");
                });

            modelBuilder.Entity("Data.Model.RecipeTag", b =>
                {
                    b.Property<byte[]>("RecipeId")
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("TagId")
                        .HasColumnType("BLOB");

                    b.HasKey("RecipeId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("RecipeTag");
                });

            modelBuilder.Entity("Data.Model.Tag", b =>
                {
                    b.Property<byte[]>("ID")
                        .HasColumnType("BLOB");

                    b.Property<string>("Color")
                        .HasColumnType("TEXT");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Data.Model.IngredientsGroup", b =>
                {
                    b.HasOne("Data.Model.Recipe", null)
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

                    b.HasOne("Data.Model.IngredientsGroup", null)
                        .WithMany("Ingredients")
                        .HasForeignKey("IngredientsGroupID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data.Model.Recipe", null)
                        .WithMany("Ingredients")
                        .HasForeignKey("RecipeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Data.Model.RecipeTag", b =>
                {
                    b.HasOne("Data.Model.Recipe", "Recipe")
                        .WithMany("Tags")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.Model.Tag", "Tag")
                        .WithMany("Recipies")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
