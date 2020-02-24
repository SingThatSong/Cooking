﻿using AutoMapper;
using Cooking.Data.Context;
using Cooking.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace Cooking.ServiceLayer
{
    /// <summary>
    /// Service for work with tags.
    /// </summary>
    public class TagService : CRUDService<Tag>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagService"/> class.
        /// </summary>
        /// <param name="contextFactory">Factory for creating <see cref="CookingContext"/> instances.</param>
        /// <param name="cultureProvider">Culture provider for determining which culture enities should belong to.</param>
        public TagService(IContextFactory contextFactory, ICurrentCultureProvider cultureProvider, IMapper mapper)
            : base(contextFactory, cultureProvider, mapper)
        {
        }

        /// <summary>
        /// Get projected list of tags filtered by type.
        /// </summary>
        /// <typeparam name="T">Type to project result to.</typeparam>
        /// <param name="tagType">Type to filter tags.</param>
        /// <returns>Projected list of tags filtered by type.</returns>
        // TODO: remove after introduction of IQueryable parameter in CRUDService
        public List<T> GetTagsByTypeProjected<T>(TagType tagType)
        {
            using CookingContext context = ContextFactory.Create();
            return Mapper.ProjectTo<T>(GetCultureSpecificSet(context).Where(x => x.Type == tagType)).ToList();
        }

        /// <summary>
        /// Get names of all tags.
        /// </summary>
        /// <returns>Names of all tags.</returns>
        public List<string> GetTagNames()
        {
            using CookingContext context = ContextFactory.Create();
            return GetCultureSpecificSet(context)
                          .Where(x => x.Name != null)
                          .Select(x => x.Name!)
                          .ToList();
        }

        /// <summary>
        /// Get menu items for tags.
        /// </summary>
        /// <returns>Names for menu items.</returns>
        public List<string> GetMenuTags()
        {
            using CookingContext context = ContextFactory.Create();
            return GetCultureSpecificSet(context)
                          .Where(x => x.Name != null && x.IsInMenu)
                          .Select(x => x.Name!)
                          .ToList();
        }
    }
}
