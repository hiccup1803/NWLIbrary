using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Payment
{
    public class MGCreditCardRequestMap : ClassMap<NW.Core.Entities.Payment.MGCreditCardRequest>
    {
        public MGCreditCardRequestMap()
        {
			Id(x => x.Id);
            Map(x => x.CompanyId);
            Map(x => x.PaymentStatusType);
            Map(x => x.ProviderId);
            Map(x => x.MemberId);
            Map(x => x.Amount);
            Map(x => x.CreateDate);

            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Note);
            Map(x => x.UpdateDate);
            Map(x => x.UpdateBy);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);




            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("MGCreditCardRequest");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
