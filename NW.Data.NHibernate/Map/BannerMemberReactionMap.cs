using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map
{
    public class BannerMemberReactionMap : ClassMap<BannerMemberReaction>
    {

        public BannerMemberReactionMap()
        {
            Id(x => x.Id);
            Map(x => x.CMS_BannerId);
            Map(x => x.MemberId);
            Map(x => x.ReactionType);
            Map(x => x.CreateDate);

            Table("CMS_BannerMemberReaction");
        }
    }
}
