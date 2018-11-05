using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Data.Model
{
    public class Tag
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public TagType Type { get; set; }
        public string Color { get; set; }
    }
}
