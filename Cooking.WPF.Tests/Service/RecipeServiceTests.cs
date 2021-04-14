using AutoMapper;
using Cooking.Data.Context;
using Cooking.Data.Model;
using Cooking.ServiceLayer;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cooking.Tests
{
    public class RecipeServiceTests : TestClass
    {
        [Fact]
        public async Task CreateRecipe_CreatesRecipe()
        {
            RecipeService recipeService = CreateRecipeService();

            var r = new Recipe();
            await recipeService.CreateAsync(r);

            int count = recipeService.GetAll().Count;
            count.Should().Be(1);
        }

        [Fact]
        public async Task CreateRecipeTwice_FailsConstraint()
        {
            RecipeService recipeService = CreateRecipeService();

            var r = new Recipe();
            await recipeService.CreateAsync(r);

            Func<Task> act = async () => await recipeService.CreateAsync(r);
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task CreateRecipe_IDGenerated()
        {
            RecipeService recipeService = CreateRecipeService();

            var r = new Recipe();
            r.ID.Should().Be(Guid.Empty);
            await recipeService.CreateAsync(r);
            r.ID.Should().NotBe(Guid.Empty);
        }

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
}
