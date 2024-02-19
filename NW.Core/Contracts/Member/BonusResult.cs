using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Member
{
    public class BonusResult
    {
        public List<Bonus> Data { get; set; }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }

    }
    public class Bonus
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TermsAndConditions { get; set; }
        public string ThumbnailImageURL { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CompleteAfterDays { get; set; }
        public int WageringContribution { get; set; }
        public decimal? FixedAmount { get; set; }
        public decimal? BonusPercentage { get; set; }
        public long? MinBonusAmount { get; set; }
        public long? MaxBonusAmount { get; set; }
        public int ConditionType { get; set; }
        public int MinCondition { get; set; }
        public string ResourceName { get; set; }

        public int EligibleAfterType { get; set; }
        public string EligibleAfterEntityIdList { get; set; }



        public long PossibleVoltronTransactionId { get; set; }
    }

    public class RelatedDepositTransaction
    {
        public long Id { get; set; }
        public long Amount { get; set; }
        public string Provider { get; set; }
        public int ProviderId { get; set; }
    }
}
