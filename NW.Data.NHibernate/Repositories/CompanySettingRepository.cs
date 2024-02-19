using NHibernate;
using NW.Core.Entities;
using NW.Core.Repositories;
using NW.Core.Services;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Repositories
{
    public class CompanySettingRepository : Repository<CompanySetting, int>, ICompanySettingRepository
    {
        public CompanySettingRepository(ISession _session) : base(_session) { }

        public string GetValue(int companyId, string key, bool isProduction)
        {
            IQueryable<CompanySetting> query = GetAll().Where(m => m.CompanyId == companyId && m.Name == key && m.Mode == isProduction);
            CompanySetting cs = query.FirstOrDefault();
            return cs != null ? cs.Value : string.Empty;
        }

        public void SetValue(int companyId, string key, string value, bool isProduction)
        {
            IQueryable<CompanySetting> query = GetAll().Where(m => m.CompanyId == companyId && m.Name == key && m.Mode == isProduction);
            CompanySetting cs = query.FirstOrDefault();
            if (cs != null)
            {
                cs.Value = value;
                Update(cs);
            }
            else
            {
                Insert(new CompanySetting()
                {
                    CompanyId = companyId,
                    Name = key,
                    Mode = isProduction,
                    Value = value,
                    KeyGroupId = 1,//TODO
                });
            }
        }
    }
}
