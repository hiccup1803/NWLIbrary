using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class Entity<TPK> : IEntity<TPK>
    {
        public virtual TPK Id { get; set; }
    }
}
