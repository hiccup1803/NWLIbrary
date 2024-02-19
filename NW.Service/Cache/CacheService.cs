using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHibernate;
using NW.Core.Services;
using NW.Core.Work;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NW.Service.Cache
{
    public class CacheService : BaseService, ICacheService, IDisposable
    {
        #region Keys


        public string GameCountKey
        {
            get { return "Cache.GameCount"; }
        }

        public string GamesByCategoryIdKey
        {
            get { return "Cache.GamesByCategoryId"; }
        }

        public string GameKey
        {
            get { return "Cache.Game"; }
        }

        public string ChildCategoriesKey
        {
            get { return "Cache.ChildCategories"; }
        }

        public string CategoryKey
        {
            get { return "Cache.Category"; }
        }

        public string TopCategoryTemplateKey
        {
            get { return "Cache.TopCategoryTemplate"; }
        }

        public string LocalizationKey
        {
            get { return "Cache.Localization"; }
        }
        #endregion

        private static readonly Object _multiplexerLock = new Object();


        //Lazy<ConnectionMultiplexer> LazyConnection { get; set; }
        ConnectionMultiplexer Connection { get; set; }
        IDatabase CacheDatabase { get; set; }
        public CacheService(IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            if (!(Connection != null && Connection.IsConnected))
            {
                lock (_multiplexerLock)
                {
                    Connection = ConnectionMultiplexer.Connect(ConfigurationManager.ConnectionStrings["CacheServer"].ConnectionString);
                }
            }
        }
        public T Get<T>(string key)
        {
            string value = null;
            try
            {
                CacheDatabase = Connection.GetDatabase();
                value = CacheDatabase.StringGet(key);
            }
            catch (Exception ex) { }

            if (value == null)
                return default(T);
            else
                return JsonConvert.DeserializeObject<T>(value);
        }

        public void Set<T>(string key, T obj, TimeSpan? timeSpan = null)
        {
            try
            {
                if (obj != null)
                {
                    CacheDatabase = Connection.GetDatabase();

                    string serializedObject = JsonConvert.SerializeObject(obj);
                    CacheDatabase.StringSet(key, serializedObject, timeSpan);
                }
            }
            catch (Exception ex) { }
        }


        public void DeleteCacheRelatedWithGame()
        {
            CacheDatabase = Connection.GetDatabase();
            IServer server = Connection.GetServer(Connection.GetEndPoints().First());

            CacheDatabase.KeyDelete(server.Keys(pattern: GameKey + ("*")).ToArray());
        }

        public void DeleteCacheRelatedWithGame(string seoURL)
        {
            CacheDatabase = Connection.GetDatabase();
            IServer server = Connection.GetServer(Connection.GetEndPoints().First());

            CacheDatabase.KeyDelete(server.Keys(pattern: GameKey + ("-seoURL:" + seoURL)).ToArray());
        }

        public void DeleteCacheRelatedWithGameCategory()
        {
            CacheDatabase = Connection.GetDatabase();
            IServer server = Connection.GetServer(Connection.GetEndPoints().First());

            List<RedisKey> keys = new List<RedisKey>();
            keys.AddRange(server.Keys(pattern: GamesByCategoryIdKey + "*"));
            keys.AddRange(server.Keys(pattern: GameCountKey + "*"));

            CacheDatabase.KeyDelete(keys.ToArray());
        }

        public void DeleteCacheRelatedWithCategory()
        {
            CacheDatabase = Connection.GetDatabase();
            IServer server = Connection.GetServer(Connection.GetEndPoints().First());

            List<RedisKey> keys = new List<RedisKey>();
            keys.AddRange(server.Keys(pattern: ChildCategoriesKey + "*"));
            keys.AddRange(server.Keys(pattern: CategoryKey + "*"));
            keys.AddRange(server.Keys(pattern: TopCategoryTemplateKey + "*"));

            CacheDatabase.KeyDelete(keys.ToArray());
        }

        public void DeleteCacheRelatedWithCategory(int companyId, string alias)
        {
            try
            {
                CacheDatabase = Connection.GetDatabase();
                IServer server = Connection.GetServer(Connection.GetEndPoints().First());

                CacheDatabase.KeyDelete(server.Keys(pattern: CategoryKey + ("-companyId:" + companyId + "-alias:" + alias)).ToArray());

            }
            catch (Exception ex)
            {

            }
        }

        public void DeleteAllCache()
        {
            IServer server = Connection.GetServer(Connection.GetEndPoints().First());
            server.FlushAllDatabases();
        }
        public void DeleteLocalizedStringByCulture(string culture)
        {
            try
            {
                CacheDatabase = Connection.GetDatabase();
                IServer server = Connection.GetServer(Connection.GetEndPoints().First());

                CacheDatabase.KeyDelete(server.Keys(pattern: LocalizationKey + "-culture:" + culture).ToArray());

            }
            catch (Exception ex)
            {
            }
        }


        public int CacheKeyCount()
        {
            IServer server = Connection.GetServer(Connection.GetEndPoints().First());
            return server.Keys().Count();
        }



        public string[] CacheKeys()
        {
            IServer server = Connection.GetServer(Connection.GetEndPoints().First());
            return server.Keys().Select(rk => rk.ToString()).ToArray();
        }


        public DataTable GetLocalizedStringByCulture(string culture)
        {
            string value = null;
            try
            {
                CacheDatabase = Connection.GetDatabase();
                value = CacheDatabase.StringGet(LocalizationKey + "-culture:" + culture);
            }
            catch (Exception ex) { }

            DataTable dt = null;
            if (value != null)
                dt = JsonConvert.DeserializeObject<DataTable>(value);
            else
            {
                dt = DbLocalization.SqlResourceDataAccess.GetResourcesByCulture(culture);


                try
                {
                    CacheDatabase.StringSet(LocalizationKey + "-culture:" + culture, JsonConvert.SerializeObject(dt));
                }
                catch (Exception ex) { }
            }

            return dt;
        }
        public void DeleteCache(string key)
        {
            CacheDatabase = Connection.GetDatabase();
            IServer server = Connection.GetServer(Connection.GetEndPoints().First());

            CacheDatabase.KeyDelete(server.Keys(pattern: key).ToArray());
        }




        public void Dispose()
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}
