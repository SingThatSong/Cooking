using AutoMapper;
using Cooking.DTO;
using Cooking.ServiceLayer;
using Cooking.ServiceLayer.MainPage;
using Cooking.ServiceLayer.Projections;
using Data.Model;
using Data.Model.Plan;
using ServiceLayer.DTO.MainPage;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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

                
                cfg.CreateMap<DTO.GarnishEdit, DTO.GarnishEdit>();
                cfg.CreateMap<ServiceLayer.GarnishDTO, DTO.GarnishEdit>();
                cfg.CreateMap<DTO.GarnishEdit, Garnish>();
                cfg.CreateMap<WeekMainPage, WeekEdit>();
                cfg.CreateMap<DayMainPage, DayEdit>();
                cfg.CreateMap<RecipeSlim, RecipeSelect>();
                cfg.CreateMap<RecipeFull, RecipeEdit>()
                .AfterMap((src, dest) =>
                {
                    if (dest.Ingredients != null)
                    {
                        dest.Ingredients = new ObservableCollection<RecipeIngredientEdit>(dest.Ingredients.OrderBy(x => x.Order));
                    }

                    if (dest.IngredientGroups != null)
                    {
                        foreach (var group in dest.IngredientGroups)
                        {
                            group.Ingredients = new ObservableCollection<RecipeIngredientEdit>(group.Ingredients.OrderBy(x => x.Order));
                        }
                    }
                });
                cfg.CreateMap<RecipeEdit, RecipeEdit>();
                cfg.CreateMap<RecipeEdit, Recipe>().AfterMap((src, dest) =>
                {
                    if (dest.Tags != null)
                    {
                        foreach (var tag in dest.Tags)
                        {
                            tag.RecipeId = dest.ID;
                        }
                    }
                });

                cfg.CreateMap<RecipeIngredientEdit, RecipeIngredientEdit>();
                cfg.CreateMap<RecipeIngredientEdit, RecipeIngredient>()
                   .ForMember(x => x.ID, opts => opts.MapFrom(_ => Guid.NewGuid()))
                   .ForMember(x => x.Ingredient, opts => opts.Ignore());

                cfg.CreateMap<RecipeIngredientData, RecipeIngredientEdit>();
                cfg.CreateMap<TagServiceDto, TagEdit>();
                cfg.CreateMap<TagData, TagEdit>();
                cfg.CreateMap<TagEdit, Tag>();
                cfg.CreateMap<TagEdit, RecipeTag>()
                    .ForMember(x => x.TagId, opt => opt.MapFrom(x => x.ID));

                cfg.CreateMap<IngredientGroupEdit, IngredientGroupEdit>();
                cfg.CreateMap<IngredientGroupEdit, IngredientsGroup>();
                cfg.CreateMap<IngredientGroupData, IngredientGroupEdit>();
                cfg.CreateMap<IngredientData, IngredientEdit>();
                cfg.CreateMap<IngredientEdit, IngredientEdit>();
                cfg.CreateMap<IngredientEdit, Ingredient>();
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
