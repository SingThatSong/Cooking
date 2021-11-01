using AutoMapper;
using Cooking.Data.Context;
using Cooking.ServiceLayer;
using Microsoft.Data.Sqlite;
using Moq;

namespace Cooking.Tests;

/// <summary>
/// Base class for unit tests.
/// </summary>
public class TestClass
{
    /// <summary>
    /// Create inmemory Sqlite databae mock.
    /// </summary>
    /// <returns>Inmemory database mock.</returns>
    protected Mock<IContextFactory> CreateInmemoryDatabase()
    {
        var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
        keepAliveConnection.Open();

        var factoryMock = new Mock<IContextFactory>();
        factoryMock.Setup(x => x.Create()).Returns(() =>
        {
            var teste = new CookingContext(keepAliveConnection, "ru-RU");
            teste.Database.EnsureCreated();
            return teste;
        });

        return factoryMock;
    }

    /// <summary>
    /// Create localization mock.
    /// </summary>
    /// <returns>Localization database mock.</returns>
    protected Mock<ILocalization> CreateLocalization()
    {
        var cultureProvider = new Mock<ILocalization>();
        cultureProvider.Setup(x => x.CurrentCulture).Returns(new System.Globalization.CultureInfo("ru-RU"));

        return cultureProvider;
    }

    /// <summary>
    /// Create recipe service for unit testing.
    /// </summary>
    /// <returns>Recipe service for unit testing.</returns>
    protected RecipeService CreateRecipeService()
    {
        Mock<IContextFactory> factoryMock = CreateInmemoryDatabase();
        Mock<ILocalization> cultureProvider = CreateLocalization();

        var service = new RecipeService(factoryMock.Object, cultureProvider.Object, Mock.Of<IMapper>(), Mock.Of<IDayService>());
        return service;
    }
}
