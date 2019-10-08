using System;
using System.Collections.Generic;
using System.Text;

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
