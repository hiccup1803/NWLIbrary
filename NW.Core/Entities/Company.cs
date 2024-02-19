using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class Company : Entity<int> 
    {
        public virtual string Name { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int StatusType { get; set; }
    }
}
