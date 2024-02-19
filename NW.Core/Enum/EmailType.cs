using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Enum
{
    public enum EmailType
    {
        Welcome = 1,
        DepositPending = 20,
        DepositSuccess = 21,
        DepositFail = 22,
        TwoWayAuth = 3,
        ForgotPassword = 4,
        LoginFailedMultipleTimes = 5,
        ConfirmEmail = 6,
        ConfirmEmailSuccess = 7,
        WithdrawRequested = 8,
        WithdrawRejected = 9,
        WithdrawSuccess = 10,
        GenericBOMail = 11,
        FreeSpins15 = 80,
        Davet1 = 81,
        Davet2 = 82,
        Davet3 = 83,
        Davet5 = 85,

        Freespin75 = 86,
        Freespin50 = 87,
        Freespin20 = 88,

        EcoPayz20 = 90,
        EcoPayz666 = 91,

        WebsiteError = 100,
        DomainBlocked = 110,
        VIPForm = 120
    }
}
