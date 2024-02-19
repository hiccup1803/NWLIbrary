using Excel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NHibernate;
using NW.Core.Model;
using NW.Core.Services;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Service
{ 
    public class AzureStorageService : BaseService, IAzureStorageService
    {
        CloudTable CloudTable { get; set; }
        TableBatchOperation TableBatchOperation { get; set; }
        DateTime RequestDate { get; set; }
        public AzureStorageService(
            IUnitOfWork _unitOfWork,
            ISession _session)
            : base(_unitOfWork, _session)
        {
        }

        public void InsertEntity(ITableEntity tableEntity)
        {
            try
            {
                TableBatchOperation.Insert(tableEntity);
            }
            catch (Exception ex) {
                ;
            }
        }
        public void SaveExcel(string key, string fileName, byte[] fileContent)
        {
            Initialize("ExcelImport");


            IExcelDataReader excelReader = null;
            try
            {
                string partitionKey = RequestDate.ToString("yyyyMMdd");
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(new MemoryStream(fileContent));
                excelReader.IsFirstRowAsColumnNames = true;
                DataSet result = excelReader.AsDataSet();
                excelReader.Close();

                List<ExcelTableEntityModel> entityList = new List<ExcelTableEntityModel>();


                if (result.Tables.Count > 0)
                {
                    System.Data.DataTable sheet = result.Tables[0];
                    for (int j = 0; j < sheet.Rows.Count; j++)
                    {
                        string rowKey = string.Format("{0}_{1}_{2}", key, RequestDate.ToString("HHmmss"), Guid.NewGuid().ToString("N"));
                        ExcelTableEntityModel entity = new ExcelTableEntityModel(partitionKey, rowKey);
                        for (int i = 0; i < sheet.Columns.Count; i++)
                        {
                            string strCloName = sheet.Columns[i].ColumnName;
                            object value = sheet.Rows[j][i];
                            if (!(value is DBNull) && (value != null))
                            {
                                EntityProperty property = ExcelTableEntityModel.GetEntityProperty(strCloName, sheet.Rows[j][i]);
                                if (!entity.Properties.ContainsKey(strCloName))
                                {
                                    entity.Properties.Add(strCloName, property);
                                }
                                else
                                {
                                    entity.Properties[strCloName] = property;
                                }
                            }
                        }
                        //insert only rows with data
                        if (entity.Properties.Count > 0)
                        {
                            entityList.Add(entity);
                        }
                    }                                                    
                }

                foreach (ExcelTableEntityModel entity in entityList)
                {
                    Task.Factory.StartNew(() => InsertEntity(entity));
                }

            }
            catch (Exception ex)
            {
            }
        }
        public bool Initialize(string tableReference)
        {
            RequestDate = DateTime.UtcNow;
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable = tableClient.GetTableReference(tableReference);
                CloudTable.CreateIfNotExists();

                TableBatchOperation = new TableBatchOperation();

                return true;

            }
            catch (Exception ex) {
                return false;
            }
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
                CloudTable.ExecuteBatch(TableBatchOperation);
            }
            catch (Exception ex) {
                ;
            }
        }
    }
}
