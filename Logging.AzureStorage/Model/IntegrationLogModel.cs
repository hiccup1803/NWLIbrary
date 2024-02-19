using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logging.AzureStorage.Model
{
    public class IntegrationLogModel : TableEntity
    {
        public string URL { get; set; }
        public string Message { get; set; }
    }
}
