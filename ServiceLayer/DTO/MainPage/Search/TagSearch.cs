using Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer.DTO.MainPage
{
    public sealed class TagServiceDto : Entity
    {
        public string Name { get; set; }
        public TagType Type { get; set; }
        public string Color { get; set; }
    }
}
