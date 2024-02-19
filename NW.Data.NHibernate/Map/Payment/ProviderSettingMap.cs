using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using NW.Core.Entities;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Payment {
    
    
    public class BankTransferRequestProviderHistoryMap : ClassMap<NW.Core.Entities.Payment.BankTransferRequestProviderHistory> {

        public BankTransferRequestProviderHistoryMap()
        {
			Id(x => x.Id);
            Map(x => x.BankTransferRequestId);
            Map(x => x.ProviderId);
            Map(x => x.ChangedBy);
            Map(x => x.CreateDate);
            
            References(x => x.NewBankTransferRequest).Column("BankTransferRequestId").ReadOnly();

            Table("BankTransferRequestProviderHistory");
        }
    }
}
