using FluentNHibernate.Mapping;
using NW.Core.Entities.Marketing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Marketing
{
    public class EmailAccountMap : ClassMap<EmailAccount> {

        public EmailAccountMap()
        {
			Id(x => x.Id);
            Map(x => x.CompanyId);
            Map(x => x.Email);
            Map(x => x.SenderName);
            Map(x => x.EmailType);
            Map(x => x.CreateDate);
            Map(x => x.IsDefault);

            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("EmailAccount");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
