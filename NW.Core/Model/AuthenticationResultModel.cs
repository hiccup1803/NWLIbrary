using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Model
{
    public class AuthenticationResultModel
    {
        public bool ForceToValidateWithPhoneDueToDeviceDifference { get; set; }
        public string Phone { get; set; }
        public string MaskedPhone { get; set; }

        public string CountryPhoneCode { get; set; }
        public string CodeToVerifyPhone { get; set; }
        public string CodeToVerifyTelegram { get; set; }
        public bool CodeSent { get; set; }
        public string CodeSentErrorMessage { get; set; }
        
    }

    public class TwoWayAuthModel
    {
        

        public string Email { get; set; }
        public string MaskedEmail { get; set; }

        public string Phone { get; set; }
        public string MaskedPhone { get; set; }

        
        public string Code { get; set; }

    }
}
