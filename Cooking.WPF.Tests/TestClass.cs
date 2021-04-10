using AutoMapper;
using Cooking.Data.Context;
using Cooking.ServiceLayer;
using Microsoft.Data.Sqlite;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooking.Tests
{
    public class TestClass
    {
        protected Mock<IContextFactory> CreateContextFactoryStub() => new();

        protected Mock<IContextFactory> CreateInmemoryDatabase()
        {
            var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
            keepAliveConnection.Open();

            var factoryMock = new Mock<IContextFactory>();
            factoryMock.Setup(x => x.Create()).Returns(() =>
            {
                var teste = new CookingContext(keepAliveConnection);
                teste.Database.EnsureCreated();
                return teste;
            });

            return factoryMock;
        }

        protected Mock<ILocalization> CreateLocalization()
        {
            var cultureProvider = new Mock<ILocalization>();
            cultureProvider.Setup(x => x.CurrentCulture).Returns(new System.Globalization.CultureInfo("ru-RU"));

            return cultureProvider;
        }

        protected RecipeService CreateRecipeService()
        {
            Mock<IContextFactory> factoryMock = CreateInmemoryDatabase();
            Mock<ILocalization> cultureProvider = CreateLocalization();

            var service = new RecipeService(factoryMock.Object, cultureProvider.Object, Mock.Of<IMapper>(), Mock.Of<IDayService>());
            return service;
        }
    }
}
