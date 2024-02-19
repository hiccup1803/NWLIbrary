using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Payment
{
    public class ParazulaAdditionalInfo
    {
        public string ParazulaAccountNumber { get; set; }
        public long Amount { get; set; }
    }
}
