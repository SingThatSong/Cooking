using Data.Context;
using Data.Model;
using Data.Model.Plan;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace DatabaseTests
{
    [TestClass]
    public class DatabaseTest
    {
        const string dbName = "TestDb.db";

        [TestInitialize]
        public void Setup()
        {
            File.Delete(dbName);
            File.Move($@"DatabaseTest\{dbName}", dbName);

            using var context = new CookingContext(dbName);
            context.Database.Migrate();
        }

        [TestMethod]
        public void SetDinnerFK_Works()
        {
            var recipe = new Recipe() { ID = new Guid("e6d12b05-4a7d-4d3e-985d-42ee1dfac767") };

            using (var context = new CookingContext(dbName))
            {
                var rec = context.Recipies.Find(recipe.ID);
                Assert.IsNotNull(rec);
            }

            var week = new Week() { Days = new List<Day> { new Day() { DinnerID = recipe.ID } } };

            // Добавим неделю и день, устанавливая только FK на существующий рецепт
            using (var context = new CookingContext(dbName))
            {
                context.Add(week);
                context.SaveChanges();
            }

            using (var context = new CookingContext(dbName, true))
            {
                var test = context.Weeks.Find(week.ID);
            }
        }
    }
}
