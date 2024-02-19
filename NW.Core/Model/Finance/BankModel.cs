using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Model.Finance
{
    public class BankModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public IList<BankTransferBankAccountModel> BankTransferBankAccountList { get; set; }
    }
}
