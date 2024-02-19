using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class KingQRDepositRequestMap : ClassMap<NW.Core.Entities.Payment.KingQRDepositRequest>
    {
        public KingQRDepositRequestMap()
        {
            Id(x => x.Id);
            Map(x => x.MemberId);
            Map(x => x.StatusType);
            Map(x => x.Amount);
            Map(x => x.Currency);
            Map(x => x.ProviderRefId);
            Map(x => x.PaymentMethod);
            Map(x => x.SystemAdminUsername);
            Map(x => x.CallbackData);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);

            Table("KingQRDepositRequest");
        }
    }
}
