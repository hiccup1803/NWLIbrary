using NW.Core.Contracts.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Member
{
    public class FinancialInfoResult
    {
        public MemberBalanceResult MemberBalanceResult { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int PendingWithdrawCount { get; set; }

        public FinancialInfoMemberBonusResult MemberBonus { get; set; }
    }
}
