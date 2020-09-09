using AutoMapper;
using Cooking.Data.Context;
using Cooking.ServiceLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cooking.Tests
{
    [TestClass]
    public class RecipeServiceTests
    {
        // Positive tests
        [TestMethod]
        public void Execute_ExecutesFunction()
        {
            var factoryMock = new Mock<IContextFactory>();
            factoryMock.Setup(x => x.Create()).Returns(() =>
            {
                var teste = new CookingContext("test.db");
                teste.Database.Migrate();
                return teste;
            });

            var cultureProvider = new Mock<ICurrentCultureProvider>();
            cultureProvider.Setup(x => x.CurrentCulture).Returns(new System.Globalization.CultureInfo("ru-RU"));

            var service = new RecipeService(factoryMock.Object, cultureProvider.Object, Mock.Of<IMapper>(), new DayService(factoryMock.Object, cultureProvider.Object, Mock.Of<IMapper>()));

            var r = new Data.Model.Recipe();

            service.CreateAsync(r);

            int count = service.GetAll().Count;
            Assert.AreEqual(1, count);
        }
    }
}
