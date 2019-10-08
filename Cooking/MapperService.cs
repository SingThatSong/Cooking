using AutoMapper;
using Cooking.DTO;
using Cooking.ServiceLayer;
using Cooking.ServiceLayer.MainPage;
using Data.Model;
using Data.Model.Plan;
using ServiceLayer.DTO.MainPage;
using System;

namespace Cooking
{
    internal static class MapperService
    {
        private static readonly Lazy<IMapper> mapper = new Lazy<IMapper>(CreateMapper);
        public static IMapper Mapper => mapper.Value;

        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullDestinationValues = true;

                
                cfg.CreateMap<DTO.GarnishDTO, DTO.GarnishDTO>();
                cfg.CreateMap<ServiceLayer.GarnishDTO, DTO.GarnishDTO>();
                cfg.CreateMap<DTO.GarnishDTO, Garnish>();
                cfg.CreateMap<WeekMainPage, WeekMain>();
                cfg.CreateMap<DayMainPage, DayMain>();
                cfg.CreateMap<RecipeSlim, RecipeSelect>();
                cfg.CreateMap<RecipeFull, RecipeMain>();
                cfg.CreateMap<RecipeMain, RecipeMain>();
                cfg.CreateMap<RecipeMain, Recipe>();
                cfg.CreateMap<RecipeIngredientMain, RecipeIngredientMain>();
                cfg.CreateMap<RecipeIngredientMain, RecipeIngredient>();
                cfg.CreateMap<RecipeIngredientData, RecipeIngredientMain>();
                cfg.CreateMap<TagServiceDto, TagDTO>();
                cfg.CreateMap<TagData, TagDTO>();
                cfg.CreateMap<TagDTO, Tag>();
                cfg.CreateMap<TagDTO, RecipeTag>();
                cfg.CreateMap<IngredientGroupMain, IngredientGroupMain>();
                cfg.CreateMap<IngredientGroupMain, IngredientsGroup>();
                cfg.CreateMap<IngredientGroupData, IngredientGroupMain>();
                cfg.CreateMap<IngredientData, IngredientMain>();
                cfg.CreateMap<IngredientMain, IngredientMain>();
                cfg.CreateMap<IngredientMain, Ingredient>();
            });

            return config.CreateMapper();
        }

        public static T MapTo<T>(this object src)
        {
            return Mapper.Map<T>(src);
        }

        public static void MapTo(this object src, object dest)
        {
            Mapper.Map(src, dest);
        }
    }
}
