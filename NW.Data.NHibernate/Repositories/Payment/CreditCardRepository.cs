using NHibernate;
using NW.Core.Entities;
using NW.Core.Entities.Payment;
using NW.Core.Repositories;
using NW.Core.Repositories.Payment;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Repositories
{
    public class CreditCardRepository : Repository<CreditCard, int>, ICreditCardRepository
    {
        public CreditCardRepository(ISession _session) : base(_session) { }

        public CreditCard GetCard(string creditCardNumber, int expiryMonth, int expiryYear)
        {
            return GetAll().FirstOrDefault(x => x.CardNumber == creditCardNumber && x.ExpiryYear == expiryYear && x.ExpiryMonth == expiryMonth);
        }

        public void AddCard(int memberId,string nameOnCard,string cardNumber,string cvv,int expiryMonth, int expiryYear)
        {
            Insert(new CreditCard{MemberId = memberId,CardNumber = cardNumber,CreateDate = DateTime.Now,CVV = cvv,ExpiryMonth = expiryMonth,ExpiryYear = expiryYear,NameOnCard = nameOnCard});
        }

        public bool CreditCardExist(string cardNumber, int expiryMonth, int expiryYear)
        {
            return GetAll().FirstOrDefault(x => x.CardNumber == cardNumber && x.ExpiryYear==expiryYear && x.ExpiryMonth==expiryMonth) != null;
        }
    }
}
