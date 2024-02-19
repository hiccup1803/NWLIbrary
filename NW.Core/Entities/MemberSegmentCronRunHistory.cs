using NW.Core.Entities.BackOffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberSegmentCronRunHistory : Entity<int>
    {
        public virtual int MemberSegmentId { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int? QueryResultCount { get; set; }
        public virtual int? DowngradeMemberCount { get; set; }
        public virtual int? UpgradeMemberCount { get; set; }
    }
}
