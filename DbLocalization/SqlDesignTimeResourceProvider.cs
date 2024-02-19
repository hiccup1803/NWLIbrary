using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Threading;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.UI.Design;

namespace DbLocalization
{
    /// <remarks>
    /// Design-time resource provider using a Microsoft Sql Database.
    /// </remarks>
    public sealed class SqlDesignTimeResourceProviderFactory : DesignTimeResourceProviderFactory
    {
        private IResourceProvider _localResourceProvider;

        public SqlDesignTimeResourceProviderFactory()
        {
        }

        public override IResourceProvider CreateDesignTimeGlobalResourceProvider(IServiceProvider serviceProvider, string applicationKey)
        {
            return new DesignTimeGlobalResourceProvider(applicationKey);
        }

        public override IResourceProvider CreateDesignTimeLocalResourceProvider(IServiceProvider serviceProvider)
        {
            // Resource reader is cached for performance reasons, otherwise a new one
            // would be created for every property of every control that was localized.
            if (_localResourceProvider == null)
            {
                _localResourceProvider = new DesignTimeLocalResourceProvider(serviceProvider);
            }
            return _localResourceProvider;
        }

        public override IDesignTimeResourceWriter CreateDesignTimeLocalResourceWriter(IServiceProvider serviceProvider)
        {
            // Resource writer is never cached because it is generally called
            // only once, and should always use fresh resources anyway.
            return new DesignTimeLocalResourceProvider(serviceProvider);
        }


        /// <remarks>
        /// Design-time resource provider for global resources.
        /// </remarks>
        private sealed class DesignTimeGlobalResourceProvider : IResourceProvider
        {
            public DesignTimeGlobalResourceProvider(string applicationKey)
            {
            }

            object IResourceProvider.GetObject(string ResourceName, CultureInfo culture)
            {
                return null;
            }

            IResourceReader IResourceProvider.ResourceReader
            {
                get
                {
                    return null;
                }
            }
        }

        /// <remarks>
        /// Design-time resource provider and writer for local resources.
        /// </remarks>
        private sealed class DesignTimeLocalResourceProvider : IResourceProvider, IDesignTimeResourceWriter
        {
            private IServiceProvider _serviceProvider;
            private IResourceDictionary _localResources;
            private IDictionary _reader = null;


            public DesignTimeLocalResourceProvider(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            private string GetVirtualPath()
            {
                IDesignerHost host = (IDesignerHost)_serviceProvider.GetService(typeof(IDesignerHost));
                WebFormsRootDesigner rootDesigner = host.GetDesigner(host.RootComponent) as WebFormsRootDesigner;
                
                return rootDesigner.DocumentUrl.TrimStart('~', '/');
                //return System.IO.Path.GetFileName(rootDesigner.DocumentUrl);
            }

            private void Load()
            {
                string virtualPath = GetVirtualPath();

                IDictionary data = SqlResourceDataAccess.GetResources(virtualPath, null, null, true, _serviceProvider);

                if (_reader == null)
                {
                    _reader = new HybridDictionary();
                }
                else
                {
                    //reload
                    _reader.Clear();
                    if (_localResources != null)
                    {
                        _localResources.Clear();
                        _localResources = null;
                    }
                }

                if (data != null)
                {
                    foreach (DictionaryEntry de in data)
                    {
                        _reader.Add(de.Key, de.Value);
                    }
                }
            }

            private void Flush()
            {
                if (LocalResources != null && LocalResources.Persistable)
                {
                    string virtualPath = GetVirtualPath();

                    bool refreshLocalResource = false;

                    foreach (DictionaryEntry de in LocalResources)
                    {
                        SqlResourceDataAccess.AddResource((string)de.Key, de.Value, virtualPath, _serviceProvider);
                        if (!_reader.Contains(de.Key))
                        {
                            _reader.Add(de.Key, de.Value);
                            refreshLocalResource = true;
                        }
                    }
                    if (refreshLocalResource && _localResources != null)
                    {
                        _localResources.Clear();
                        _localResources = null;
                    }
                }
            }

            private IResourceDictionary LocalResources
            {
                get
                {
                    if (_localResources == null)
                    {
                        string resourceName = null; // application key
                        if (resourceName == null || resourceName.Length == 0)
                        {
                            Load();
                            _localResources = new ResourceDictionary(_reader);
                        }
                    }
                    return _localResources;
                }
            }

            private string CreateResourceName(string resourceName, object obj)
            {
                if (resourceName == null || resourceName.Length == 0)
                {
                    return LocalResources.CreateResourceName(obj);
                }
                return null;
            }

            object IResourceProvider.GetObject(string ResourceName, CultureInfo culture)
            {               
                if (culture != CultureInfo.InvariantCulture)
                {
                    // TODO: String resource
                    throw new ArgumentException("Only the InvariantCulture is supported.", "culture");
                }
                
                if (LocalResources == null)
                {
                    return null;
                }
                return LocalResources[ResourceName];
            }

            IResourceReader IResourceProvider.ResourceReader
            {
                get
                {
                    if (LocalResources == null)
                    {
                        return null;
                    }
                    return new DictionaryResourceReader(LocalResources);
                }
            }

            string IDesignTimeResourceWriter.CreateResourceKey(string resourceName, object obj)
            {
                return CreateResourceName(resourceName, obj);
            }

            void IResourceWriter.AddResource(string name, byte[] value)
            {
                if (LocalResources == null)
                {
                    return;
                }
                LocalResources[name] = value;
            }

            void IResourceWriter.AddResource(string name, object value)
            {
                if (LocalResources == null)
                {
                    return;
                }
                LocalResources[name] = value;
            }

            void IResourceWriter.AddResource(string name, string value)
            {
                if (LocalResources == null)
                {
                    return;
                }
                LocalResources[name] = value;
            }

            void IResourceWriter.Generate()
            {
                Flush();
            }

            void IResourceWriter.Close()
            {
            }

            void IDisposable.Dispose()
            {
            }
        }

