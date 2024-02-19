using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class HeetpayRequestMap : ClassMap<NW.Core.Entities.Payment.HeetpayRequest>
    {
        public HeetpayRequestMap()
        {
            Id(x => x.Id);
            Map(x => x.StatusType);
            Map(x => x.Amount);
            Map(x => x.MemberId);
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.IdentityNumber);
            Map(x => x.ProviderRefId);
            Map(x => x.CallbackData);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);

            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("HeetpayRequest");
            //Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
