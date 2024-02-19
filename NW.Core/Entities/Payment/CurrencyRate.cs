using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    /// <summary>
    /// </summary>
    public class CurrencyRate : Entity<int>
    {
        public virtual int UpdateId { get; set; }
        public virtual Currency FromCurrency { get; set; }
        public virtual Currency ToCurrency { get; set; }

        public virtual double Rate { get; set; }
        public virtual double SellingRate { get; set; }

        public virtual DateTime CreateDate { get; set; }
    }
}