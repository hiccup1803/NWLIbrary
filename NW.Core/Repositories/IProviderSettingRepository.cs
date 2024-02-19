using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW.Core.Entities;

namespace NW.Core.Repositories
{
    public interface IProviderSettingRepository: IRepository<ProviderSetting, int>
    {
        string GetValue(int providerId, int companyId, string key, bool isProduction);
        void SetValue(int providerId, int companyId, string key, string value, bool isProduction);
    }
}
