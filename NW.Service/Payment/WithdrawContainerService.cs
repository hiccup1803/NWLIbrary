using NHibernate;
using NW.Core.Entities.Payment;
using NW.Core.Enum;
using NW.Core.Repositories;
using NW.Core.Services.Payment;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Service.Payment
{
    public class WithdrawContainerService : BaseService, IWithdrawContainerService
    {
        IRepository<EcoPayzRequest, int> EcoPayzRewpository { get; set; }
        IRepository<WithdrawRequestBankTransfer, int> WithdrawRequestBankTransferRepository { get; set; }
        public WithdrawContainerService(IRepository<EcoPayzRequest, int> _ecoPayzRewpository, IRepository<WithdrawRequestBankTransfer, int> _withdrawRequestBankTransferRepository, IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            EcoPayzRewpository = _ecoPayzRewpository;
            WithdrawRequestBankTransferRepository = _withdrawRequestBankTransferRepository;
        }



        public string GetWithdrawStatusByVoltronTransactionId(long voltronTransactionId, int providerId)
        {
            switch (providerId)
            {
                case 45: // ecopayz

                    EcoPayzRequest eco = EcoPayzRewpository.GetAll()
                        .FirstOrDefault(w => w.PaymentTransactionId == voltronTransactionId);

                    return eco != null ? ((WithdrawStatusType)eco.WithdrawStatusType).ToString() : string.Empty;

                case 33: // bank transfer

                    WithdrawRequestBankTransfer withdrawRequestBankTransfer =
                        WithdrawRequestBankTransferRepository.GetAll()
                        .FirstOrDefault(w => w.PaymentTransactionId == voltronTransactionId);

                    return withdrawRequestBankTransfer != null ? ((WithdrawStatusType)withdrawRequestBankTransfer.WithdrawStatusType).ToString() : string.Empty;
                default:
                    return null;
            }
        }


        public IList<WithdrawRequestBankTransfer> PendingWithdrawRequestBankTransferList()
        {
            return WithdrawRequestBankTransferRepository.GetAll().Where(w => w.WithdrawStatusType == (int)WithdrawStatusType.Processing && w.PaymentTransactionId != 0).ToList();
        }

        public IList<EcoPayzRequest> PendingEcoPayzRequestList()
        {
            return EcoPayzRewpository.GetAll().Where(e => e.WithdrawStatusType == (int)WithdrawStatusType.Processing && e.PaymentTransactionId != 0).ToList();
        }
    }
}
