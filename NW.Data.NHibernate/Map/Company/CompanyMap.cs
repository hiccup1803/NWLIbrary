using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Company
{
    public class CompanyMap : ClassMap<Core.Entities.Company>
    {
        public CompanyMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.StatusType).Column("Status");
            Map(x => x.CreateDate);

            Table("Company");
        }
    }
}
