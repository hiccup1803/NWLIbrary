using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class PayminoRequestMap : ClassMap<NW.Core.Entities.Payment.PayminoRequest>
    {
        public PayminoRequestMap()
        {
            Id(x => x.Id);
            Map(x => x.StatusType);
            Map(x => x.BankId);
            Map(x => x.MemberId);
            Map(x => x.Firstname);
            Map(x => x.Lastname);
            Map(x => x.Amount);
            Map(x => x.CreateDate);
            Map(x => x.PaymentProviderTransactionId);
            Map(x => x.UpdateDate);

            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("PayminoRequest");
            //Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
