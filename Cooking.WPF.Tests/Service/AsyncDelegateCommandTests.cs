using AutoMapper;
using Cooking.Data.Context;
using Cooking.ServiceLayer;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using System.Threading.Tasks;
using System;
using Microsoft.Data.Sqlite;

namespace Cooking.Tests
{
    public class RecipeServiceTests : IDisposable
    {
        // Positive tests
        [Fact]
        public async Task Execute_ExecutesFunction()
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

            var cultureProvider = new Mock<ICurrentCultureProvider>();
            cultureProvider.Setup(x => x.CurrentCulture).Returns(new System.Globalization.CultureInfo("ru-RU"));

            var service = new RecipeService(factoryMock.Object, cultureProvider.Object, Mock.Of<IMapper>(), new DayService(factoryMock.Object, cultureProvider.Object, Mock.Of<IMapper>()));

            var r = new Data.Model.Recipe();

            await service.CreateAsync(r);

            int count = service.GetAll().Count;
            count.Should().Be(1);
        }

        public void Dispose()
        {
            var teste = new CookingContext("test.db");
            teste.Database.EnsureDeleted();
        }
    }
}
