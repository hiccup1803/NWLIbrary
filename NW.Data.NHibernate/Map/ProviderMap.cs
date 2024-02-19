using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using NW.Core.Entities;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Company{
    
    
    public class ProviderMap : ClassMap<NW.Core.Entities.Provider> {

        public ProviderMap()
        {
			Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.ProviderTypeId);
            Map(x => x.VoltronProviderId);
            Map(x => x.CreateDate);
            Map(x => x.SystemName);

            References(x => x.ProviderType).Column("ProviderTypeId").ReadOnly();
            HasMany(x => x.ProviderSettings).KeyColumn("ProviderId").Inverse().Cascade.All();

            Table("Provider");
        }
    }
}
