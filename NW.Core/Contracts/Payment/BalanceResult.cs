using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Payment
{
    public class BalanceResult
    {
        public long VReferenceId { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int ResponseCode { get; set; }
        public Int64 BonusBalance { get; set; }
        public Int64 AllocatedBalance { get; set; }
        public Int64 RealBalance { get; set; }
        public Int64 TotalBalance { get; set; }

    }
    public class MemberBalanceResult
    {
        public long VReferenceId { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int ResponseCode { get; set; }


        public Int64 WithdrawableBalance { get; set; }
        public Int64 BonusBalance { get; set; }
        public Int64 SportsBonusBalance { get; set; }
        public Int64 AllocatedBalance { get; set; }
        public Int64 TotalBalance { get; set; }

        public string Currency { get; set; }


    }
}