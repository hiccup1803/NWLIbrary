using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberConnection : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual string ConnectionId { get; set; }

        public virtual string Username { get; set; }
        public virtual string CurrentUrl { get; set; }
        public virtual string Device { get; set; }
        public virtual bool IsOnline { get; set; }
        public virtual string IP { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }

    }
}
