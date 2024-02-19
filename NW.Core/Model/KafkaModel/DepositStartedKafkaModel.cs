using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Model.KafkaModel
{
    public class DepositStartedKafkaModel
    {
        public int CompanyId { get; set; }
        public string Username { get; set; }
        public int MemberId { get; set; }
        public long Amount { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
    }
}
