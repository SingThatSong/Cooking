using AutoMapper;
using Cooking.DTO;
using Cooking.ServiceLayer;
using Cooking.ServiceLayer.MainPage;
using ServiceLayer.DTO.MainPage;
using System;

namespace Cooking
{
    internal static class MapperService
    {
        private static Lazy<IMapper> mapper = new Lazy<IMapper>(CreateMapper);
        public static IMapper Mapper => mapper.Value;

        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullDestinationValues = true;

                cfg.CreateMap<WeekMainPage, WeekMain>();
                cfg.CreateMap<DayMainPage, DayMain>();
                cfg.CreateMap<RecipeSlim, RecipeSelect>();
                cfg.CreateMap<RecipeFull, RecipeMain>();
                cfg.CreateMap<RecipeIngredientData, RecipeIngredientMain>();
                cfg.CreateMap<TagSearch, TagDTO>();
                cfg.CreateMap<TagData, TagDTO>();
                cfg.CreateMap<IngredientGroupData, IngredientGroupMain>();
            });

            return config.CreateMapper();
        }
    }
}
