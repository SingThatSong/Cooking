using Cooking.Data.Context;
using Cooking.Data.Model;
using Cooking.Data.Model.Plan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RtfPipe;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooking.Web.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly WeekService weekService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleDataController"/> class.
        /// </summary>
        /// <param name="weekService"></param>
        public SampleDataController(WeekService weekService)
        {
            this.weekService = weekService;
        }

        private string Database => @"C:\Cooking\publish\cooking.db";

        /// <summary>
        ///
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
        ///
        /// </summary>
        /// <param name="id"></param>
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
        ///
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("[action]/currentweek")]
        public async Task<Week> CurrentWeek()
        {
            return await weekService.GetWeekAsync(DateTime.Now).ConfigureAwait(false);
        }
    }
}
