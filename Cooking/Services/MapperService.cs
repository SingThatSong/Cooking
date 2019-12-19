using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cooking.DTO;
using Cooking.ServiceLayer;
using Cooking.ServiceLayer.Projections;
using Cooking.Services;
using Data.Model;
using Data.Model.Plan;
using ServiceLayer.DTO.MainPage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Cooking
{
    internal static class MapperService
    {
        public static IConfigurationProvider CreateMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AllowNullDestinationValues = true;

                cfg.CreateMap<Garnish, GarnishEdit>().ReverseMap();
                cfg.CreateMap<Week, WeekEdit>();
                cfg.CreateMap<Day, DayEdit>();
                cfg.CreateMap<Tag, TagEdit>();
                cfg.CreateMap<Ingredient, IngredientEdit>();
                cfg.CreateMap<RecipeIngredient, RecipeIngredientEdit>()
                    .ForMember(x => x.MeasureUnit, opt => opt.Ignore())
                    .ForMember(x => x.Amount, opt => opt.Ignore())
                    .ForMember(x => x.Ingredient, opt => opt.Ignore())
                    .ForMember(x => x.Order, opt => opt.Ignore())
                    ;
                cfg.CreateMap<IngredientsGroup, IngredientGroupEdit>()
                   .ForMember(x => x.Ingredients, opt => opt.Ignore())
                   .AfterMap((src, dest, context) => 
                   { 
                       if (src.Ingredients != null)
                       {
                           dest.Ingredients = new ObservableCollection<RecipeIngredientEdit>();
                       }
                   });

                cfg.CreateMap<Recipe, RecipeSelectDto>();
                cfg.CreateMap<Recipe, RecipeEdit>()
                   .ForMember(x => x.Tags, opt => opt.MapFrom(x => x.Tags.Select(t => t.Tag)))
                   .AfterMap<RecipeConverter>();


                cfg.CreateMap<RecipeSlim, RecipeSelectDto>();
                cfg.CreateMap<RecipeFull, RecipeEdit>()
                .AfterMap((src, dest) =>
                {
                    if (dest.Ingredients != null)
                    {
                        dest.Ingredients = new List<RecipeIngredientEdit>(dest.Ingredients.OrderBy(x => x.Order));
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
        }
    }


}
