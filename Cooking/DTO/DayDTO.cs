using System;

namespace Cooking.DTO
{
    public class DayDTO
    {
        public Guid? ID { get; set; }
        public Guid? DinnerID { get; set; }
        public RecipeDTO Dinner { get; set; }
    }
}