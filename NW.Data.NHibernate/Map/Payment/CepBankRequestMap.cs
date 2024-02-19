using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class CepBankRequestMap : ClassMap<NW.Core.Entities.Payment.CepBankRequest>
    {
        public CepBankRequestMap()
        {
            Id(x => x.Id);
            Map(x => x.PaymentProviderId);
            Map(x => x.CepBankId);
            Map(x => x.MemberId);
            Map(x => x.PaymentTransactionId);
            Map(x => x.SenderId);
            Map(x => x.ReceipientId);
            Map(x => x.SenderPhone);
            Map(x => x.ReceipientPhone);
            Map(x => x.ReceipientBirthday);
            Map(x => x.Password);
            Map(x => x.Amount);
            Map(x => x.CreateDate);
            Map(x => x.PaymentStatusType);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);
            Map(x => x.ProviderRefId);


            References(cbr => cbr.CepBank).Column("CepBankId").ReadOnly();
            References(cbr => cbr.Member).Column("MemberId").ReadOnly();

            Table("CepBankRequest");
        }
    }
}
