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

        //[TestMethod]
        //public void CreateRecipe()
        //{
        //    using (var context = new CookingContext())
        //    {
        //        context.Recipies.Add(new Recipe());
        //        context.SaveChanges();
        //    }

        //    using (var context = new CookingContext())
        //    {
        //        Assert.AreEqual(1, context.Recipies.Count());
        //    }
        //}

        [TestMethod]
        public void Test()
        {
            using (var context = new CookingContext())
            {
                var results = context.Recipies.Where(x => x.Name.StartsWith("Творожная")).ToList();

                foreach (var zapekanka in results)
                {

                }
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

            //    int weeks = 0;
            //    using (var context = new CookingContext())
            //    {
            //        context.Database.Migrate();
            //    }

            //    using (var context = new CookingContext())
            //    {
            //        weeks = context.Weeks.Count();

            //        var week = context.Weeks.ToList();

            //        var recipeDb = context.Recipies.Find(new Guid("2727f533-ec92-4566-a008-76b82cba1ad1"));
            //        context.Recipies.Remove(recipeDb);

            //        context.SaveChanges();
            //    }

            //    using (var context = new CookingContext())
            //    {
            //        Assert.AreEqual(weeks, context.Weeks.Count());
            //    }
            //}

        [TestMethod]
        public void BackupAndRestoreDb()
        {
            //DatabaseBackup.Backup();
            //DatabaseBackup.Restore(Environment.CurrentDirectory, "newdb.db");
        }
    }
}
