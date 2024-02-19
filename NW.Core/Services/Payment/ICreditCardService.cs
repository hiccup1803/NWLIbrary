using NW.Core.Entities;
using NW.Core.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services.Payment
{
    public interface ICreditCardService 
    {
        CreditCard GetCard(string cardNo);
        CreditCard GetCard(int id);
       
        bool CreditCardExist(string cardNumber);
    }
}
