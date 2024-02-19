using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Payment
{
    public class CurrencyUpdateMap : ClassMap<NW.Core.Entities.Payment.CurrencyUpdate>
    {
        public CurrencyUpdateMap()
        {
			Id(x => x.Id);
			Map(x => x.IsActive);
            Map(x => x.CreateDate);
            Table("CurrencyUpdate");
        }
    }
}