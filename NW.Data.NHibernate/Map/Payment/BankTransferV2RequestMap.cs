using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class BankTransferV2RequestMap : ClassMap<NW.Core.Entities.Payment.BankTransferV2Request>
    {
        public BankTransferV2RequestMap()
        {
            Id(x => x.Id);
            Map(x => x.PaymentStatusType);
            Map(x => x.PaymentProviderId);
            Map(x => x.MemberId);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.UpdateBy);
            Map(x => x.UpdateAmount);
            Map(x => x.Amount);
            Map(x => x.TransferDate);
            Map(x => x.SenderIdentityNumber);
            Map(x => x.SenderFirstname);
            Map(x => x.SenderLastname);
            Map(x => x.RequestBankId);
            Map(x => x.FastEnabled);
            Map(x => x.ReceiverBankAccountId);
            Map(x => x.ReceiverIBAN);
            Map(x => x.ReceiverBankId);
            Map(x => x.ReceiverBranchCode);
            Map(x => x.ReceiverAccountNumber);
            Map(x => x.ReceiverFullname);
            Map(x => x.ReceiverReference);
            Map(x => x.CompanyId);
            Map(x => x.RecognisedAmount);

            Table("BankTransferV2Request");
        }
    }
}
