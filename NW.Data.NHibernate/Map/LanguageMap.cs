using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map
{
    public class LanguageMap: ClassMap<NW.Core.Entities.Language> 
    {

        public LanguageMap()
        {
            Id(x => x.Id);
			Map(x => x.Name);
			Map(x => x.Culture);
            Map(x => x.IVRCode);
            Map(x => x.Status);

            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("Language");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}

