using NW.Core.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Payment
{
    public class BankTransferAdditionalInfo
    {
        public long VoltronTransactionId { get; set; }
        public DateTime UpdateDate { get; set; }
        public Dictionary<int, BankAccount> BankAccountList { get; set; }
        public long Amount { get; set; }
    }
}
