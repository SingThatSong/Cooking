using AutoMapper;
using Cooking.DTO;
using Data.Context;
using Data.Model;
using Data.Model.Plan;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

                cfg.CreateMap<IngredientGroupDTO, IngredientsGroup>()
                    .ForMember(x => x.Ingredients, op => op.Ignore())
                    .AfterMap((src, dest, context) =>
                    {
                        if (src.Ingredients != null)
                        {
                            foreach (var ingredient in src.Ingredients)
                            {
                                var existingIngredient = dest.Ingredients?.SingleOrDefault(x => x.ID == ingredient.ID);
                                if (existingIngredient != null)
                                {
                                    context.Mapper.Map(ingredient, existingIngredient);
                                }
                                else
                                {
                                    dest.Ingredients = dest.Ingredients ?? new List<RecipeIngredient>();
                                    dest.Ingredients.Add(context.Mapper.Map<RecipeIngredient>(ingredient));
                                }
                            }
                        }
                        else
                        {
                            dest.Ingredients = null;
                        }
                    })
                    .ReverseMap();
                cfg.CreateMap<DayDTO, Day>().ReverseMap();
                cfg.CreateMap<TagDTO, Tag>()
                   .ReverseMap();

                cfg.CreateMap<RecipeDTO, Recipe>()
                    .ForMember(x => x.IngredientGroups, op => op.Ignore())
                    .ForMember(x => x.Ingredients, op => op.Ignore())
                    .ForMember(x => x.Tags, op => op.Ignore())
                    .AfterMap((src, dest, context) =>
                    {
                        if (src.IngredientGroups != null)
                        {
                            foreach (var group in src.IngredientGroups)
                            {
                                var existingGroup = dest.IngredientGroups?.SingleOrDefault(x => x.ID == group.ID);
                                if (existingGroup != null)
                                {
                                    context.Mapper.Map(group, existingGroup);
                                }
                                else
                                {
                                    dest.IngredientGroups = dest.IngredientGroups ?? new List<IngredientsGroup>();
                                    dest.IngredientGroups.Add(context.Mapper.Map<IngredientsGroup>(group));
                                }
                            }
                        }
                        else
                        {
                            dest.IngredientGroups = null;
                        }

                        if (src.Ingredients != null)
                        {
                            foreach (var ingredient in src.Ingredients)
                            {
                                var existingIngredient = dest.Ingredients?.SingleOrDefault(x => x.ID == ingredient.ID);
                                if (existingIngredient != null)
                                {
                                    context.Mapper.Map(ingredient, existingIngredient);
                                }
                                else
                                {
                                    dest.Ingredients = dest.Ingredients ?? new List<RecipeIngredient>();
                                    dest.Ingredients.Add(context.Mapper.Map<RecipeIngredient>(ingredient));
                                }
                            }
                        }
                        else
                        {
                            dest.Ingredients = null;
                        }

                        dest.Tags = src.Tags?.Select(x => new RecipeTag() { Recipe = dest, Tag = context.Mapper.Map<Tag>(x)}).ToList();
                    });

                cfg.CreateMap<Recipe, RecipeDTO>()
                   .ForMember(x => x.Tags, op => op.Ignore())
                   .AfterMap((src, dest, context) => 
                   {
                       dest.Tags = new ObservableCollection<TagDTO>(src.Tags.Select(x => context.Mapper.Map<TagDTO>(x.Tag)));
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
