using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface IProviderService
    {
        string GetValue(int providerId, int companyId, string key, bool isProduction);
        void SetValue(int providerId, int companyId, string key, string value, bool isProduction);
        Provider Provider(int id);
        Provider GetProviderByVoltronProviderId(int voltronProviderId);
        IList<Provider> GetAllProviders();
        IList<Provider> GetAllProviders(int providerTypeId);
        void InsertProvider(Provider provider);
        void UpdateProvider(Provider provider);
        int EnableProviderForLevel(int providerId, int levelId);
        int DisableProviderForLevel(int providerId, int levelId);
    }
}
