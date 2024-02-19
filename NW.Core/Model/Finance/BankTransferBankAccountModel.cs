using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Model.Finance
{
    public class BankTransferBankAccountModel
    {
        public int Id { get; set; }
        public string NameSurname { get; set; }
        public string NameSurnameMasked { get; set; }
        public string Branch { get; set; }
        public string BranchCode { get; set; }
        public string AccountNumber { get; set; }
        public string IBAN { get; set; }
    }
}
