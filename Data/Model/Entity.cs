using System;
using System.Collections.Generic;
using System.Text;

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
