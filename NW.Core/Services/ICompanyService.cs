using NW.Core.Entities;
using NW.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface ICompanyService
    {
        string GetValue(int companyId, string key, bool isProduction, bool useCache = true);
        void SetValue(int companyId, string key, string value, bool isProduction);
        int CompanyId(string domain);
        int BackOfficeCompanyId(string domain);
        int BackOfficeVoltronCompanyId(string domain);
        Company Company(int id);
        IList<Company> GetAllCompanies();
        void InsertCompany(Company company);
        void UpdateCompany(Company company);
        string GetCurrentDomain(int companyId);
    }
}
