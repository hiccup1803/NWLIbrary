using NHibernate;
using NW.Core.Entities;
using NW.Core.Repositories;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Repositories
{
    public class CompanyDomainRepository : Repository<CompanyDomain, int>, ICompanyDomainRepository
    {
        public CompanyDomainRepository(ISession _session) : base(_session) { }

        public int? CompanyId(string domain)
        {
            CompanyDomain companyDomain = GetAll().FirstOrDefault(cd => cd.Domain == domain);
            //return companyDomain != null ? companyDomain.CompanyId : new Nullable<int>();
            return companyDomain != null ? companyDomain.CompanyId : 1; // set default company 1 
        }

        public string GetLiveDomain(int companyId)
        {
            return GetAll().FirstOrDefault(cd => cd.CompanyId == companyId && cd.IsLive == true).Domain;
        }
    }
}
