using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Enum
{

    public enum KycStatusType
    {
        Processing = -2,
        Failed = -1,
        Neutral = 0,
        Verified = 1
    }
}
