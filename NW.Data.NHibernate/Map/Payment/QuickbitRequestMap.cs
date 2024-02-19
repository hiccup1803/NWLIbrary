using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class QuickbitRequestMap : ClassMap<NW.Core.Entities.Payment.QuickbitRequest>
    {
        public QuickbitRequestMap()
        {
            Id(x => x.Id);
            Map(x => x.StatusType);
            Map(x => x.MemberId);
            Map(x => x.Firstname);
            Map(x => x.Lastname);
            Map(x => x.OriginalAmount);
            Map(x => x.Amount);
            Map(x => x.CreateDate);
            Map(x => x.PaymentProviderTransactionId);
            Map(x => x.UpdateDate);
            Map(x => x.ResponseData).Length(4001);
            Map(x => x.CompanyId);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);
            Table("QuickbitRequest");
        }
    }
}
