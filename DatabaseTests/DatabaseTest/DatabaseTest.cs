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
    public class DatabaseTest
    {
        [TestInitialize]
        public void Setup()
        {
            File.Delete("cooking.db");
            File.Move(@"DatabaseTest\TestDb.db", "cooking.db");
        }

        [TestMethod]
        public void SetDinnerFK_Works()
        {
            var recipe = new Recipe() { ID = new Guid("e6d12b05-4a7d-4d3e-985d-42ee1dfac767") };

            using (var context = new CookingContext())
            {
                var rec = context.Recipies.Find(recipe.ID);
                Assert.IsNotNull(rec);
            }

            // Добавим неделю и день, устанавливая только FK на существующий рецепт
            using (var context = new CookingContext())
            {
                context.Add(new Week() { Monday = new Day() { DinnerID = recipe.ID } });
                context.SaveChanges();
            }

            using (var context = new CookingContext())
            {
                Assert.AreEqual(recipe.ID, context.Days.First().DinnerID);
            }
        }
    }
}
