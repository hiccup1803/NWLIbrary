using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Payment
{
    public class PayKasaTransactionMap : ClassMap<NW.Core.Entities.Payment.PayKasaTransaction>
    {
        public PayKasaTransactionMap()
        {
			Id(x => x.Id);
			Map(x => x.MemberId);
            Map(x => x.Amount);
            Map(x => x.Request);
            Map(x => x.CreateDate);

            Map(x => x.Status);
            Map(x => x.Response);
            Map(x => x.WalletTransactionRefId);
            Map(x => x.ProviderRef);
          
           
            

            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("PayKasaTransaction");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
