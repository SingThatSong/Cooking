using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Cooking.Data.Model;
using Cooking.Data.Model.Plan;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using System;
using System.Linq;

// Class contains imperative mapping rules, null-checks are irrelevant
#pragma warning disable CS8604

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
                    cfg.AddCollectionMappers();

                    // Base mapping for db-dto mappings
                    cfg.CreateMap<Entity, Entity>()
                       .EqualityComparison((a, b) => a.ID == b.ID)
                       .IncludeAllDerived();

                    // Map created recipe to displayed in list
                    cfg.CreateMap<RecipeEdit, RecipeListViewDto>()

                       // It is a new recipe, so we just set LastCooked to infinity
                       .ForMember(x => x.LastCooked, opts => opts.MapFrom(_ => int.MaxValue));

                    // Backup dto for editing
                    cfg.CreateMap<GarnishEdit, GarnishEdit>();
                    cfg.CreateMap<RecipeEdit, RecipeEdit>();
                    cfg.CreateMap<TagEdit, TagEdit>();
                    cfg.CreateMap<IngredientEdit, IngredientEdit>();
                    cfg.CreateMap<MeasureUnit, MeasureUnit>();
                    cfg.CreateMap<RecipeIngredientEdit, RecipeIngredientEdit>();
                    cfg.CreateMap<IngredientGroupEdit, IngredientGroupEdit>();

                    cfg.CreateMap<RecipeIngredient, RecipeIngredientEdit>()
                       .ReverseMap()

                        // Do not map ingredient and measure unit object, it's not new, so db will fail on attempt to create duplicate
                       .ForMember(x => x.Ingredient, opts => opts.Ignore())
                       .ForMember(x => x.MeasureUnit, opts => opts.Ignore())
                       .ForMember(x => x.MeasureUnitID, opts => opts.MapFrom(x => x.MeasureUnit != null ? (Guid?)x.MeasureUnit.ID : null));

                    // Project Recipe from db to displayed in lists
                    cfg.CreateMap<Recipe, RecipeListViewDto>()
                       .AfterMap<RecipeDtoConverter>();

                    // Project Recipe from db to displayed in recipe view
                    cfg.CreateMap<Recipe, RecipeEdit>()
                       .ForMember(x => x.Tags, opt => opt.MapFrom(x => x.Tags.Select(t => t.Tag)))
                       .AfterMap<RecipeConverter>();

                    // Update db entry
                    cfg.CreateMap<RecipeEdit, Recipe>()
                       .AfterMap((_, dest) =>
                       {
                           if (dest.Tags != null)
                           {
                               foreach (RecipeTag tag in dest.Tags)
                               {
                                   tag.RecipeId = dest.ID;
                               }
                           }
                       });

                    // Update ingredients group in recipe
                    cfg.CreateMap<IngredientGroupEdit, IngredientsGroup>()
                       .ReverseMap();

                    // Project day to display
                    cfg.CreateMap<Day, DayDisplay>();

                    // Project enities for editing
                    cfg.CreateMap<Garnish, GarnishEdit>()
                       .ReverseMap();
                    cfg.CreateMap<Tag, TagEdit>()
                       .ReverseMap();
                    cfg.CreateMap<Ingredient, IngredientEdit>()
                       .ReverseMap();

                    // Recipe tag mapping
                    cfg.CreateMap<TagEdit, RecipeTag>()
                        .ForMember(x => x.TagId, opt => opt.MapFrom(x => x.ID));
                });
    }
}

#pragma warning restore CS8604