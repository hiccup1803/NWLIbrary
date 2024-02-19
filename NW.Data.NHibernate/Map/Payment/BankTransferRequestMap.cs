using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class BankTransferRequestMap : ClassMap<NW.Core.Entities.Payment.BankTransferRequest>
    {
        public BankTransferRequestMap()
        {
            Id(x => x.Id);
            Map(x => x.PaymentStatusType);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.UpdateBy);
            Map(x => x.MemberId);
            Map(x => x.BankTransferBankAccountId);
            Map(x => x.Amount);
            Map(x => x.UpdateAmount);
            Map(x => x.IdentityNumber);
            Map(x => x.Bank);
            Map(x => x.BranchCode);
            Map(x => x.IBAN);
            Map(x => x.AccountNumber);
            Map(x => x.Note);
            Map(x => x.TransferDate);
            Map(x => x.TransferWayType);
            Map(x => x.SenderFullName);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);
            Map(x => x.CompanyId);

            Table("BankTransferRequest");
        }
    }
}
