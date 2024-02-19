using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class DittoCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ReadConnectionString { get; set; }
        public string WriteConnectionString { get; set; }
        public string CacheConnectionString { get; set; }
        public string StrongKey { get; set; }
        public string ExternalServiceMeshDomain { get; set; }
    }
}
