using AutoMapper;
using Cooking.ServiceLayer;
using Cooking.ServiceLayer.MainPage;
using Data.Model;
using Data.Model.Plan;
using ServiceLayer.DTO.MainPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ServiceLayer
{
    internal static class MapperService
    {
        private static readonly Lazy<IMapper> mapper = new Lazy<IMapper>(CreateMapper);

        public static IMapper Mapper => mapper.Value;

        private static IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AllowNullDestinationValues = true;
                cfg.AllowNullCollections = null;

                cfg.AddMaps(Assembly.GetEntryAssembly());

                cfg.CreateMap<Tag, TagSearch>();

                cfg.CreateMap<Week, WeekMainPage>();
                cfg.CreateMap<Day, DayMainPage>();

                cfg.CreateMap<Recipe, RecipeSlim>();
                cfg.CreateMap<Recipe, RecipeFull>()
                   .ForMember(x => x.Tags, opt => opt.MapFrom(x => x.Tags.Select(t => t.Tag)));

                cfg.CreateMap<IngredientsGroup, IngredientGroupData>();
                cfg.CreateMap<RecipeIngredient, RecipeIngredientData>();
                cfg.CreateMap<Tag, TagData>();

                cfg.CreateMap<Ingredient, IngredientData>();

                cfg.CreateMap<Garnish, GarnishDTO>();
                cfg.CreateMap<Garnish, Garnish>();

                //cfg.CreateMap<IngredientDTO, IngredientDTO>();

                //cfg.CreateMap<Tag, Tag>();
                //cfg.CreateMap<Recipe, Recipe>();
                //cfg.CreateMap<Recipe, RecipeSearch>();
                //cfg.CreateMap<Ingredient, Ingredient>();
                //cfg.CreateMap<MeasureUnit, MeasureUnit>();
                //cfg.CreateMap<RecipeIngredient, RecipeIngredient>();

                //cfg.CreateMap<IngredientGroupDTO, IngredientsGroup>()
                //    .ForMember(x => x.Ingredients, op => op.Ignore())
                //    .AfterMap((src, dest, context) =>
                //    {
                //        dest.Ingredients = MapCollection(src.Ingredients, dest.Ingredients, context.Mapper);
                //    })
                //    .ReverseMap();
                //cfg.CreateMap<DayDTO, Day>().ReverseMap();
                //cfg.CreateMap<Week, WeekAllData>().ReverseMap();

                //cfg.CreateMap<RecipeDTO, Recipe>()
                //    .ForMember(x => x.IngredientGroups, op => op.Ignore())
                //    .ForMember(x => x.Ingredients, op => op.Ignore())
                //    .ForMember(x => x.Tags, op => op.Ignore())
                //    .AfterMap((src, dest, context) =>
                //    {
                //        dest.IngredientGroups = MapCollection(src.IngredientGroups, dest.IngredientGroups, context.Mapper);
                //        dest.Ingredients = MapCollection(src.Ingredients, dest.Ingredients, context.Mapper);
                //        dest.Tags = src.Tags?.Select(x => new RecipeTag() { Recipe = dest, Tag = context.Mapper.Map<Tag>(x) }).ToList();
                //    });

                //cfg.CreateMap<RecipeMainPage, Recipe>().ReverseMap();


                //cfg.CreateMap<Recipe, RecipeDTO>()
                //   .ForMember(x => x.Tags, op => op.Ignore())
                //   .AfterMap((src, dest, context) =>
                //   {
                //       if (dest.IngredientGroups != null)
                //       {
                //           foreach (var group in dest.IngredientGroups)
                //           {
                //               group.Ingredients = group.Ingredients.OrderBy(x => x.Order);
                //           }
                //       }

                //       dest.Ingredients = dest.Ingredients.OrderBy(x => x.Order);
                //       if (src.Tags != null)
                //       {
                //           var tags = src.Tags.Select(x => context.Mapper.Map<TagDTO>(x.Tag));
                //           dest.Tags = tags.OrderBy(x => x.Type).ThenBy(x => x.Name);
                //       }
                //   });

                //cfg.CreateMap<IngredientDTO, Ingredient>().ReverseMap();
                //cfg.CreateMap<RecipeIngredientDTO, RecipeIngredient>()
                //   // Избегаем конфликтов в бд, используем только ID
                //   .ForMember(x => x.Ingredient, op => op.Ignore());

                //cfg.CreateMap<RecipeIngredient, RecipeIngredientDTO>();

            }).CreateMapper();
        }

        private static List<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> source, IEnumerable<TDestination> destination, IRuntimeMapper mapper)
            where TDestination : Entity
            where TSource : Entity
        {
            if (source == null)
            {
                return null;
            }

            var result = new List<TDestination>();

            foreach (var ingredient in source)
            {
                if (ingredient.ID == null)
                {
                    result.Add(mapper.Map<TDestination>(ingredient));
                }
                else
                {
                    var existingIngredient = destination?.SingleOrDefault(x => x.ID == ingredient.ID);
                    if (existingIngredient != null)
                    {
                        mapper.Map(ingredient, existingIngredient);
                        result.Add(existingIngredient);
                    }
                    else
                    {
                        result.Add(mapper.Map<TDestination>(ingredient));
                    }
                }
            }

            return result;
        }
    }
}
