using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class PayfixTransferRequestMap : ClassMap<NW.Core.Entities.Payment.PayfixTransferRequest>
    {
        public PayfixTransferRequestMap()
        {
            Id(x => x.Id);
            Map(x => x.PaymentStatusType);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.UpdateBy);
            Map(x => x.MemberId);
            Map(x => x.PayfixTransferPayfixAccountId);
            Map(x => x.Amount);
            Map(x => x.UpdateAmount);
            Map(x => x.AccountNumber);
            Map(x => x.SenderFullName);
            Map(x => x.CompanyId);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);
            Map(x => x.ProviderId);
            Map(x => x.Note);
            Map(x => x.ReferenceId);
            Map(x => x.RecognisedAmount);

            References(m => m.Member).Column("MemberId").ReadOnly();
            References(m => m.PayfixTransferPayfixAccount).Column("PayfixTransferPayfixAccountId").ReadOnly();

            Table("PayfixTransferRequest");
        }
    }
}
