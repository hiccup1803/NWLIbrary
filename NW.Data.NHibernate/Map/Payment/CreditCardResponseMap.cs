using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Payment
{
    public class CreditCardResponseMap : ClassMap<NW.Core.Entities.Payment.CreditCardResponse>
    {
        public CreditCardResponseMap()
        {
			Id(x => x.Id);
			Map(x => x.CCRequestId);
			Map(x => x.Amount);
            Map(x => x.Response);
            Map(x => x.BalanceRefId);
            Map(x => x.BalanceUpdated);
            Map(x => x.CreateDate);

            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("CreditCardResponse");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
