using NW.Core.Entities;
using NW.Core.Entities.Payment;
using NW.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Repositories.Payment
{
    public interface ICreditCardResponseRepository : IRepository<CreditCardResponse, int>
    {
        CreditCardResponse GetByCcRequestId(int requestId);
    }
}
