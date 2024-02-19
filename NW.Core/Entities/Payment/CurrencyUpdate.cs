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
    public class CurrencyUpdate : Entity<int>
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsActive { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime CreateDate { get; set; }
    }
}