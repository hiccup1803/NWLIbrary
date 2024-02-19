using NW.Core.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Model.Finance
{
    public class CepbankPaymentGenericViewModel
    {
        public IList<CepBank> CepBankList { get; set; }
        public long Amount { get; set; }
        public int? BonusId { get; set; }
        public int? SelectedBonusId { get; set; }
        public string Provider { get; set; }
    }
}
