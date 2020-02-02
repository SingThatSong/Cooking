using Cooking.Data.Context;
using Cooking.Data.Model;
using Cooking.Data.Model.Plan;
using Cooking.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RtfPipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooking.Web.Controllers
{
    /// <summary>
    /// Sample controller. To be renamed.
    /// </summary>
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly DayService dayService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleDataController"/> class.
        /// </summary>
        /// <param name="dayService"><see cref="WeekService"/>.</param>
        public SampleDataController(DayService dayService)
        {
            this.dayService = dayService;
        }

        private string Database => @"C:\Cooking\publish\cooking.db";

        /// <summary>
        /// TODO: Rename.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("[action]")]
        public async Task<IEnumerable<Recipe>> WeatherForecasts()
        {
            List<Recipe> result;
            using (var context = new CookingContext(Database))
            {
                result = await context.Recipies.OrderBy(x => x.Name).ToListAsync().ConfigureAwait(false);
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            foreach (Recipe res in result.Where(x => x.Description != null))
            {
                res.Description = Rtf.ToHtml(res.Description);
            }

            return result;
        }

        /// <summary>
        /// TODO: Rename.
        /// </summary>
        /// <param name="id">Recipe id to return.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("[action]/{id}")]
        public async Task<Recipe?> WeatherForecasts(Guid id)
        {
            Recipe result;
            using (var context = new CookingContext(Database))
            {
                result = await context.Recipies.Include(x => x.Ingredients)
                                                  .ThenInclude(x => x.Ingredient)
                                               .Where(x => x.ID == id)
                                               .FirstOrDefaultAsync().ConfigureAwait(false);
            }

            if (result != null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                result.Description = Rtf.ToHtml(result.Description);
            }

            return result;
        }

        /// <summary>
        /// TODO: Rename.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("[action]/currentweek")]
        public async Task<List<Day>?> CurrentWeek() => await dayService.GetWeekAsync(DateTime.Now).ConfigureAwait(false);
    }
}
