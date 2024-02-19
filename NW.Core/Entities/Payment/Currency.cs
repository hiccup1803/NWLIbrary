using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    /// <summary>
    /// 
    /// </summary>
    public class Currency : Entity<int>
    {

        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsBaseCurrency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime CreateDate { get; set; }
    }
}