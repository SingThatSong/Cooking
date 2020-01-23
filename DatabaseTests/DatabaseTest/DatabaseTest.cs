using Cooking.Data.Context;
using Cooking.Data.Model;
using Cooking.Data.Model.Plan;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cooking.Tests
{
    [TestClass]
    public class DatabaseTest
    {
        private const string DbName = "TestDb.db";

        [TestInitialize]
        public void Setup()
        {
            File.Delete(DbName);
            File.Move($@"DatabaseTest\{DbName}", DbName);

            using var context = new CookingContext(DbName);
            context.Database.Migrate();
        }

        [TestMethod]
        public void SetDinnerFK_Works()
        {
            var recipe = new Recipe() { ID = new Guid("e6d12b05-4a7d-4d3e-985d-42ee1dfac767") };

            using (var context = new CookingContext(DbName))
            {
                Recipe rec = context.Recipies.Find(recipe.ID);
                Assert.IsNotNull(rec);
            }

            var week = new Week() { Days = new List<Day> { new Day() { DinnerID = recipe.ID } } };

            // Добавим неделю и день, устанавливая только FK на существующий рецепт
            using (var context = new CookingContext(DbName))
            {
                context.Add(week);
                context.SaveChanges();
            }

            using (var context = new CookingContext(DbName, true))
            {
                Week test = context.Weeks.Find(week.ID);
            }
        }
    }
}
