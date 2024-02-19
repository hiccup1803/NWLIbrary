using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW.Core.Entities.Payment;
using NW.Core.Repositories.Payment;
using NW.Core.Work;
using NHibernate;

namespace NW.Data.NHibernate.Repositories
{
    public class CreditCardRequestRepository : Repository<CreditCardRequest, int>, ICreditCardRequestRepository
    {
        public CreditCardRequestRepository(ISession _session) : base(_session) { }

        public CreditCardRequest GetPendingReyPayRequest(int paymentProviderId, int memberId, bool smsAlreadySent)
        {
            return GetAll().OrderByDescending(x => x.Id).FirstOrDefault(x => x.MemberId == memberId && x.PaymentProviderId == paymentProviderId && x.IsComplete == smsAlreadySent && x.ProviderRefId != null && x.ProviderRefId != string.Empty);
        }

        public CreditCardRequest GetPendingReyPayRequest(int paymentProviderId, string reference, bool smsAlreadySent)
        {
            return GetAll().FirstOrDefault(x => x.PaymentProviderId == paymentProviderId && x.ProviderRefId.Equals(reference) && x.IsComplete == smsAlreadySent && x.ProviderRefId != null && x.ProviderRefId != string.Empty);
        }
    }
}
