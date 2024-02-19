using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public class BankTransferV2BankTransferAccount : Entity<int>
    {
        public virtual int StatusType { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string NameSurname { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string IBAN { get; set; }
        public virtual string BranchCode { get; set; }
        public virtual string ReferenceNote { get; set; }
        public virtual int? BankId { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual int ProviderId { get; set; }
        //public virtual IList<Level> Levels { get; set; }
        public virtual Company Company { get; set; }
        public virtual Provider Provider { get; set; }
        public virtual Bank Bank { get; set; }

    }
}
