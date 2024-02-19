using NW.Core.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services.Payment
{
    public interface ICepBankService
    {
        IList<CepBank> CepBankList();

        string GetCepBankViewTemplateByCepBankId(int cepBankId);
        void InsertCepBankRequest(string domain, int paymentProviderId, int cepBankId, int memberId, string senderId, string receipientId, string senderPhone, string receipientPhone, string receipientBirthday, string password, long amount, bool withBonus, int? bonusId, string providerRefId);
        IList<CepBankRequest> PendingCepBankRequestList(int memberId);
        IList<CepBankRequest> SuccessfulCepBankRequestList(int memberId);
        IList<CepBankRequest> PendingCepBankRequestList(int memberId, int bankId);
    }
}
