using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Payment
{
    public class CurrencyRateMap : ClassMap<NW.Core.Entities.Payment.CurrencyRate>
    {
        public CurrencyRateMap()
        {
			Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.UpdateId);
            References(x => x.FromCurrency).Column("FromCurrency");
            References(x => x.ToCurrency).Column("ToCurrency");
            Map(x => x.SellingRate);
            Map(x => x.Rate);
            Map(x => x.CreateDate);
            Table("CurrencyRate");
        }
    }
}