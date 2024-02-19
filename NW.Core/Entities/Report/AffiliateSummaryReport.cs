using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Report
{
    public class AffiliateSummaryReport
    {
        public DateTime ReportDate { get; set; }
        public int RegisteredCount { get; set; }
        public int DepositCount { get; set; }
        public decimal TotalDeposit { get; set; }
        public decimal TotalBet { get; set; }
        public decimal TotalWin { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalBonus { get; set; }
        public decimal TotalCashback { get; set; }
        public decimal TotalWithdraw { get; set; }
        public decimal TotalWithdrawCancelled { get; set; }
        public decimal NetGameRevenue { get; set; }
        public decimal Income { get; set; }
    }
}
