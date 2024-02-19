using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using NW.Core.Entities;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Company{
    
    
    public class ProviderSettingMap : ClassMap<NW.Core.Entities.ProviderSetting> {

        public ProviderSettingMap()
        {
			Id(x => x.Id);
            Map(x => x.CompanyId);
            Map(x => x.ProviderId);
            Map(x => x.Value);
            Map(x => x.Name);
            Map(x => x.IsProduction);
            Map(x => x.Mode);

            References(x => x.Company).Column("CompanyId").ReadOnly();
            References(x => x.Provider).Column("ProviderId").ReadOnly();

            Table("ProviderSetting");
        }
    }
}
