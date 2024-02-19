using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Helper.Email.Entities
{
    public class Template
    {
        public string ApiKey { get; set; }
        public string Html { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public string TemplateName { get; set; }

    }
}
