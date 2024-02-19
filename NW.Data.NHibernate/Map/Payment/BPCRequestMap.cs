using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class BPCRequestMap: ClassMap<NW.Core.Entities.Payment.BPCRequest>
    {
        public BPCRequestMap()
        {
			Id(x => x.Id);
            Map(x => x.StatusType);
            Map(x => x.BPCPaymentMethodTypeId);
            Map(x => x.MemberId);
            Map(x => x.Amount);
            Map(x => x.Currency);
            Map(x => x.ProviderRefId);
            Map(x => x.Data);
            Map(x => x.ResultData);
            Map(x => x.CallbackData);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);
            Map(x => x.RecognisedAmount);


            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("BPCRequest");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
