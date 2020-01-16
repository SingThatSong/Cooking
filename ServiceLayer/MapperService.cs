using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Cooking.ServiceLayer;
using Cooking.ServiceLayer.Projections;
using Cooking.Data.Model;
using Cooking.Data.Model.Plan;
using System;
using System.Linq;

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
                cfg.AllowNullCollections = true;

                cfg.AddCollectionMappers();

                cfg.CreateMap<Entity, Entity>();

                cfg.CreateMap<Recipe, Recipe>().IncludeBase<Entity, Entity>();

                cfg.CreateMap<RecipeTag, RecipeTag>();
                cfg.CreateMap<Tag, Tag>().IncludeBase<Entity, Entity>();
                cfg.CreateMap<IngredientsGroup, IngredientsGroup>()
                   .IncludeBase<Entity, Entity>()
                   .EqualityComparison((a, b) => a.ID == b.ID);
                cfg.CreateMap<RecipeIngredient, RecipeIngredient>().IncludeBase<Entity, Entity>();
                cfg.CreateMap<Ingredient, Ingredient>().IncludeBase<Entity, Entity>();

                cfg.CreateMap<Recipe, RecipeSlim>().ReverseMap();
                cfg.CreateMap<Recipe, RecipeFull>()
                   .ForMember(x => x.Tags, opt => opt.MapFrom(x => x.Tags.Select(t => t.Tag)));

                cfg.CreateMap<RecipeFull, Recipe>()
                   .ForMember(x => x.Tags, opt => opt.Ignore())
                   .AfterMap((src, dest, context) => dest.Tags = src.Tags.Select(x => new RecipeTag()
                   {
                       //Recipe = dest,
                       RecipeId = dest.ID,
                       //Tag = context.Mapper.Map<Tag>(x),
                       TagId = x.ID
                   }).ToList()
                   );

                cfg.CreateMap<IngredientsGroup, IngredientGroupData>();
                cfg.CreateMap<RecipeIngredient, RecipeIngredientData>();
                cfg.CreateMap<Tag, TagData>().ReverseMap();

                cfg.CreateMap<Ingredient, Ingredient>();
                cfg.CreateMap<Ingredient, IngredientData>();

                cfg.CreateMap<Garnish, Garnish>();
            }).CreateMapper();
        }
    }
}
