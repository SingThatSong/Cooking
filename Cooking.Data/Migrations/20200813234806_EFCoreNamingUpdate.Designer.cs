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
    [Migration("20200813234806_EFCoreNamingUpdate")]
    partial class EFCoreNamingUpdate
    {
        /// <inheritdoc/>
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0-rc.1.20413.2");

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

                    b.Property<string>("FullNamePluralization")
                        .IsRequired()
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

                    b.Property<Guid?>("IngredientID")
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

                    b.HasIndex("IngredientID");

                    b.HasIndex("IngredientsGroupID");

                    b.HasIndex("MeasureUnitID");

                    b.HasIndex("RecipeID");

                    b.ToTable("RecipeIngredients");
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

            modelBuilder.Entity("GarnishRecipe", b =>
                {
                    b.Property<Guid>("GarnishesID")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("RecipiesID")
                        .HasColumnType("TEXT");

                    b.HasKey("GarnishesID", "RecipiesID");

                    b.HasIndex("RecipiesID");

                    b.ToTable("GarnishRecipe");
                });

            modelBuilder.Entity("RecipeTag", b =>
                {
                    b.Property<Guid>("RecipiesID")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TagsID")
                        .HasColumnType("TEXT");

                    b.HasKey("RecipiesID", "TagsID");

                    b.HasIndex("TagsID");

                    b.ToTable("RecipeTag");
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

                    b.Navigation("Dinner");
                });

            modelBuilder.Entity("Cooking.Data.Model.RecipeIngredient", b =>
                {
                    b.HasOne("Cooking.Data.Model.Ingredient", "Ingredient")
                        .WithMany()
                        .HasForeignKey("IngredientID")
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

                    b.Navigation("Ingredient");

                    b.Navigation("MeasureUnit");
                });

            modelBuilder.Entity("GarnishRecipe", b =>
                {
                    b.HasOne("Cooking.Data.Model.Plan.Garnish", null)
                        .WithMany()
                        .HasForeignKey("GarnishesID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cooking.Data.Model.Recipe", null)
                        .WithMany()
                        .HasForeignKey("RecipiesID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RecipeTag", b =>
                {
                    b.HasOne("Cooking.Data.Model.Recipe", null)
                        .WithMany()
                        .HasForeignKey("RecipiesID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cooking.Data.Model.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Cooking.Data.Model.IngredientsGroup", b =>
                {
                    b.Navigation("Ingredients");
                });

            modelBuilder.Entity("Cooking.Data.Model.Recipe", b =>
                {
                    b.Navigation("IngredientGroups");

                    b.Navigation("Ingredients");
                });
#pragma warning restore 612, 618
        }
    }
}
