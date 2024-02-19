using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Payment
{
    public enum PaymentStatusType
    {
        Approved = 1,
        Pending = 0,
        Rejected = -1,
        Transferred=2,
        Cancelled = -2,

        BTFormFilled = 201,
        BTProviderInfoSet = 202,
        BTProviderInfoFilled = 203,
        BTProviderInfoRejected = 204,

    }
}
