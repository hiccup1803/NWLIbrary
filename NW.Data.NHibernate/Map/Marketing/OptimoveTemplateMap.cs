using FluentNHibernate.Mapping;
using NW.Core.Entities.Marketing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Marketing
{
    public class OptimoveTemplateMap : ClassMap<OptimoveTemplate>
    {

        public OptimoveTemplateMap()
        {
            Id(x => x.Id);
            Map(x => x.TemplateType);
            Map(x => x.StatusType);
            Map(x => x.Name);
            Map(x => x.Content);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);

            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("OptimoveTemplate");
            //Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
