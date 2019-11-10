using Cooking.ServiceLayer.MainPage;
using Data.Context;
using Data.Model;
using Data.Model.Plan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RtfPipe;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private string Database => @"C:\Cooking\publish\cooking.db";

        [HttpGet("[action]")]
        public async Task<IEnumerable<Recipe>> WeatherForecasts()
        {
            List<Recipe> result;
            using (var context = new CookingContext(Database))
            {
                result = await context.Recipies.OrderBy(x => x.Name).ToListAsync();
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            foreach (var res in result.Where(x => x.Description != null))
            {
                res.Description = Rtf.ToHtml(res.Description);
            }

            return result;
        }

        [HttpGet("[action]/{id}")]
        public async Task<Recipe> WeatherForecasts(Guid id)
        {
            Recipe result;
            using (var context = new CookingContext(Database))
            {
                result = await context.Recipies.Include(x => x.Ingredients)
                                                  .ThenInclude(x => x.Ingredient)
                                               .Where(x => x.ID == id)
                                               .FirstOrDefaultAsync();
            }

            if (result != null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                result.Description = Rtf.ToHtml(result.Description);
            }

            return result;
        }

        [HttpGet("[action]/currentweek")]
        public async Task<WeekMainPage> CurrentWeek()
        {
            return await WeekService.GetWeekAsync(DateTime.Now);
        }
    }
}
