using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Member
{
    public class ActivatedBonusResult
    {
        public List<MemberBonus> Data { get; set; } 

        public bool IsSuccess { get; set; }
        public string Message { get; set; } 
    }

    public class MemberBonus
    {
        public int Id { get; set; }
        public int BonusId { get; set; }
        public int MemberId { get; set; }
        public int BonusStatusType { get; set; }
        public long BonusAmount { get; set; }
        public long RemainBonusAmount { get; set; }
        public long AllocatedAmount { get; set; }
        public long WageringAmount { get; set; }
        public long WageredAmount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? WageredDate { get; set; }
        public long? RelatedVoltronTransactionId { get; set; }
        public DateTime? EndingDate { get; set; }
    }
}