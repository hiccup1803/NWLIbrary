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
    public class ProviderSettingRepository : Repository<ProviderSetting, int>, IProviderSettingRepository
    {
        public ProviderSettingRepository(ISession _session) : base(_session) { }

        public string GetValue(int providerId, int companyId, string key, bool isProduction)
        {
            IQueryable<ProviderSetting> query = GetAll().Where(m => m.ProviderId == providerId && m.CompanyId == companyId && m.Name == key && m.Mode == isProduction);
            ProviderSetting ps = query.FirstOrDefault();
            return ps != null ? ps.Value : string.Empty;
        }

        public void SetValue(int providerId, int companyId, string key, string value, bool isProduction)
        {
            IQueryable<ProviderSetting> query = GetAll().Where(m => m.ProviderId == providerId && m.CompanyId == companyId && m.Name == key && m.Mode == isProduction);
            ProviderSetting ps = query.FirstOrDefault();
            if (ps != null)
            {
                ps.Value = value;
                Update(ps);
            }
            else
            {
                Insert(new ProviderSetting()
                {
                    ProviderId = providerId,
                    CompanyId = companyId,
                    Name = key,
                    Mode = isProduction,
                    Value = value,
                });
            }
        }
    }
}
