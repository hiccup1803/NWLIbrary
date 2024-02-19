using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class ReyPayCMTRequest : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual int StatusType { get; set; }
        public virtual long Amount { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string ReferenceCode { get; set; }
        public virtual string ProcessId { get; set; }
        public virtual string ResultData { get; set; }
    }
}
