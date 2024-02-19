using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class PayfixPaymentProvider
    {
        public int PayfixRequestId { get; set; }
        public NewPaymentProvider NewPaymentProvider { get; set; }
    }
}
