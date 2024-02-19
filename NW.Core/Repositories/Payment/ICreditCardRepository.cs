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
    public interface ICreditCardRepository : IRepository<CreditCard, int>
    {
        CreditCard GetCard(string cardNumber, int expiryMonth, int expiryYear);
        void AddCard(int memberId,string nameOnCard,string cardNumber,string cvv,int expiryMonth, int expiryYear);
        bool CreditCardExist(string cardNumber,int expiryMonth,int expiryYear);
    }
}
