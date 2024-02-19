using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberTracking : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual string Domain { get; set; }
        public virtual string AbsoluteUri { get; set; }
        public virtual string Ip { get; set; }
        public virtual DateTime CreateDate { get; set; }
    }
}
