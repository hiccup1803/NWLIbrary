using NW.Core.Entities;
using NW.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Repositories
{
    public interface ICompanyDomainRepository : IRepository<CompanyDomain, int>
    {
        int? CompanyId(string domain);
        string GetLiveDomain(int companyId);
    }
}
