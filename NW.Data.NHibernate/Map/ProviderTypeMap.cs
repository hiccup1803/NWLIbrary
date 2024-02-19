using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using NW.Core.Entities;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Company{
    
    
    public class ProviderTypeMap : ClassMap<NW.Core.Entities.ProviderType> {

        public ProviderTypeMap()
        {
			Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.CreateDate);
            

            Table("ProviderType");
        }
    }
}
