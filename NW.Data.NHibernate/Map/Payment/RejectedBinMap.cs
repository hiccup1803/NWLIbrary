using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class RejectedBinMap : ClassMap<NW.Core.Entities.Payment.RejectedBin>
    {
        public RejectedBinMap()
        {
            Id(x => x.Id);
            Map(x => x.FilteredCardNo);
            Map(x => x.CreatedDate);
            Table("CreditCard_RejectedBin");
        }
    }
}
