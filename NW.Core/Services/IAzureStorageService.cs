using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface IAzureStorageService : IDisposable
    {
       
        void InsertEntity(ITableEntity tableEntity);
        void SaveExcel(string key, string fileName, byte[] fileContent);
        bool Initialize(string tableReference);
        void Dispose();
        void ExecuteBatch();

    }
}
