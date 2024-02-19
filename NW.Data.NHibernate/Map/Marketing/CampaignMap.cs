using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using NW.Core.Entities;
using FluentNHibernate.Mapping;
using NW.Core.Entities.Marketing;

namespace NW.Data.NHibernate.Maps{
    
    
    public class CampaignMap : ClassMap<Campaign> {

        public CampaignMap()
        {
			Id(x => x.Id);
            Map(x => x.UID);
            Map(x => x.Name,"CampaignName");
            Map(x => x.CreateDate);

            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("MarketingCampaign");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}

