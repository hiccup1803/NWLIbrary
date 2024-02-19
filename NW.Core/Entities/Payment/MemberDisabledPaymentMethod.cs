using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class MemberDisabledPaymentMethod : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual int ProviderId { get; set; }
        public virtual DateTime CreateDate { get; set; }
    }
}
