using Data.Context;
using Data.Model;
using ServiceLayer.DTO.MainPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceLayer
{
    public static class TagService
    {
        public static List<TagSearch> GetSearchTagsByType(TagType tagType)
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                return MapperService.Mapper.ProjectTo<TagSearch>(context.Tags.Where(x => x.Type == tagType)).ToList();
            }
        }

        public static List<TagSearch> GetSearchTags()
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                return MapperService.Mapper.ProjectTo<TagSearch>(context.Tags).ToList();
            }
        }

        public static List<string> GetSearchTagNames()
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                return context.Tags.Select(x => x.Name).ToList();
            }
        }
    }
}
