using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class MemberDisabledPaymentMethodMap : ClassMap<NW.Core.Entities.Payment.MemberDisabledPaymentMethod>
    {
        public MemberDisabledPaymentMethodMap()
        {
            Id(x => x.Id);
            Map(x => x.MemberId);
            Map(x => x.ProviderId);
            Map(x => x.CreateDate);

            Table("MemberDisabledPaymentMethod");
        }
    }
}
