using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Payment.Wrappers.BP
{
    public class BPResponse
    {
        public string Token { get; set; }
        public string RedirectURL { get; set; }
        public string TransactionId { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }

    }
}
