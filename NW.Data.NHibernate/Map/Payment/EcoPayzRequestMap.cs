using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Payment
{
    public class EcoPayzRequestMap : ClassMap<NW.Core.Entities.Payment.EcoPayzRequest>
    {
        public EcoPayzRequestMap()
        {
			Id(x => x.Id);
			Map(x => x.MemberId);
            Map(x => x.TxBatchNumber);
            Map(x => x.Currency);
            Map(x => x.Amount);
            Map(x => x.CreateDate);

            Map(x => x.ProcessedByEco);
            Map(x => x.EcoTxId);
            Map(x => x.EcoXml);
            Map(x => x.EcoProcessedDate);

            Map(x => x.EcoClientAccountNumber);
            Map(x => x.RequestType);
            Map(x => x.WithdrawStatusType);
            Map(x => x.PaymentTransactionId);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);


            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("EcoRequest");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
