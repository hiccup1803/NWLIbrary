using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class WithdrawRequestBankTransferMap : ClassMap<NW.Core.Entities.Payment.WithdrawRequestBankTransfer>
    {
        public WithdrawRequestBankTransferMap()
        {
            Id(x => x.Id);
            Map(x => x.MemberId);
            Map(x => x.WithdrawStatusType);
            Map(x => x.PaymentTransactionId);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.Bank);
            Map(x => x.IBAN);
            Map(x => x.BranchCode);
            Map(x => x.AccountNumber);
            Map(x => x.Currency);
            Map(x => x.Amount);

            Table("WithdrawRequestBankTransfer");
        }
    }
}
