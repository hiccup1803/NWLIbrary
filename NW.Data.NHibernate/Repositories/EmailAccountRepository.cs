using NHibernate;
using NW.Core.Entities.Marketing;
using NW.Core.Enum;
using NW.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Repositories
{
    public class EmailAccountRepository : Repository<EmailAccount, int>, IEmailAccountRepository
    {
        public EmailAccountRepository(ISession _session) : base(_session) { }
        public EmailAccount EmailAccountByEmailType(int companyId, EmailType emailType)
        {
            EmailAccount emailAccount = GetAll().FirstOrDefault(ea => ea.CompanyId == companyId && ea.EmailType == (int)emailType);
            return emailAccount != null ? emailAccount : GetAll().FirstOrDefault(ea => ea.CompanyId == companyId && ea.IsDefault);
        }
    }
}