        private sealed class DictionaryResourceReader : IResourceReader
        {
            private IDictionary _resources;

            public DictionaryResourceReader(IDictionary resources)
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

        private interface IResourceDictionary : IDictionary
        {
            string CreateResourceName(object obj);
            string CreateResourceName(string key);
            bool Persistable { get; }
        }

        private sealed class ResourceDictionary : IResourceDictionary
        {
            private IDictionary _resources;
            private IDictionary _ResourceNames;
            private bool _persistable;


            public ResourceDictionary(IDictionary resources)
            {
                _resources = new Hashtable(resources);
                _ResourceNames = new HybridDictionary();
            }


            string IResourceDictionary.CreateResourceName(object obj)
            {
                /*
                string ResourceKey = null;
                
                Control Ctl = obj as Control;
                if (Ctl != null)
                    ResourceKey = Ctl.ID;

                if (string.IsNullOrEmpty(ResourceKey))
                    ResourceKey = obj.GetType().Name;
                */
                string ResourceKey = obj.GetType().Name + "Resource";
                return CreateKey(ResourceKey);
            }

            string IResourceDictionary.CreateResourceName(string key)
            {
                return CreateKey(key);
            }

            bool IResourceDictionary.Persistable
            {
                get
                {
                    return _persistable;
                }
            }


            object IDictionary.this[object key]
            {
                get
                {
                    object val = null;
                    if (_resources != null && key != null)
                    {
                        val = _resources[key];
                    }
                    return val;
                }
                set
                {
                    if (_resources != null && key != null)
                    {
                        _persistable = true;
                        _resources[key] = value;
                    }
                }
            }

            ICollection IDictionary.Keys
            {
                get
                {
                    return _resources.Keys;
                }
            }

            ICollection IDictionary.Values
            {
                get
                {
                    return _resources.Values;
                }
            }

            bool IDictionary.IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            bool IDictionary.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            void IDictionary.Add(object key, object value)
            {
                ((IDictionary)this)[key] = value;
            }

            void IDictionary.Clear()
            {
                if (_resources != null)
                {
                    _resources.Clear();
                    _resources = null;
                }
            }

            bool IDictionary.Contains(object key)
            {
                return _resources.Contains(key);
            }

            void IDictionary.Remove(object key)
            {
                _resources.Remove(key);
            }

            IDictionaryEnumerator IDictionary.GetEnumerator()
            {
                return _resources.GetEnumerator();
            }


            void ICollection.CopyTo(Array array, int index)
            {
                _resources.CopyTo(array, index);
            }

            int ICollection.Count
            {
                get
                {
                    return _resources.Count;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }


            IEnumerator IEnumerable.GetEnumerator()
            {
                return _resources.GetEnumerator();
            }


            private bool IsUsedKey(string key)
            {
                bool used = false;
                used = _ResourceNames.Contains(key);
                if (!used)
                {
                    IEnumerator enumKeys = _resources.Keys.GetEnumerator();
                    while (enumKeys.MoveNext())
                    {
                        string reskey = (string)enumKeys.Current;
                        if (reskey != null)
                        {
                            int index = reskey.IndexOf(key);
                            if (index >= 0)
                            {
                                used = true;
                                break;
                            }
                        }
                    }
                }
                return used;
            }

            private string CreateKey(string key)
            {
                int count = 1;
                string ResourceName = key + count;

                while (IsUsedKey(ResourceName))
                {
                    ResourceName = key + count;
                    count++;
                }

                _ResourceNames[ResourceName] = String.Empty;

                return ResourceName;
            }
        }
    }
}
