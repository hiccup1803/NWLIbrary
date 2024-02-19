using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Payment
{
    public class CreditCardMap : ClassMap<NW.Core.Entities.Payment.CreditCard>
    {
        public CreditCardMap()
        {
			Id(x => x.Id);
            Map(x => x.StatusType);
            Map(x => x.NameOnCard);
			Map(x => x.MemberId);
            Map(x => x.CardNumber);
            Map(x => x.CVV);
            Map(x => x.ExpiryMonth);
            Map(x => x.ExpiryYear);
            Map(x => x.CreateDate);

            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("CreditCard");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
