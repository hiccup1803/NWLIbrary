using NW.Core.Entities;
using NW.Core.Model.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services.Payment
{
    public interface ICepbankPaymentService
    {
        CepbankPaymentModel CepbankFirstStep(int companyId, Member member, CepbankPaymentGenericViewModel cepbankPaymentGenericViewModel);
        string ProviderRefId(int companyId, bool isProduction,string token, string url, string bankName, Member member, string senderId, string receipientId, string senderPhone, string receipientPhone, string receipientBirthday, string password, long amount);
    }
}
