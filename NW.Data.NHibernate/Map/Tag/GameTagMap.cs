using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Tag
{
    public class GameTagMap : ClassMap<GameTag>
    {

        public GameTagMap()
        {
            Id(x => x.Id);
            Map(x => x.TagId);
            Map(x => x.GameId);
            Map(x => x.Active);
            Map(x => x.CreateDate);

            References(x => x.Tag).Column("TagId").Cascade.None().ReadOnly();
            References(x => x.Game).Column("GameId").Cascade.None().ReadOnly();

            Table("CasinoGameTag");
        }
    }
}
