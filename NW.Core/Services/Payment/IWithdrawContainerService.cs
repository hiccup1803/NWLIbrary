using NW.Core.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services.Payment
{
    public interface IWithdrawContainerService
    {
        string GetWithdrawStatusByVoltronTransactionId(long voltronTransactionId, int providerId);
        IList<WithdrawRequestBankTransfer> PendingWithdrawRequestBankTransferList();
        IList<EcoPayzRequest> PendingEcoPayzRequestList();
    }
}
