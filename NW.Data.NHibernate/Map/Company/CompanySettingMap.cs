using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using NW.Core.Entities;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Company{
    
    
    public class CompanySettingMap : ClassMap<NW.Core.Entities.CompanySetting> {

        public CompanySettingMap()
        {
			Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.CompanyId);
			Map(x => x.Value);
			Map(x => x.Mode);
            Map(x => x.KeyGroupId);

            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("CompanySetting");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
