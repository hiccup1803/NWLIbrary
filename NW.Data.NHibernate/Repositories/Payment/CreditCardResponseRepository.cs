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
    public class CreditCardResponseRepository : Repository<CreditCardResponse, int>, ICreditCardResponseRepository
    {
        public CreditCardResponseRepository(ISession _session) : base(_session) { }

        public CreditCardResponse GetByCcRequestId(int requestId)
        {
            return GetAll().FirstOrDefault(x => x.CCRequestId == requestId);
        }
    }
}