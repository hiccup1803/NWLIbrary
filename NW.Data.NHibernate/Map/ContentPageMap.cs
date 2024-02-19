using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using NW.Core.Entities;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Maps{
    
    
    public class ContentPageMap : ClassMap<NW.Core.Entities.ContentPage> {

        public ContentPageMap()
        {
			Id(x => x.Id);
			Map(x => x.PageId);
            Map(x => x.PageName);
			Map(x => x.Title);
            Map(x => x.Keywords);
            Map(x => x.Description);
            Map(x => x.Content).Length(4001);
            Map(x => x.LanguageId);
            Map(x => x.CompanyId);
            Map(x => x.CreatedDate);

            //Join("SEO_Pages", join =>
            //{
                
            //    join.KeyColumn("Id");
            //    join.Map(prop => prop.PageName,"Name");
            //});
            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("SEO_PageDetails");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}

