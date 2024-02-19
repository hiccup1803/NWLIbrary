using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class Transaction
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public long TransactionTypeId { get; set; }
        public string TransactionType { get { return TransactionTypeId.ToString(); } }
        public decimal TotalBalanceAfter { get; set; }
        public decimal BonusBalanceAfter { get; set; }

        public decimal Amount { get; set; }
        public int ProviderId { get; set; }


        public string ProviderName { get; set; }
        public string FinancialStatus { get; set; }
    }

    public enum TransactionType
    {
        GameWin=1,
        GamePlay=	2,
        GameCancel=	3,
        Deposit=	4,
        Withdrawal=	5,
        WithdrawCancel=	15,
        DepositBonus=	500
    }
}
