using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class Bank : Entity<int>
    {
        public virtual int StatusType { get; set; }
        public virtual string Name { get; set; }
        public virtual string Logo { get; set; }
        public virtual DateTime CreateDate { get; set; }


        public virtual IEnumerable<BankTransferBankAccount> BankTransferBankAccounts { get; set; }
    }
}
