using System;
using System.Threading.Tasks;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cooking.Tests;

/// <summary>
/// Tests for <see cref="RecipeService"/>.
/// </summary>
public class RecipeServiceTests : TestClass
{
    /// <summary>
    /// Recipe creation actually creates record.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CreateRecipe_CreatesRecipe()
    {
        RecipeService recipeService = CreateRecipeService();

        var r = new Recipe();
        await recipeService.CreateAsync(r);

        int count = recipeService.GetAll().Count;
        count.Should().Be(1);
    }

    /// <summary>
    /// Recipe creation twice fails.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CreateRecipeTwice_FailsConstraint()
    {
        RecipeService recipeService = CreateRecipeService();

        var r = new Recipe();
        await recipeService.CreateAsync(r);

        Func<Task> act = async () => await recipeService.CreateAsync(r);
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    /// <summary>
    /// Recipe creation fills Recipe's ID.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CreateRecipe_IDGenerated()
    {
        RecipeService recipeService = CreateRecipeService();

        var r = new Recipe();
        r.ID.Should().Be(Guid.Empty);
        await recipeService.CreateAsync(r);
        r.ID.Should().NotBe(Guid.Empty);
    }

    /// <summary>
    /// Recipe deletion actially deletes Recipe.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CreateRecipe_AndRemove()
    {
        RecipeService recipeService = CreateRecipeService();

        var r = new Recipe();
        await recipeService.CreateAsync(r);
        await recipeService.DeleteAsync(r.ID);

        int count = recipeService.GetAll().Count;
        count.Should().Be(0);
    }
}
