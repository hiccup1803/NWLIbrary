using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logging.AzureStorage.Model
{
    public class ExceptionLogModel : TableEntity
    {
        public string URL { get; set; }
        public string Message { get; set; }
        public string InnerMessage { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
    }
}
