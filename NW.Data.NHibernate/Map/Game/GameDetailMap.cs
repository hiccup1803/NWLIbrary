using FluentNHibernate.Mapping;
using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Game
{
    public class GameDetailMap : ClassMap<GameDetail>
    {

        public GameDetailMap()
        {
            Id(x => x.Id);
            Map(x => x.GameId).Column("CasinoGameId");
            Map(x => x.Key).Column("[Key]");
            Map(x => x.Value);
            HasOne(x => x.Game).ForeignKey("GameId").Cascade.None();

            Table("CasinoGameDetail");
            //ManyToOne(x => x.Member, map => { map.Column("MemberId"); map.Cascade(Cascade.None); });

        }
    }
}
