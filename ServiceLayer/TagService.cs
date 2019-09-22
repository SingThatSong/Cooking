using Data.Context;
using Data.Model;
using ServiceLayer.DTO.MainPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public static class TagService
    {
        public static List<TagServiceDto> GetTagsByType(TagType tagType)
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                return MapperService.Mapper.ProjectTo<TagServiceDto>(context.Tags.Where(x => x.Type == tagType)).ToList();
            }
        }

        public static async Task<Guid> CreateAsync(Tag tag)
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                await context.Tags.AddAsync(tag);
                await context.SaveChangesAsync();
                return tag.ID;
            }
        }

        public static List<TagServiceDto> GetTags()
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                return MapperService.Mapper.ProjectTo<TagServiceDto>(context.Tags).ToList();
            }
        }

        public static List<string> GetTagNames()
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                return context.Tags.Select(x => x.Name).ToList();
            }
        }

        public static async Task UpdateTagAsync(Tag tag)
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                var existing = await context.Tags.FindAsync(tag.ID);
                MapperService.Mapper.Map(tag, existing);
                await context.SaveChangesAsync();
            }
        }

        public static async Task DeleteAsync(Guid id)
        {
            using (var context = new CookingContext(DatabaseService.DbFileName))
            {
                var entity = await context.Tags.FindAsync(id);
                context.Tags.Remove(entity);
            }
        }
    }
}
