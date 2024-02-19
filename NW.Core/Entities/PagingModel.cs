using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class PagingModel<T>
    {
        public IList<T> ItemList { get; set; }
        public long TotalCount { get; set; }
    }
}
