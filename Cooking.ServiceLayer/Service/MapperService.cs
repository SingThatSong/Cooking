﻿using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Cooking.Data.Model;
using Cooking.Data.Model.Plan;
using System;

namespace ServiceLayer
{
    /// <summary>
    /// Mapper for updating database.
    /// </summary>
    internal static class MapperService
    {
        private static readonly Lazy<IMapper> MapperValue = new Lazy<IMapper>(CreateMapper);

        /// <summary>
        /// Gets mapper instance. Lazy-loaded.
        /// </summary>
        public static IMapper Mapper => MapperValue.Value;

        private static IMapper CreateMapper()
            => new MapperConfiguration(cfg =>
            {
                cfg.AllowNullDestinationValues = true;
                cfg.AllowNullCollections = true;

                cfg.AddCollectionMappers();

                // Ignore Culture changes in mapping
                // Why: projections should not load culture, so on update they will not know it. Keep Culture as it is in database.
                cfg.CreateMap<Entity, Entity>()
                   .ForMember(x => x.Culture, opts => opts.Ignore());

                cfg.CreateMap<IngredientsGroup, IngredientsGroup>()
                   .IncludeBase<Entity, Entity>()

                   // When mapping collections of values, use this to detect the same objects
                   .EqualityComparison((a, b) => a.ID == b.ID);

                cfg.CreateMap<RecipeIngredient, RecipeIngredient>()
                   .IncludeBase<Entity, Entity>()
                   .EqualityComparison((a, b) => a.ID == b.ID);

                // Refactor below
                // --------------------------
                cfg.CreateMap<Recipe, Recipe>().IncludeBase<Entity, Entity>();

                cfg.CreateMap<RecipeTag, RecipeTag>();
                cfg.CreateMap<Tag, Tag>().IncludeBase<Entity, Entity>();
                cfg.CreateMap<Ingredient, Ingredient>().IncludeBase<Entity, Entity>();
                cfg.CreateMap<Garnish, Garnish>().IncludeBase<Entity, Entity>();
            }).CreateMapper();
    }
}
