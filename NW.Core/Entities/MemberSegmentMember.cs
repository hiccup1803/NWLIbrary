using NW.Core.Entities.BackOffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberSegmentMember : Entity<int>
    {
        public virtual MemberSegment MemberSegment { get; set; }
        public virtual int MemberSegmentId { get; set; }
        public virtual int MemberId { get; set; }
        public virtual int StatusType { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime? UpdateDate { get; set; }
    }
}
