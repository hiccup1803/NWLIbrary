using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Enum
{
    public enum MemberDeviceFingerPrintStatusType
    {
        Force2FA = -2,
        Blocked = -1,
        Neutral = 0,
        Whitelisted = 1
    }
}
