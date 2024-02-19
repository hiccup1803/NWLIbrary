using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.BackOffice
{
    public class BackOfficeRole : Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual int StatusType { get; set; }
        public virtual DateTime CreateDate { get; set; }
    }
}
