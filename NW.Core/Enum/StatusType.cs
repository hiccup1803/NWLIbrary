using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Enum
{
    public enum StatusType
    {
        UnWanted = -2,
        Deleted = -1,
        Passive = 0,
        Active = 1,

        KYC = 2,
        BonusAbuser = 3,

        
        RegistrationStep1 = 101,
        RegistrationStep2 = 102,
    }
}
