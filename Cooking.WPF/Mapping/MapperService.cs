using System;
using System.ComponentModel;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using AutoMapper.Internal;
using Cooking.Data.Model;
using Cooking.Data.Model.Plan;
using Cooking.WPF.DTO;
using Cooking.WPF.Services;
using Cooking.WPF.Validation;
using Cooking.WPF.ViewModels;
using Prism.Ioc;

namespace Cooking;

/// <summary>
/// Service for object mapping and projections.
/// </summary>
internal static class MapperService
{
    /// <summary>
    /// Register automapper container instance.
    /// </summary>
    /// <param name="containerRegistry">Container registry to register automapper.</param>
    /// <param name="container">Container provider to use as resolver for automapper.</param>
    /// <returns>Same IContainerRegistry instance for chaining.</returns>
    public static IContainerRegistry UseAutomapper(this IContainerRegistry containerRegistry, IContainerProvider container)
    {
        // Register services
        containerRegistry.RegisterInstance<IMapper>(
            new Mapper(
                CreateMapper(),
                container.Resolve<IContainerExtension>().Resolve
            )
        );

        return containerRegistry;
    }

    /// <summary>
    /// Create Autmapper Mapper provider.
    /// </summary>
    /// <returns>Instance of <see cref="IConfigurationProvider"/>.</returns>
    private static IConfigurationProvider CreateMapper()
        => new MapperConfiguration(cfg =>
        {
            cfg.AddCollectionMappers();

            // Disable validation while mapping and map
            cfg.ForAllMaps((_, mappingExpression) =>
            {
                mappingExpression.ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));

                mappingExpression.BeforeMap((_, dest) =>
                {
                    if (dest is IDataErrorInfo)
                    {
                        IValidationTemplate? template = dest.GetValidationTemplate();
                        template?.DisableValidation();
                    }
                });

                mappingExpression.AfterMap((_, dest) =>
                {
                    if (dest is IDataErrorInfo)
                    {
                        IValidationTemplate? template = dest.GetValidationTemplate();
                        if (template != null)
                        {
                            template.EnableValidation();
                            template.ForceValidate();
                        }
                    }
                });
            });

            // Base mapping for db-dto mappings
            cfg.CreateMap<Entity, Entity>()
               .EqualityComparison((a, b) => a.ID == b.ID)
               .IncludeAllDerived();

            cfg.CreateMap<Entity, EntityNotify>()
               .EqualityComparison((a, b) => a.ID == b.ID)
               .IncludeAllDerived();

            // Map created recipe to displayed in list
            cfg.CreateMap<RecipeEdit, RecipeListViewDto>()

               // It is a new recipe, so we just set LastCooked to infinity
               .ForMember(x => x.LastCooked, opts => opts.MapFrom(_ => int.MaxValue));

            // Backup dto for editing
            cfg.CreateMap<RecipeEdit, RecipeEdit>()
               .ForMember(x => x.LastCooked, opts => opts.Ignore());

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
               .ForMember(x => x.LastCooked, opts => opts.Ignore())
               .AfterMap<RecipeDtoConverter>();

            // Project Recipe form db to displayed in week generation
            cfg.CreateMap<Recipe, DayPlanRecipe>();

            // Project Recipe from db to displayed in recipe view
            cfg.CreateMap<Recipe, RecipeEdit>()
               .ForMember(x => x.LastCooked, opts => opts.Ignore())
               .AfterMap<RecipeConverter>()
               .ReverseMap();

            // Update ingredients group in recipe
            cfg.CreateMap<IngredientGroupEdit, IngredientsGroup>()
               .ReverseMap();

            // Project day to display
            cfg.CreateMap<Day, DayDisplay>();

            // Project enities for editing
            cfg.CreateMap<Tag, TagEdit>()
               .ForMember(x => x.IsChecked, opts => opts.Ignore())
               .ForMember(x => x.CanBeRemoved, opts => opts.Ignore())
               .ReverseMap();

            cfg.CreateMap<Ingredient, IngredientEdit>()
               .ReverseMap();
        });
}
