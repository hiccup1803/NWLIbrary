using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Enum
{
    public enum WithdrawStatusType
    {
        WaitingVoltron = -99,
        Processing = 0,
        Rejected = -1,
        Approved = 1,
        Cancelled = 2,
        Sent = 5
    }
}
