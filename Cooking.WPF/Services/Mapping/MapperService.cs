using AutoMapper;
using Cooking.Data.Model;
using Cooking.Data.Model.Plan;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cooking
{
    /// <summary>
    /// Service for object mapping and projections.
    /// </summary>
    internal static class MapperService
    {
        /// <summary>
        /// Create Autmapper Mapper provider.
        /// </summary>
        /// <returns>Instance of <see cref="IConfigurationProvider"/>.</returns>
        public static IConfigurationProvider CreateMapper()
            => new MapperConfiguration(cfg =>
                {
                    cfg.AllowNullDestinationValues = true;

                    // Map created recipe to displayed in list
                    cfg.CreateMap<RecipeEdit, RecipeListViewDto>()

                       // It is a new recipe, so we just set LastCooked to infinity
                       .ForMember(x => x.LastCooked, opts => opts.MapFrom(_ => int.MaxValue));

                    // Backup dto for editing
                    cfg.CreateMap<GarnishEdit, GarnishEdit>();
                    cfg.CreateMap<RecipeEdit, RecipeEdit>();
                    cfg.CreateMap<TagEdit, TagEdit>();
                    cfg.CreateMap<IngredientEdit, IngredientEdit>();

                    // Base mapping for db-dto mappings
                    cfg.CreateMap<Entity, Entity>();

                    // Project Recipe from db to displayed in lists
                    cfg.CreateMap<Recipe, RecipeListViewDto>()
                       .AfterMap<RecipeDtoConverter>();

                    // Project Recipe from db to displayed in recipe view
                    cfg.CreateMap<Recipe, RecipeEdit>()
                       .IncludeBase<Entity, Entity>()
                       .ForMember(x => x.Tags, opt => opt.MapFrom(x => x.Tags.Select(t => t.Tag)))
                       .AfterMap<RecipeConverter>();

                    // Cleanup below
                    // -----------------------
                    cfg.CreateMap<Garnish, GarnishEdit>().ReverseMap();
                    cfg.CreateMap<Day, DayEdit>();
                    cfg.CreateMap<Tag, TagEdit>();
                    cfg.CreateMap<Ingredient, IngredientEdit>().ReverseMap();
                    cfg.CreateMap<RecipeIngredient, RecipeIngredientEdit>();
                    cfg.CreateMap<IngredientsGroup, IngredientGroupEdit>();

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

                    cfg.CreateMap<TagEdit, Tag>();
                    cfg.CreateMap<TagEdit, RecipeTag>()
                        .ForMember(x => x.TagId, opt => opt.MapFrom(x => x.ID));

                    cfg.CreateMap<IngredientGroupEdit, IngredientGroupEdit>();
                    cfg.CreateMap<IngredientGroupEdit, IngredientsGroup>();
                });
    }
}
