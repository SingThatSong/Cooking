using AutoMapper;
using Cooking.Data.Context;
using Cooking.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace Cooking.ServiceLayer
{
    public class TagService : CRUDService<Tag>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagService"/> class.
        /// </summary>
        /// <param name="contextFactory"></param>
        public TagService(IContextFactory contextFactory) : base(contextFactory)
        {

        }

        public List<T> GetTagsByType<T>(TagType tagType, IMapper mapper)
        {
            using CookingContext context = ContextFactory.Create();
            return mapper.ProjectTo<T>(GetCultureSpecificSet(context).Where(x => x.Type == tagType)).ToList();
        }

        public List<string> GetTagNames()
        {
            using CookingContext context = ContextFactory.Create();
            return GetCultureSpecificSet(context)
                          .Where(x => x.Name != null)
                          .Select(x => x.Name!)
                          .ToList();
        }
    }
}
