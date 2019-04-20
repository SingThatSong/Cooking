using Data.Backup;
using Data.Context;
using Data.Model;
using Data.Model.Plan;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
                context.Weeks.Add(new Week() { Monday = new Day() { Dinner = recipe } });
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
            }

            // Добавим неделю и день, устанавливая только FK на существующий рецепт
            using (var context = new CookingContext())
            {
                context.Add(new Week() { Monday = new Day() { DinnerID = recipe.ID } });
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

        [TestMethod]
        public void BackupAndRestoreDb()
        {
            //DatabaseBackup.Backup();
            //DatabaseBackup.Restore(Environment.CurrentDirectory, "newdb.db");
        }
    }
}
