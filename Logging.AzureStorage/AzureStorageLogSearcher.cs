using Logging.AzureStorage.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace Logging.AzureStorage
{
    public class AzureStorageLogSearcher
    {

        CloudTable CloudTable { get; set; }
        TableBatchOperation TableBatchOperation { get; set; }
        LogType LogType { get; set; }
        DateTime RequestDate { get; set; }
        public AzureStorageLogSearcher(LogType logType)
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
            }
            catch (Exception ex) { }
        }


        public DataTable Search(DateTime startDate, DateTime endDate, string provider, string message)
        {
            string filterString = string.Empty;
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                string searchStr = string.Format("{0}_", provider);

                char lastChar = searchStr[searchStr.Length - 1];
                char nextLastChar = (char)((int)lastChar + 1);
                string nextSearchStr = searchStr.Substring(0, searchStr.Length - 1) + nextLastChar;
                string prefixCondition = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, searchStr),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, nextSearchStr)
                    );

                string partitionFilter = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, date.ToString("yyyyMMdd")),
                    TableOperators.And,
                    prefixCondition
                    );


                if (string.IsNullOrEmpty(filterString))
                {
                    filterString = partitionFilter;
                }
                else
                {
                    filterString = TableQuery.CombineFilters(filterString, TableOperators.Or, partitionFilter);
                }
            }

            DataTable dt = new DataTable();
            switch (LogType)
            {
                case LogType.IntegrationLog:
                    {
                        var query = new TableQuery<IntegrationLogModel>().Where(filterString);


                        dt.Columns.Add("URL");
                        dt.Columns.Add("Message");
                        dt.Columns.Add("CreateDate");


                        foreach (var row in CloudTable.ExecuteQuery<IntegrationLogModel>(query))
                        {
                            if (string.IsNullOrEmpty(message) || row.Message.IndexOf(message) > 0)
                            {
                                DataRow dr = dt.NewRow();
                                dr["URL"] = row.URL;
                                dr["Message"] = row.Message;
                                dr["CreateDate"] = row.Timestamp.ToUniversalTime().ToString("dd.MM.yyyy HH:mm:ss.fff");

                                dt.Rows.Add(dr);

                            }
                        }

                        break;
                    }
                case LogType.Track:
                case LogType.PartnerLog:
                case LogType.Exception:
                    break;
                default:
                    break;
            }

            return dt;
        }
    }
}
