using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Model.KafkaModel
{
    public class LoginFailedKafkaModel
    {
        public int CompanyId { get; set; }
        public int MemberId { get; set; }
        public string UnformattedUsername { get; set; }
        public string Url { get; set; }
        public string ErrorMessage { get; set; }
    }
}
