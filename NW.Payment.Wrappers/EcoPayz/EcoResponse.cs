using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NW.Payment.Wrappers.EcoPayz
{

    [XmlRoot(ElementName = "SVSPurchaseStatusNotificationResponse", Namespace = "", IsNullable = false)]
    [DataContract(Namespace = "")]
    [XmlInclude(typeof(TransactionResult))]
    [XmlInclude(typeof(Authentication))]
    public class EcoResponse
    {
        public TransactionResult TransactionResult { get; set; }
        public string Status { get; set; }
        
        public Authentication Authentication { get; set; }
    }

    [XmlRoot(ElementName = "TransactionResult", Namespace = "", IsNullable = false)]
    [DataContract(Namespace = "")]
    public class TransactionResult
    {
        public string Description { get; set; }
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "Authentication", Namespace = "", IsNullable = false)]
    [DataContract(Namespace = "")]
    public class Authentication
    {
        public string Checksum { get; set; }
    }
}
