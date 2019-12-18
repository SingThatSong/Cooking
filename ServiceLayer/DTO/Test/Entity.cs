using System;

namespace TEST
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
