using AutoMapper;
using Cooking.DTO;
using Cooking.ServiceLayer;
using Cooking.ServiceLayer.MainPage;
using Data.Model.Plan;
using ServiceLayer;
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
                cfg.CreateMap<RecipeIngredientData, RecipeIngredientMain>();
                cfg.CreateMap<TagSearch, TagDTO>();
                cfg.CreateMap<TagData, TagDTO>();
                cfg.CreateMap<IngredientGroupData, IngredientGroupMain>();
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
