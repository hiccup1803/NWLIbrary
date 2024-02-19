using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class BPCPaymentMethodTypeMap : ClassMap<NW.Core.Entities.Payment.BPCPaymentMethodType>
    {
        public BPCPaymentMethodTypeMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.VoltronProviderId);


            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("BPCPaymentMethodType");
            //Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
