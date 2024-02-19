using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Payment
{
    public class PEPAdditionalInfo
    {
        public string PEPAccountNumber { get; set; }
        public long Amount { get; set; }
    }
}
