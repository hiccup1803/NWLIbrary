using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class GenericDepositRequestMap : ClassMap<NW.Core.Entities.Payment.GenericDepositRequest>
    {
        public GenericDepositRequestMap()
        {
            Id(x => x.Id);
            Map(x => x.PaymentStatusType);
            Map(x => x.ProviderId);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.UpdateBy);
            Map(x => x.MemberId);
            Map(x => x.Amount);
            Map(x => x.UpdateAmount);
            Map(x => x.Payload);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);
            Map(x => x.Note);
            Map(x => x.CompanyId);
            Map(x => x.RecognisedAmount);

            Table("GenericDepositRequest");
        }
    }
}
