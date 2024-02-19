using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.UI.Design;
using System.Linq;

namespace DbLocalization
{
    public class SqlResourceSearchReturnResult
    {
        public IList<string> Items { get; set; }
        public int Count { get; set; }
    }
    /// <remarks>
    /// Runtime resource provider using a Microsoft Sql Database.
    /// </remarks>
    [DesignTimeResourceProviderFactoryAttribute(typeof(SqlDesignTimeResourceProviderFactory))]
    public sealed class SqlResourceProviderFactory : ResourceProviderFactory
    {

        public string StripVirtualPath(string FullVirtualPath)
        {
            string StripVirtual = string.Empty;

            if (HttpContext.Current != null)
                StripVirtual = HttpContext.Current.Request.ApplicationPath;

            if (!StripVirtual.EndsWith("/"))
                StripVirtual = StripVirtual + "/";

            if (StripVirtual == "/" && FullVirtualPath.StartsWith("/"))
                return FullVirtualPath.TrimStart('/');

            return FullVirtualPath.Replace(StripVirtual, "");
        }

        public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
        {
            virtualPath = StripVirtualPath(virtualPath);
            return new SqlResourceProvider(virtualPath, null);
        }

        public override IResourceProvider CreateGlobalResourceProvider(string className)
        {
            return new SqlResourceProvider(null, className);
        }
        public static SqlResourceSearchReturnResult Search(string className, string culture, string searchText)
        {
            return new SqlResourceProvider(null, className).Search(culture, searchText);
        }

        private sealed class SqlResourceProvider : IResourceProvider
        {
            private string _virtualPath;
            private string _className;
            private IDictionary _resourceCache;

            private static object CultureNeutralKey = new object();

            public SqlResourceProvider(string virtualPath, string className)
            {
                _virtualPath = virtualPath;
                _className = className;
            }

            private IDictionary GetResourceCache(string cultureName)
            {
                if (_resourceCache == null)
                {
                    _resourceCache = new ListDictionary();
                } 

                object cultureKey = (cultureName != null ? cultureName : CultureNeutralKey);
                SqlResourceListDictionary resourceDict = new SqlResourceListDictionary();
                if (HttpContext.Current != null && HttpContext.Current.Application["LocalizedStringTable" + cultureName] != null)
                {
                    DataTable dt = (DataTable)HttpContext.Current.Application["LocalizedStringTable" + cultureName];                    

                    foreach(DataRow dr in dt.Rows.OfType<DataRow>().Where(dr => dr["ClassName"].ToString() == _className))
                    {
                        if(!resourceDict.Contains(dr["ResourceName"].ToString()))
                            resourceDict.Add(dr["ResourceName"].ToString(), dr["ResourceValue"].ToString());
                    }
                }
                else
                {
                    resourceDict = _resourceCache[cultureKey] as SqlResourceListDictionary;
                    if ((resourceDict == null) ||
                        ((resourceDict != null) && ((((TimeSpan)DateTime.Now.Subtract(resourceDict.DateCreated)).Minutes >= SqlResourceHelper.ResourceTimeOutAsMinute))))
                    {
                        resourceDict = (SqlResourceListDictionary)SqlResourceDataAccess.GetResources(_virtualPath, _className, cultureName, false, null);                        
                    }

                }

                _resourceCache[cultureKey] = resourceDict;
                return resourceDict;
            }

            object IResourceProvider.GetObject(string resourceName, CultureInfo culture)
            {
                if (HttpContext.Current != null)
                {
                    SqlResourceUpdate sqlResourceUpdate = new SqlResourceUpdate();
                    foreach (ResourceUpdate resourceUpdate in sqlResourceUpdate)
                    {
                        if (((resourceUpdate.VirtualPath != null) && (_virtualPath != null) &&
                            (resourceUpdate.VirtualPath.ToLower() == _virtualPath.ToLower())) ||
                            ((resourceUpdate.ClassName != null) && (_className != null) &&
                            (resourceUpdate.ClassName.ToLower() == _className.ToLower())))
                        {
                            if (resourceUpdate.ResourceKey != null)
                            {
                                object _value = GetResourceCache(resourceUpdate.CultureCode)[resourceUpdate.ResourceKey];
                                if (_value != null)
                                {
                                    string _resourceValue = SqlResourceDataAccess.GetSingleResource(_virtualPath, _className, resourceUpdate.CultureCode, resourceUpdate.ResourceKey);
                                    if (_resourceValue != null)
                                        ((SqlResourceListDictionary)_resourceCache[resourceUpdate.CultureCode])[resourceUpdate.ResourceKey] = _resourceValue;
                                    else
                                        ((SqlResourceListDictionary)_resourceCache[resourceUpdate.CultureCode])[resourceUpdate.ResourceKey] = string.Empty;
                                }
                            }
                            else
                            {
                                _resourceCache[resourceUpdate.CultureCode] = null;
                            }

                            sqlResourceUpdate.Remove(resourceUpdate);
                        }
                    }
                }

                string cultureName = null;
                if ((culture != null) && (!string.IsNullOrEmpty(culture.Name)))
                {
                    cultureName = culture.Name;
                }
                else
                {
                    cultureName = Thread.CurrentThread.CurrentCulture.Name;
                }
                object value = GetResourceCache(cultureName)[resourceName];
                if (value == null)
                {
                    value = GetResourceCache(null)[resourceName];
                }
                return value;
            }
            public SqlResourceSearchReturnResult Search(string culture, string searchText)
            {
                string cultureName;
                if (!string.IsNullOrEmpty(culture))
                {
                    cultureName = culture;
                }
                else
                {
                    cultureName = Thread.CurrentThread.CurrentCulture.Name;
                }
                SqlResourceSearchReturnResult result = new SqlResourceSearchReturnResult();

                IEnumerable<DictionaryEntry> items = GetResourceCache(cultureName).OfType<DictionaryEntry>().Where(kvp => kvp.Value.ToString().ToLower(CultureInfo.GetCultureInfo("en-GB")).Contains(searchText.ToLower()));

                result.Items = items.Select(kvp => kvp.Key.ToString()).ToList();
                result.Count = items.Count();
                return result;
            }

            IResourceReader IResourceProvider.ResourceReader
            {
                get
                {
                    return new SqlResourceReader(GetResourceCache(null));
                }
            }

            private sealed class SqlResourceReader : IResourceReader
            {
                private IDictionary _resources;

                public SqlResourceReader(IDictionary resources)
                {
                    _resources = resources;
                }

                IDictionaryEnumerator IResourceReader.GetEnumerator()
                {
                    return _resources.GetEnumerator();
                }

                void IResourceReader.Close()
                {
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return _resources.GetEnumerator();
                }

                void IDisposable.Dispose()
                {
                }
            }
        }
    }
}
