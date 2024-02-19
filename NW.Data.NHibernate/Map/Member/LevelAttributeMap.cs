using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class LevelAttributeMap : ClassMap<LevelAttribute>
    {
        public LevelAttributeMap()
        {
            Id(l => l.Id);
            Map(l => l.LevelId);
            Map(l => l.Key).Column("[Key]");
            Map(l => l.Value);


            HasOne(x => x.Level).ForeignKey("LevelId").Cascade.None();
        }
    }
}
