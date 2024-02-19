using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class NewBankTransferRequestMap : ClassMap<NW.Core.Entities.Payment.NewBankTransferRequest>
    {
        public NewBankTransferRequestMap()
        {
            Id(x => x.Id);
            Map(x => x.ProviderId);
            Map(x => x.PaymentStatusType);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.UpdateBy);
            Map(x => x.MemberId);
            Map(x => x.ReceiverBankId);
            Map(x => x.ReceiverNameSurname);
            Map(x => x.ReceiverNameSurnameMasked);
            Map(x => x.ReceiverBranch);
            Map(x => x.ReceiverBranchCode);
            Map(x => x.ReceiverAccountNumber);
            Map(x => x.ReceiverIBAN);
            Map(x => x.Amount);
            Map(x => x.UpdateAmount);
            Map(x => x.IdentityNumber);
            Map(x => x.SenderBankId);
            Map(x => x.BranchCode);
            Map(x => x.IBAN);
            Map(x => x.AccountNumber);
            Map(x => x.Note);
            //Map(x => x.TransferDate);
            Map(x => x.TransferWayType);
            Map(x => x.SenderFullName);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);
            Map(x => x.IsEFT);
            Map(x => x.CompanyId);

            References(x => x.SenderBank).Column("SenderBankId").ReadOnly();
            References(x => x.ReceiverBank).Column("ReceiverBankId").ReadOnly();
            HasMany(x => x.ProviderHistory).KeyColumn("BankTransferRequestId").Inverse().Cascade.All();

            Table("NewBankTransferRequest");
        }
    }
}
