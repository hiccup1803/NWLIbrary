using NW.Core.Entities.Marketing;
using NW.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Repositories
{
    public interface IEmailAccountRepository
    {
        EmailAccount EmailAccountByEmailType(int companyId, EmailType emailType);
    }
}
