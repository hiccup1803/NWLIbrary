using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class BPCPaymentMethodType : Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual int VoltronProviderId { get; set; }
    }
}
