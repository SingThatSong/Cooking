using Cooking.ServiceLayer;
using Data.Context;
using Data.Model;
using Data.Model.Plan;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DatabaseTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize]
        public void Setup()
        {
            File.Delete("cooking.db");
            using (var context = new CookingContext())
            {
                context.Database.EnsureCreated();
            }
        }

        [TestCleanup]
        public void Teardown()
        {
            File.Delete("cooking.db");
        }

        [TestMethod]
        public void CreateRecipe()
        {
            using (var context = new CookingContext())
            {
                context.Recipies.Add(new Recipe());
                context.SaveChanges();
            }

            using (var context = new CookingContext())
            {
                Assert.AreEqual(1, context.Recipies.Count());
            }
        }

        [TestMethod]
        public void RemovindRecipe_DoesNotRemoveWeek()
        {
            var recipe = new Recipe();

            using (var context = new CookingContext())
            {
                context.Recipies.Add(recipe);
                var week = new Week();
                week.Days = new List<Day>()
                {
                    new Day() { Dinner = recipe }
                };

                context.Weeks.Add(week);
                context.SaveChanges();
            }

            using (var context = new CookingContext())
            {
                Assert.AreEqual(1, context.Recipies.Count());
                Assert.AreEqual(1, context.Weeks.Count());
                Assert.AreEqual(1, context.Days.Count());
            }

            using (var context = new CookingContext())
            {
                context.Recipies.Remove(recipe);
                context.SaveChanges();
            }

            using (var context = new CookingContext())
            {
                Assert.AreEqual(0, context.Recipies.Count());
                Assert.AreEqual(1, context.Weeks.Count());
                Assert.AreEqual(1, context.Days.Count());
            }
        }

        [TestMethod]
        public void RemovindWeek_RemovesDay()
        {
            var recipe = new Recipe();
            var week = new Week();

            using (var context = new CookingContext())
            {
                context.Recipies.Add(recipe);
                week.Days = new List<Day>()
                {
                    new Day() { Dinner = recipe }
                };

                context.Weeks.Add(week);
                context.SaveChanges();
            }

            using (var context = new CookingContext())
            {
                Assert.AreEqual(1, context.Recipies.Count());
                Assert.AreEqual(1, context.Weeks.Count());
                Assert.AreEqual(1, context.Days.Count());
            }

            using (var context = new CookingContext())
            {
                context.Weeks.Remove(week);
                context.SaveChanges();
            }

            using (var context = new CookingContext())
            {
                Assert.AreEqual(1, context.Recipies.Count());
                Assert.AreEqual(0, context.Weeks.Count());
                Assert.AreEqual(0, context.Days.Count());
            }
        }

        [TestMethod]
        public void RemovindDay_DoesNotRemoveWeek()
        {
            using (var context = new CookingContext())
            {
                var recipe = new Recipe();
                context.Recipies.Add(recipe);
                context.Weeks.Add(new Week() { Days = new List<Day> { new Day() { Dinner = recipe } } });
                context.SaveChanges();
            }

            using (var context = new CookingContext())
            {
                Assert.AreEqual(1, context.Recipies.Count());
                Assert.AreEqual(1, context.Weeks.Count());
                Assert.AreEqual(1, context.Days.Count());
            }

            using (var context = new CookingContext())
            {
                var day = context.Days.First();
                context.Days.Remove(day);
                context.SaveChanges();
            }

            using (var context = new CookingContext())
            {
                Assert.AreEqual(1, context.Recipies.Count());
                Assert.AreEqual(1, context.Weeks.Count());
                Assert.AreEqual(0, context.Days.Count());
            }
        }

        [TestMethod]
        public void SetDinnerFK_Works()
        {
            var recipe = new Recipe();

            // Добавим рецепт в БД
            using (var context = new CookingContext())
            {
                context.Recipies.Add(recipe);
                context.SaveChanges();
            }

            using (var context = new CookingContext())
            {
                Assert.AreEqual(1, context.Recipies.Count());
                var rec = context.Recipies.Find(recipe.ID);
                Assert.IsNotNull(rec);
            }

            // Добавим неделю и день, устанавливая только FK на существующий рецепт
            using (var context = new CookingContext())
            {
                context.Add(new Week() { Days = new List<Day> { new Day() { DinnerID = recipe.ID } } });
                context.SaveChanges();
            }

            using (var context = new CookingContext())
            {
                Assert.AreEqual(1, context.Recipies.Count());
                Assert.AreEqual(1, context.Weeks.Count());
                Assert.AreEqual(1, context.Days.Count());
                Assert.AreEqual(recipe.ID, context.Days.First().DinnerID);
            }
        }


        [DataTestMethod]
        public void AddTagToRecipe()
        {
            var tag = new Tag() { Name = "hi" };
            TagService.CreateAsync(tag).Wait();

            var recipe = new Recipe();
            RecipeService.CreateAsync(recipe).Wait();

            var get = RecipeService.Get(recipe.ID);

            get.Tags = new List<RecipeTag> { new RecipeTag() { TagId = tag.ID, RecipeId = get.ID } };

            RecipeService.UpdateAsync(get).Wait();

            var get2 = RecipeService.Get(recipe.ID);

            Assert.IsNotNull(get2.Tags.First().Tag);
            //Assert.AreEqual(tag.Name, get2.Tags.First().Tag.Name);
        }
    }
}
