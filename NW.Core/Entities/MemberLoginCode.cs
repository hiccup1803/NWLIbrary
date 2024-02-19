using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberLoginCode : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual int StatusType { get; set; }
        public virtual string LoginCode { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int TryCount { get; set; }
        public virtual int TryLoginCodeCount { get; set; }
    }
}
