using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface ICacheService : IDisposable
    {
        #region Keys
        string GameCountKey { get; }
        string GamesByCategoryIdKey { get; }
        string GameKey { get; }
        string ChildCategoriesKey { get; }
        string CategoryKey { get; }
        string TopCategoryTemplateKey { get; }
        string LocalizationKey { get; }
        #endregion
        T Get<T>(string key);
        void Set<T>(string key, T obj, TimeSpan? timeSpan = null);
        DataTable GetLocalizedStringByCulture(string culture);
        void DeleteLocalizedStringByCulture(string culture);
        void DeleteCacheRelatedWithGame();
        void DeleteCacheRelatedWithGame(string seoURL);
        void DeleteCacheRelatedWithGameCategory();
        void DeleteCacheRelatedWithCategory();
        void DeleteCacheRelatedWithCategory(int companyId, string alias);
        void DeleteAllCache();
        void DeleteCache(string key);
        int CacheKeyCount();
        string[] CacheKeys();
    }
}
