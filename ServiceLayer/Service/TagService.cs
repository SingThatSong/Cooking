using AutoMapper;
using Cooking.Data.Context;
using Data.Context;
using Data.Model;
using ServiceLayer;
using System.Collections.Generic;
using System.Linq;

namespace Cooking.ServiceLayer
{
    public class TagService : CRUDService<Tag>
    {
        public TagService(IContextFactory contextFactory) : base(contextFactory)
        {

        }

        public List<T> GetTagsByType<T>(TagType tagType, IMapper mapper)
        {
            using var context = ContextFactory.Create();
            return mapper.ProjectTo<T>(context.Tags.Where(x => x.Type == tagType)).ToList();
        }

        public List<string> GetTagNames()
        {
            using var context = ContextFactory.Create();
            return context.Tags
                          .Where(x => x.Name != null)
                          .Select(x => x.Name!)
                          .ToList();
        }
    }
}
