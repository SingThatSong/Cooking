using AutoMapper;
using Cooking.DTO;
using Data.Context;
using Data.Model;
using Data.Model.Plan;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Windows;

namespace Cooking
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AllowNullDestinationValues = true;

                cfg.CreateMap<TagDTO, TagDTO>();
                cfg.CreateMap<RecipeDTO, RecipeDTO>();
                cfg.CreateMap<IngredientDTO, IngredientDTO>();

                cfg.CreateMap<Tag, Tag>();
                cfg.CreateMap<Recipe, Recipe>();
                cfg.CreateMap<Ingredient, Ingredient>();
                cfg.CreateMap<MeasureUnit, MeasureUnit>();
                cfg.CreateMap<RecipeIngredient, RecipeIngredient>();

                cfg.CreateMap<DayDTO, Day>().ReverseMap();
                cfg.CreateMap<TagDTO, Tag>()
                   .ReverseMap();

                cfg.CreateMap<RecipeDTO, Recipe>();
                cfg.CreateMap<Recipe, RecipeDTO>()
                   .AfterMap((src, dest) => 
                   {
                       dest.FullPath = Path.GetFullPath(src.ImagePath);
                   });

                cfg.CreateMap<IngredientDTO, Ingredient>().ReverseMap();
                cfg.CreateMap<RecipeIngredientDTO, RecipeIngredient>()
                // Избегаем конфликтов в бд, используем только ID
                   .ForMember(x => x.Ingredient, op => op.Ignore());

                cfg.CreateMap<RecipeIngredient, RecipeIngredientDTO>();

            });

            using (var context = new CookingContext())
            {
                context.Database.Migrate();
            }
        }
    }
}
