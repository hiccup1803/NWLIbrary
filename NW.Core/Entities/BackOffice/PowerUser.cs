using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.BackOffice
{
    public class PowerUser : Entity<int>
    {
        public PowerUser()
        {
            Members = new List<Member>();
        }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual int Type { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int StatusType { get; set; }
        public virtual int BackOfficeRoleId { get; set; }
        public virtual string IPAddresses { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string LoginCode { get; set; }
        public virtual string SecretCode { get; set; }
        public virtual BackOfficeRole BackOfficeRole { get; set; }
        public virtual IList<Company> RestrictedCompanies { get; set; }
        public virtual IList<Provider> Providers { get; set; }

        public virtual IList<Member> Members { get; set; }
    }
}
