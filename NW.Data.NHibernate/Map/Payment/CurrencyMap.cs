using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Payment
{
    public class CurrencyMap : ClassMap<NW.Core.Entities.Payment.Currency>
    {
        public CurrencyMap()
        {
			Id(x => x.Id);
			Map(x => x.Name,"Currency");
			Map(x => x.IsBaseCurrency);
            Map(x => x.CreateDate);
            Table("Currency");
        }
    }
}