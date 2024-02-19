using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class CustomStuff : Entity<int>
    {
        public virtual string DataKey { get; set; }
        public virtual string Data { get; set; }
        public virtual DateTime? ExpiryDate { get; set; }
    }
}
