using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberGameHistory : Entity<int>
    {
        public virtual int MemberId { get; set; }
        public virtual int GameId { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual bool PFF { get; set; }
        public virtual string SessionId { get; set; }
    }
}
