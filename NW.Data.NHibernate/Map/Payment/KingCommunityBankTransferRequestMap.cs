using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class KingCommunityBankTransferRequestMap : ClassMap<NW.Core.Entities.Payment.KingCommunityBankTransferRequest>
    {
        public KingCommunityBankTransferRequestMap()
        {
            Id(x => x.Id);
            Map(x => x.MemberId);
            Map(x => x.StatusType);
            Map(x => x.Amount);
            Map(x => x.Currency);
            Map(x => x.ProviderRefId);
            Map(x => x.Bank);
            Map(x => x.Data);
            Map(x => x.ResultData);
            Map(x => x.CallbackData);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);
            Map(x => x.RecognisedAmount);



            Table("KingCommunityBankTransferRequest");
        }
    }
}
