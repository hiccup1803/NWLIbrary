using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Member
{
    public class FinancialInfoMemberBonusResult
    {
        public long WageredAmount { get; set; }
        public long WageringAmount { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
