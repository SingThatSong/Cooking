using System;

namespace Data.Model
{
    public class Entity
    {
        public Entity()
        {
            ID = Guid.NewGuid();
        }

        public Guid ID { get; set; }
    }
}
