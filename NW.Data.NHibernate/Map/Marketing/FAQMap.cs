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
    
    
    public class FAQMap : ClassMap<FAQ> {

        public FAQMap()
        {
			Id(x => x.Id);
            Map(x => x.Title,"Title");
            Map(x => x.BodyContent,"BodyContent");
            Map(x => x.CategoryId, "CategoryId");
            Map(x => x.LanguageId, "LanguageId");
            Map(x => x.CompanyId, "CompanyId");
            Map(x => x.ItemOrder, "ItemOrder");
            Map(x => x.Status, "Status");
            Map(x => x.CreateDate);

            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("FAQ");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}

