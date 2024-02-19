using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using log4net.Core;
using log4net.Layout.Pattern;

namespace Logging.Converters
{
    public class Member : PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, LoggingEvent loggingEvent)
        {
            string name = null;
            HttpContext context = HttpContext.Current;
            if (context != null && context.User != null && context.User.Identity.IsAuthenticated)
            {
                name = context.User.Identity.Name;
            }
            writer.Write(name);
        }
    }

    public class Hostname : PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, LoggingEvent loggingEvent)
        {
            string name=null;
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                name = context.Request.Url.ToString();
            }
            writer.Write(name);
        }
    }
}
