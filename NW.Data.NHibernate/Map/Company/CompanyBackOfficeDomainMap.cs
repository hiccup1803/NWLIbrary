using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Company
{
   
    public class CompanyBackOfficeDomainMap : ClassMap<CompanyBackOfficeDomain>
    {
        public CompanyBackOfficeDomainMap()
        {
            Id(x => x.Id);
            Map(x => x.Domain);
            Map(x => x.CompanyId);
            Map(x => x.VoltronCompanyId);
            Map(x => x.CreateDate);
            Map(x => x.IsLive);

            Table("CompanyBackOfficeDomain");
        }
    }
}
