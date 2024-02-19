using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using NW.Core.Entities.Payment;

namespace NW.Data.NHibernate.Map.Payment
{
    public class OtoPayTransactionMap: ClassMap<OtoPayTransaction>
    {
        public OtoPayTransactionMap()
        {
            Id(x => x.Id, "OId");//.GeneratedBy.Assigned();
            Map(x => x.MemberId);
            Map(x => x.Amount);
            Map(x => x.TLAmount);
            Map(x => x.CardNumber);
            Map(x => x.CVV);
            Map(x => x.Description);
            Map(x => x.CreateDate);


            Map(x => x.StatusType);
            Map(x => x.WalletTransactionRefId);
            Map(x => x.Response);
            Map(x => x.ResponseDate);
            Map(x => x.OtoPayId);
            Map(x => x.OtoPayReference);

            Table("OtoPayTransaction");
        }
    }
}
