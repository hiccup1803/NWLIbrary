using NW.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Payment
{
    public class GenericPaymentResult
    {
        public int Success { get; set; }
        public UIActionType UIActionType { get; set; }
        public object Data { get; set; }
        public string ReferenceId { get; set; }
    }

    public class PaparaAccountResult
    {
        public int Id { get; set; }
        public string NameSurnameMasked { get; set; }
        public string AccountNumber { get; set; }
    }
}
