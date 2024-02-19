using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Payment
{
    public class JetonAdditionalInfo
    {
        public string JetonAccountNumber { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
    }
}
