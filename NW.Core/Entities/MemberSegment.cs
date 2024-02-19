using NW.Core.Entities.BackOffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberSegment : Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual string FilterQueryJSON { get; set; }
        public virtual string FilterQuerySQL { get; set; }
        public virtual int? PowerUserId { get; set; }
        public virtual int StatusType { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime? UpdateDate { get; set; }
        public virtual PowerUser PowerUser { get; set; }
        public virtual string CronExpression { get; set; }
        public virtual IList<Banner> Banners { get; set; }
    }
}
