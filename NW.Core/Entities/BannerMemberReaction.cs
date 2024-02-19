using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class BannerMemberReaction : Entity<int>
    {
        public virtual int CMS_BannerId { get; set; }
        public virtual int MemberId { get; set; }
        public virtual int ReactionType { get; set; }
        public virtual DateTime CreateDate { get; set; }
    }
}
