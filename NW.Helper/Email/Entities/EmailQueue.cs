using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Helper.Email.Entities
{
    public class EmailQueue
    {
        public string ApiKey { get; set; }
        //public string TemplateID { get; set; }

        public string Subject { get; set; }
        public string BodyHtml { get; set; }

        public string From { get; set; }
        public string FromName { get; set; }
        public List<Recipient> Recipients { get; set; }
        
    }

    public class Recipient
    {
        public string ToAddress { get; set; }
        
    }
}
