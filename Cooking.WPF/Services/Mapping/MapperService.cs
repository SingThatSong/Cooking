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
                       .EqualityComparison((a, b) => a.ID == b.ID);

                    // Map created recipe to displayed in list
                    cfg.CreateMap<RecipeEdit, RecipeListViewDto>()
                       .IncludeBase<Entity, Entity>()

                       // It is a new recipe, so we just set LastCooked to infinity
                       .ForMember(x => x.LastCooked, opts => opts.MapFrom(_ => int.MaxValue));

                    // Backup dto for editing
                    cfg.CreateMap<GarnishEdit, GarnishEdit>()
                       .IncludeBase<Entity, Entity>();

                    cfg.CreateMap<RecipeEdit, RecipeEdit>()
                       .IncludeBase<Entity, Entity>();

                    cfg.CreateMap<TagEdit, TagEdit>()
                       .IncludeBase<Entity, Entity>();

                    cfg.CreateMap<IngredientEdit, IngredientEdit>()
                       .IncludeBase<Entity, Entity>();

                    cfg.CreateMap<MeasureUnit, MeasureUnit>()
                       .IncludeBase<Entity, Entity>()
                       .EqualityComparison((a, b) => a.ID == b.ID);

                    cfg.CreateMap<RecipeIngredient, RecipeIngredientEdit>()
                       .EqualityComparison((a, b) => a.ID == b.ID)
                       .IncludeBase<Entity, Entity>()
                       .ReverseMap()
                       .EqualityComparison((a, b) => a.ID == b.ID)
                        // Do not map ingredient object, it's not new, so db will fail on attempt to create duplicate
                       .ForMember(x => x.Ingredient, opts => opts.Ignore())
                       .ForMember(x => x.MeasureUnit, opts => opts.Ignore())
                       .ForMember(x => x.MeasureUnitGuid, opts => opts.MapFrom(x => x.MeasureUnit != null ? (Guid?)x.MeasureUnit.ID : null))
                       .IncludeBase<Entity, Entity>();

                    // Project Recipe from db to displayed in lists
                    cfg.CreateMap<Recipe, RecipeListViewDto>()
                       .IncludeBase<Entity, Entity>()
                       .AfterMap<RecipeDtoConverter>();

                    // Project Recipe from db to displayed in recipe view
                    cfg.CreateMap<Recipe, RecipeEdit>()
                       .IncludeBase<Entity, Entity>()
                       .ForMember(x => x.Tags, opt => opt.MapFrom(x => x.Tags.Select(t => t.Tag)))
                       .AfterMap<RecipeConverter>();

                    // Update db entry
                    cfg.CreateMap<RecipeEdit, Recipe>()
                       .IncludeBase<Entity, Entity>()
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
                       .EqualityComparison((a, b) => a.ID == b.ID);

                    // Cleanup below
                    // -----------------------
                    cfg.CreateMap<RecipeIngredientEdit, RecipeIngredientEdit>()
                       .IncludeBase<Entity, Entity>();
                    cfg.CreateMap<Garnish, GarnishEdit>().ReverseMap();
                    cfg.CreateMap<Day, DayEdit>();
                    cfg.CreateMap<Tag, TagEdit>();
                    cfg.CreateMap<Ingredient, IngredientEdit>().ReverseMap();
                    cfg.CreateMap<IngredientsGroup, IngredientGroupEdit>();

                    cfg.CreateMap<TagEdit, Tag>();
                    cfg.CreateMap<TagEdit, RecipeTag>()
                        .ForMember(x => x.TagId, opt => opt.MapFrom(x => x.ID));

                    cfg.CreateMap<IngredientGroupEdit, IngredientGroupEdit>();
                });
    }
}

#pragma warning restore CS8604