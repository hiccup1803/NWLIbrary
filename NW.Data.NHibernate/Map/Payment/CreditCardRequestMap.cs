using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Payment
{
    public class CreditCardRequestMap : ClassMap<NW.Core.Entities.Payment.CreditCardRequest>
    {
        public CreditCardRequestMap()
        {
			Id(x => x.Id);
			Map(x => x.PaymentProviderId);
			Map(x => x.MemberId);
            Map(x => x.CCId);
            Map(x => x.OriginalAmount);
            Map(x => x.Amount);
            Map(x => x.Request);
            Map(x => x.IsComplete);
            Map(x => x.ProviderRefId);
            Map(x => x.CreateDate);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);

            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("CreditCardRequest");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
