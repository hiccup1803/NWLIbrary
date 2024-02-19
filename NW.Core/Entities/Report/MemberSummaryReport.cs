using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Report
{
    public class MemberSummaryReport
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public DateTime RegisterDate { get; set; }


        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public decimal StartBalance { get; set; }
        public decimal EndBalance { get; set; }

        public decimal TotalCasinoBet { get; set; }
        public decimal TotalCasinoWin { get; set; }
        public decimal TotalCasinoCancel { get; set; }

        public decimal TotalSportsBet { get; set; }
        public decimal TotalSportsWin { get; set; }
        public decimal TotalSportsCancel { get; set; }

        public decimal TotalDeposit { get; set; }
        public int NoOfDeposit { get; set; }
        public decimal TotalCredit { get; set; }
        public int NoOfCredit { get; set; }
        public decimal TotalWithdraw { get; set; }
        public decimal TotalBonus { get; set; }
        public int NoOfBonus { get; set; }
        public decimal TotalCashback { get; set; }
        public int NoOfCashback { get; set; }


        public decimal TotalCreditPaidCash { get; set; }
        public int NoOfCreditPaidCash { get; set; }

        public decimal TotalCreditBack { get; set; }
        public int NoOfCreditBack { get; set; }

        public decimal TotalCreditDiskont { get; set; }
        public int NoOfCreditDiskont { get; set; }


        public decimal TotalNegativeAdjustment { get; set; }




        public decimal CurrentBalance { get; set; }
    }
}
