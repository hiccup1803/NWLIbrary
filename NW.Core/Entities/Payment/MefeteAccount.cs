using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class MefeteAccount : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual string Firstname { get; set; }
        public virtual string Lastname { get; set; }
        public virtual int StatusType { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string Currency { get; set; }
        public virtual Member Member { get; set; }

    }
}
