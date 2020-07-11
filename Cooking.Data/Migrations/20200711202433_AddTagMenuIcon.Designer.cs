﻿// <auto-generated />
using System;
using Cooking.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cooking.Data.Migrations
{
    [DbContext(typeof(CookingContext))]
    [Migration("20200711202433_AddTagMenuIcon")]
    partial class AddTagMenuIcon
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0-preview.8.20360.8");

            modelBuilder.Entity("Cooking.Data.Model.Ingredient", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("Cooking.Data.Model.IngredientsGroup", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("RecipeID")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("RecipeID");

                    b.ToTable("IngredientsGroup");
                });

            modelBuilder.Entity("Cooking.Data.Model.MeasureUnit", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<string>("FullName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("MeasureUnit");
                });

            modelBuilder.Entity("Cooking.Data.Model.Plan.Day", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("DinnerID")
                        .HasColumnType("TEXT");

                    b.Property<bool>("DinnerWasCooked")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("DinnerID");

                    b.ToTable("Day");
                });

            modelBuilder.Entity("Cooking.Data.Model.Plan.Garnish", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Garnishes");
                });

            modelBuilder.Entity("Cooking.Data.Model.Recipe", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

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

            modelBuilder.Entity("Cooking.Data.Model.RecipeIngredient", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<double?>("Amount")
                        .HasColumnType("REAL");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("IngredientId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("IngredientsGroupID")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("MeasureUnitID")
                        .HasColumnType("TEXT");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("RecipeID")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("IngredientId");

                    b.HasIndex("IngredientsGroupID");

                    b.HasIndex("MeasureUnitID");

                    b.HasIndex("RecipeID");

                    b.ToTable("RecipeIngredients");
                });

            modelBuilder.Entity("Cooking.Data.Model.RecipeTag", b =>
                {
                    b.Property<Guid>("RecipeId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TagId")
                        .HasColumnType("TEXT");

                    b.HasKey("RecipeId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("RecipeTag");
                });

            modelBuilder.Entity("Cooking.Data.Model.Tag", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Color")
                        .HasColumnType("TEXT");

                    b.Property<string>("Culture")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsInMenu")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MenuIcon")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Cooking.Data.Model.IngredientsGroup", b =>
                {
                    b.HasOne("Cooking.Data.Model.Recipe", null)
                        .WithMany("IngredientGroups")
                        .HasForeignKey("RecipeID")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Cooking.Data.Model.Plan.Day", b =>
                {
                    b.HasOne("Cooking.Data.Model.Recipe", "Dinner")
                        .WithMany()
                        .HasForeignKey("DinnerID")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();
                });

            modelBuilder.Entity("Cooking.Data.Model.RecipeIngredient", b =>
                {
                    b.HasOne("Cooking.Data.Model.Ingredient", "Ingredient")
                        .WithMany()
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Cooking.Data.Model.IngredientsGroup", null)
                        .WithMany("Ingredients")
                        .HasForeignKey("IngredientsGroupID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Cooking.Data.Model.MeasureUnit", "MeasureUnit")
                        .WithMany()
                        .HasForeignKey("MeasureUnitID");

                    b.HasOne("Cooking.Data.Model.Recipe", null)
                        .WithMany("Ingredients")
                        .HasForeignKey("RecipeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Cooking.Data.Model.RecipeTag", b =>
                {
                    b.HasOne("Cooking.Data.Model.Recipe", "Recipe")
                        .WithMany("Tags")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cooking.Data.Model.Tag", "Tag")
                        .WithMany("Recipies")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
