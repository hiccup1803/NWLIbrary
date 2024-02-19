using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class CepBankMap : ClassMap<NW.Core.Entities.Payment.CepBank>
    {
        public CepBankMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.StatusType);
            Map(x => x.ViewTemplate);
            Map(x => x.AgencyId);

            Table("CepBank");
        }
    }
}
