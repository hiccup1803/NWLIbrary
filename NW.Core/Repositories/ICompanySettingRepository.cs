using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW.Core.Entities;

namespace NW.Core.Repositories
{
    public interface ICompanySettingRepository: IRepository<CompanySetting, int>
    {
        string GetValue(int companyId, string key, bool isProduction);
        void SetValue(int companyId, string key, string value, bool isProduction);
    }
}
