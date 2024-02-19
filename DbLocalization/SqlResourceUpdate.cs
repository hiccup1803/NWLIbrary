using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Web;

namespace DbLocalization
{
    public class ResourceUpdate
    {
        public string VirtualPath;
        public string ClassName;
        public string CultureCode;
        public string ResourceKey;
    }

    public class SqlResourceUpdate : IEnumerable
    {
        private const string AppResourceUpdates = "ResourceUpdates";
        private List<ResourceUpdate> _updates;

        public SqlResourceUpdate()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Application[AppResourceUpdates] == null)
                    HttpContext.Current.Application[AppResourceUpdates] = new List<ResourceUpdate>();

                _updates = (List<ResourceUpdate>)HttpContext.Current.Application[AppResourceUpdates];
            }
            else
                throw new Exception("HttpContext not found!");
        }

        public void Add(ResourceUpdate ResourceUpdate)
        {
            _updates.Add(ResourceUpdate);
        }

        public void Remove(ResourceUpdate ResourceUpdate)
        {
            _updates.Remove(ResourceUpdate);
        }

        public IEnumerator GetEnumerator()
        {
            return new ResourceUpdateEnum(_updates);
        }
    }

    public class ResourceUpdateEnum : IEnumerator
    {
        private List<ResourceUpdate> _updates;

        int position = -1;

        public ResourceUpdateEnum(List<ResourceUpdate> List)
        {
            _updates = List;
            position = _updates.Count;
        }

        public bool MoveNext()
        {
            position--;
            return (position >= 0);
        }

        public void Reset()
        {
            position = _updates.Count;
        }

        public object Current
        {
            get
            {
                try
                {
                    return _updates[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
