using AutoMapper;
using Cooking.DTO;
using Data.Context;
using Data.Model;
using Data.Model.Plan;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
            Trace.Listeners.Add(new TextWriterTraceListener("Log.log"));
            Trace.AutoFlush = true;
            AppDomain.CurrentDomain.UnhandledException += FatalUnhandledException;

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
                        dest.Ingredients = MapCollection(src.Ingredients, dest.Ingredients, context.Mapper);
                    })
                    .ReverseMap();
                cfg.CreateMap<DayDTO, Day>().ReverseMap();
                cfg.CreateMap<Week, WeekDTO>().ReverseMap();
                cfg.CreateMap<Garnish, GarnishDTO>().ReverseMap();
                cfg.CreateMap<TagDTO, Tag>()
                   .ReverseMap();

                cfg.CreateMap<RecipeDTO, Recipe>()
                    .ForMember(x => x.IngredientGroups, op => op.Ignore())
                    .ForMember(x => x.Ingredients, op => op.Ignore())
                    .ForMember(x => x.Tags, op => op.Ignore())
                    .AfterMap((src, dest, context) =>
                    {
                        dest.IngredientGroups = MapCollection(src.IngredientGroups, dest.IngredientGroups, context.Mapper);
                        dest.Ingredients      = MapCollection(src.Ingredients, dest.Ingredients, context.Mapper);
                        dest.Tags = src.Tags?.Select(x => new RecipeTag() { Recipe = dest, Tag = context.Mapper.Map<Tag>(x)}).ToList();
                    });

                cfg.CreateMap<Recipe, RecipeDTO>()
                   .ForMember(x => x.Tags, op => op.Ignore())
                   .AfterMap((src, dest, context) => 
                   {
                       if(dest.IngredientGroups != null)
                       {
                           foreach(var group in dest.IngredientGroups)
                           {
                               group.Ingredients = new ObservableCollection<RecipeIngredientDTO>(group.Ingredients.OrderBy(x => x.Order));
                           }
                       }

                       dest.Ingredients = new ObservableCollection<RecipeIngredientDTO>(dest.Ingredients.OrderBy(x => x.Order));
                       if (src.Tags != null)
                       {
                           var tags = src.Tags.Select(x => context.Mapper.Map<TagDTO>(x.Tag));
                           dest.Tags = new ObservableCollection<TagDTO>(tags.OrderBy(x => x.Type).ThenBy(x => x.Name));
                       }
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

        private const string dateTimeFormat = "dd.MM.yyyy hh:mm";

        private void FatalUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                var exceptionDescription = new StringBuilder();

                exceptionDescription.AppendLine(exception.Message);
                exceptionDescription.AppendLine(exception.StackTrace);

                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    exceptionDescription.AppendLine(exception.Message);
                    exceptionDescription.AppendLine(exception.StackTrace);
                }
                Trace.TraceError($"[{DateTime.Now.ToString(dateTimeFormat)}] {exceptionDescription.ToString()}");
            }
        }

        private List<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> source, IEnumerable<TDestination> destination, IRuntimeMapper mapper)
            where TDestination : class
        {
            if (source == null)
            {
                return null;
            }

            List<TDestination> result = new List<TDestination>();

            foreach (var ingredient in source)
            {
                if (((dynamic)ingredient).ID == null)
                {
                    result.Add(mapper.Map<TDestination>(ingredient));
                }
                else
                {
                    var existingIngredient = destination?.SingleOrDefault(x => ((dynamic)x).ID == ((dynamic)ingredient).ID);
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
