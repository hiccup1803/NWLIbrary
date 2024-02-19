using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class Banner : Entity<int>
    {
        public virtual string Title { get; set; }
        public virtual int BannerPlace { get; set; }
        public virtual int BannerType { get; set; }
        public virtual int LanguageId { get; set; }
        public virtual string BannerUrl { get; set; }
        public virtual string MobileUrl { get; set; }
        public virtual string BannerImagePath { get; set; }
        public virtual string MobileBannerImagePath { get; set; }
        public virtual int? MemberTagFilterId { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual bool IsVip { get; set; }
        public virtual bool Active { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual int? BannerDay { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual TimeSpan? StartTime { get; set; }
        public virtual TimeSpan? EndTime { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int StatusType { get; set; }
        public virtual Language Language { get; set; }
        public virtual Company Company { get; set; }
        public virtual MemberTagFilter MemberTagFilter { get; set; }
        public virtual string UsernameList { get; set; }
        public virtual int BannerUsernameFilterType { get; set; }
        public virtual IList<MemberSegment> MemberSegments { get; set; }
    }
}
