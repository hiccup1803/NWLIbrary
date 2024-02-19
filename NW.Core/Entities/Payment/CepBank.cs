using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class CepBank : Entity<int>
    {
        public virtual int AgencyId { get; set; }
        public virtual string Name { get; set; }
        public virtual int StatusType { get; set; }
        public virtual string ViewTemplate { get; set; }
    }
}
