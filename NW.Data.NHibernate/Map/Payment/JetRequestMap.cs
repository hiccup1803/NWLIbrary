using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class JetRequestMap: ClassMap<NW.Core.Entities.Payment.JetRequest>
    {
        public JetRequestMap()
        {
			Id(x => x.Id);
            Map(x => x.StatusType);
            Map(x => x.MemberId);
            Map(x => x.Amount);
            Map(x => x.Currency);
            Map(x => x.JetPaymentId);
            Map(x => x.JetToken);
            Map(x => x.JetResult);
            Map(x => x.CreateDate);
            Map(x => x.JetClientAccountNumber);
            Map(x => x.PaymentTransactionId);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);
            Map(x => x.CallbackData);
            Map(x => x.UpdateDate);
            Map(x => x.RecognisedAmount);


            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("JetRequest");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}