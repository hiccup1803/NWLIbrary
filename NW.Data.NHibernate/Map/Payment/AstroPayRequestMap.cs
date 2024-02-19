using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Payment
{
    public class AstroPayRequestMap : ClassMap<NW.Core.Entities.Payment.AstroPayRequest>
    {
        public AstroPayRequestMap()
        {
			Id(x => x.Id);
			Map(x => x.MemberId);
            Map(x => x.CardAmount);
            Map(x => x.WalletAmount);
            Map(x => x.CreateDate);

            Map(x => x.CardCvv);
            Map(x => x.CardExpMonth);
            Map(x => x.CardExpYear);
            Map(x => x.CardNumber);
            Map(x => x.CardCurrency);
            Map(x => x.Status);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);

            Map(x => x.Type);
           
            

            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("AstroPayRequest");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
