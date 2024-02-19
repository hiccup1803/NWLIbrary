using NW.Core.Contracts.Payment;
using NW.Core.Model.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services.Payment
{
    public interface IPaparaPaymentService
    {
        GenericPaymentResult GetPaymentModel(string domain, int memberId, bool isProduction, Int64 amount, bool withBonus, int? bonusId);
    }
}
