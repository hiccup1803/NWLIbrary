using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class AffiliateTypeMap : ClassMap<AffiliateType>
    {
        public AffiliateTypeMap()
        {
            Id(l => l.Id);
            Map(l => l.Name);

            Table("AffiliateType");
        }
    }
}
