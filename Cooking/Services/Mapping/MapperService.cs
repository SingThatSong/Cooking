using AutoMapper;
using Cooking.Data.Model;
using Cooking.Data.Model.Plan;
using Cooking.ServiceLayer;
using Cooking.ServiceLayer.Projections;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking
{
    internal static class MapperService
    {
        public static IConfigurationProvider CreateMapper()
            => new MapperConfiguration(cfg =>
                {
                    cfg.AllowNullDestinationValues = true;

                    // Map created recipe to displayed in list
                    cfg.CreateMap<RecipeEdit, RecipeSelectDto>()

                       // It is a new recipe, so we just set LastCooked to infinity
                       .ForMember(x => x.LastCooked, opts => opts.MapFrom(_ => int.MaxValue));

                    // Backup dto for editing
                    cfg.CreateMap<GarnishEdit, GarnishEdit>();
                    cfg.CreateMap<RecipeEdit, RecipeEdit>();
                    cfg.CreateMap<TagEdit, TagEdit>();
                    cfg.CreateMap<IngredientEdit, IngredientEdit>();

                    // Project Recipe from db to displayed in lists
                    cfg.CreateMap<Recipe, RecipeSelectDto>()
                       .AfterMap<RecipeDtoConverter>();

                    // Cleanup below
                    // -----------------------
                    cfg.CreateMap<Garnish, GarnishEdit>().ReverseMap();
                    cfg.CreateMap<Week, WeekEdit>();
                    cfg.CreateMap<Day, DayEdit>();
                    cfg.CreateMap<Tag, TagEdit>();
                    cfg.CreateMap<Ingredient, IngredientEdit>().ReverseMap();
                    cfg.CreateMap<RecipeIngredient, RecipeIngredientEdit>();
                    cfg.CreateMap<IngredientsGroup, IngredientGroupEdit>();

                    cfg.CreateMap<Recipe, RecipeEdit>()
                       .ForMember(x => x.Tags, opt => opt.MapFrom(x => x.Tags.Select(t => t.Tag)))
                       .AfterMap<RecipeConverter>();

                    cfg.CreateMap<RecipeSlim, RecipeSelectDto>();
                    cfg.CreateMap<RecipeFull, RecipeEdit>()
                    .AfterMap((src, dest) =>
                    {
                        if (dest.Ingredients != null)
                        {
                            dest.Ingredients = new ObservableCollection<RecipeIngredientEdit>(dest.Ingredients.OrderBy(x => x.Order));
                        }

                        if (dest.IngredientGroups != null)
                        {
                            foreach (IngredientGroupEdit group in dest.IngredientGroups)
                            {
                                group.Ingredients = new ObservableCollection<RecipeIngredientEdit>(group.Ingredients.OrderBy(x => x.Order));
                            }
                        }
                    });

                    cfg.CreateMap<RecipeEdit, Recipe>()
                       .AfterMap((src, dest) =>
                       {
                           if (dest.Tags != null)
                           {
                               foreach (RecipeTag tag in dest.Tags)
                               {
                                   tag.RecipeId = dest.ID;
                               }
                           }
                       });

                    cfg.CreateMap<RecipeIngredientEdit, RecipeIngredientEdit>();
                    cfg.CreateMap<RecipeIngredientEdit, RecipeIngredient>()
                       .ForMember(x => x.Ingredient, opts => opts.Ignore());

                    cfg.CreateMap<RecipeIngredientData, RecipeIngredientEdit>();
                    cfg.CreateMap<TagData, TagEdit>();
                    cfg.CreateMap<TagEdit, Tag>();
                    cfg.CreateMap<TagEdit, RecipeTag>()
                        .ForMember(x => x.TagId, opt => opt.MapFrom(x => x.ID));

                    cfg.CreateMap<IngredientGroupEdit, IngredientGroupEdit>();
                    cfg.CreateMap<IngredientGroupEdit, IngredientsGroup>();
                    cfg.CreateMap<IngredientGroupData, IngredientGroupEdit>();
                    cfg.CreateMap<IngredientData, IngredientEdit>();
                });
    }
}
