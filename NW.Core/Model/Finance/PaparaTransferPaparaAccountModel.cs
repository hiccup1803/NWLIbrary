using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Model.Finance
{
    public class PaparaTransferPaparaAccountModel
    {
        public int Id { get; set; }
        public int StatusType { get; set; }
        public DateTime CreateDate { get; set; }
        public string NameSurname { get; set; }
        public string NameSurnameMasked { get; set; }
        public string AccountNumber { get; set; }
        public string BlaclistedUsernameList { get; set; }
        public int CompanyId { get; set; }
    }
}
