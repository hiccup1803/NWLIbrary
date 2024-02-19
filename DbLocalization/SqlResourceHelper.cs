using System;
using System.Collections.Generic;
using System.Text;

namespace DbLocalization
{
    internal static class SqlResourceHelper
    {
        public static string ConnectionStringKey = "DbLocalization";
        public static string LocalizationDomainStringKey = "LocalizationDomain";
        public static string DefaultCulture = "en-GB";
        public static int ResourceTimeOutAsMinute = 60;
    }
}
