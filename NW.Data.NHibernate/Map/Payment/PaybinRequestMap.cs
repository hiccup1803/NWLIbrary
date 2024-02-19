using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class PaybinRequestMap : ClassMap<NW.Core.Entities.Payment.PaybinRequest>
    {
        public PaybinRequestMap()
        {
			Id(x => x.Id);
            Map(x => x.StatusType);
            Map(x => x.MemberId);
            Map(x => x.Amount);
            Map(x => x.ActualAmount);
            Map(x => x.Currency);
            Map(x => x.UniqueId);
            Map(x => x.OriginalAmount);
            Map(x => x.Symbol);
            Map(x => x.OrderId);
            Map(x => x.Data);
            Map(x => x.CallbackData);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);


            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("PaybinRequest");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
