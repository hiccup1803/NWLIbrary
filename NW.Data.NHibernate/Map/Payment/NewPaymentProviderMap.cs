using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Payment
{
    public class NewPaymentProviderMap : ClassMap<NW.Core.Entities.Payment.NewPaymentProvider>
    {
        public NewPaymentProviderMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.StatusType);
            Map(x => x.CreateDate);
            Map(x => x.Currency);
            Map(x => x.DisplayOrder);
            Map(x => x.VoltronProviderId);
            Map(x => x.ParentProviderId);
            Map(x => x.ThumbnailName);
            Map(x => x.SystemName);
            Map(x => x.MinAmount);
            Map(x => x.MaxAmount);
            Map(x => x.Weight);
            Map(x => x.HelplinkId);
            Map(x => x.ClassName);
            Map(x => x.ClosedCron);
            Map(x => x.ResourceName);

            Table("PaymentProvider");
        }
    }
}
