using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Payment
{


    public class DepositSummaryModel
    {
        public DateTime CreateDate { get; set; }
        public string Amount { get; set; }
        public int StatusType { get; set; }
        public string ProviderName { get; set; }
    }
}
