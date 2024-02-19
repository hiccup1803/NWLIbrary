using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class LevelMap : ClassMap<Level>
    {
        public LevelMap()
        {
            Id(l => l.Id);
            Map(l => l.Name);
            Map(l => l.StatusType);
            Map(l => l.CreateDate);
            Map(l => l.SystemName);
            Map(l => l.CssClass);


            HasMany(x => x.LevelAttributes).KeyColumn("LevelId").Inverse().Cascade.All();
        }
    }
}
