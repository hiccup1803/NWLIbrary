using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class ParazulaTransferRequestMap : ClassMap<NW.Core.Entities.Payment.ParazulaTransferRequest>
    {
        public ParazulaTransferRequestMap()
        {
            Id(x => x.Id);
            Map(x => x.PaymentStatusType);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.UpdateBy);
            Map(x => x.MemberId);
            Map(x => x.ParazulaTransferParazulaAccountId);
            Map(x => x.Amount);
            Map(x => x.UpdateAmount);
            Map(x => x.AccountNumber);
            Map(x => x.SenderFullName);
            Map(x => x.CompanyId);
            Map(x => x.WithBonus);
            Map(x => x.BonusId);
            Map(x => x.ProviderId);
            Map(x => x.Note);

            References(m => m.Member).Column("MemberId").ReadOnly();
            References(m => m.ParazulaTransferParazulaAccount).Column("ParazulaTransferParazulaAccountId").ReadOnly();

            Table("ParazulaTransferRequest");
        }
    }
}
