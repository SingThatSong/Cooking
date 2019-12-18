using Data.Context;
using Data.Model;
using ServiceLayer;
using ServiceLayer.DTO.MainPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cooking.ServiceLayer
{
    public class TagService : CRUDService<Tag>
    {
        public List<Tag> GetTagsByType(TagType tagType)
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            return context.Tags.Where(x => x.Type == tagType).ToList();
        }

        public List<string> GetTagNames()
        {
            using var context = new CookingContext(DatabaseService.DbFileName);
            return context.Tags.Select(x => x.Name).ToList();
        }
    }
}
