using Logging.AzureStorage.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.AzureStorage
{
    public class AzureStorageLogger : IDisposable
    {
        CloudTable CloudTable { get; set; }
        TableBatchOperation TableBatchOperation { get; set; }
        LogType LogType { get; set; }
        DateTime RequestDate { get; set; }
        public AzureStorageLogger(LogType logType)
        {
            LogType = logType;
            RequestDate = DateTime.UtcNow;

            try
            {
                string connString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable = tableClient.GetTableReference(LogType.ToString());
                CloudTable.CreateIfNotExists();

                TableBatchOperation = new TableBatchOperation();

            }
            catch (Exception ex) { }
        }

        public void Log(ITableEntity tableEntity)
        {
            try
            {
                TableBatchOperation.Insert(tableEntity);
            }
            catch (Exception ex) { }
        }
        public void LogIntegrationLog(string provider, string url, string message)
        {
            if (LogType == AzureStorage.LogType.IntegrationLog)
            {
                Task.Factory.StartNew(() => Log(new IntegrationLogModel() { PartitionKey = RequestDate.ToString("yyyyMMdd"), RowKey = string.Format("{0}_{1}_{2}", provider, RequestDate.ToString("HHmmss"), Guid.NewGuid().ToString("N")), URL = url, Message = message }));
            }
        }
        public void LogExceptionLog(string url, string exMessage, string innerMessage, string stackTrace, string source)
        {
            Task.Factory.StartNew(() => Log(new ExceptionLogModel() { PartitionKey = RequestDate.ToString("yyyyMMdd"), RowKey = string.Format("Exception_{0}_{1}", RequestDate.ToString("HHmmss"), Guid.NewGuid().ToString("N")), URL = url, Message = exMessage, InnerMessage = innerMessage, Source = source, StackTrace = stackTrace }));
        }
        public void Dispose()
        {
            ExecuteBatch();
            CloudTable = null;
            TableBatchOperation = null;
        }
        public void ExecuteBatch()
        {
            try
            {
                CloudTable.ExecuteBatchAsync(TableBatchOperation);
            }
            catch (Exception ex) { }
        }
    }
}
