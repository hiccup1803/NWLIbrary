using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class PayminoRequest2Map : ClassMap<NW.Core.Entities.Payment.PayminoRequest2>
    {
        public PayminoRequest2Map()
        {
            Id(x => x.Id);
            Map(x => x.StatusType);
            Map(x => x.RequestType);
            Map(x => x.PaymentMethod);
            Map(x => x.MemberId);
            Map(x => x.Firstname);
            Map(x => x.Lastname);
            Map(x => x.Amount);
            Map(x => x.CreateDate);
            Map(x => x.PaymentProviderTransactionId);
            Map(x => x.UpdateDate);
            Map(x => x.ResponseJson).Length(4001);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);
            Map(x => x.RecognisedAmount);

            Table("PayminoRequest2");
        }
    }
}
