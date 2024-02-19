using FluentNHibernate.Mapping;
using NW.Core.Entities.Marketing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Marketing
{
    public class TournamentGameMap : ClassMap<TournamentGame>
    {
        public TournamentGameMap()
        {

            Id(x => x.Id);
            Map(x => x.CasinoGameId);
            Map(x => x.TournamentId);
            Map(x => x.DisplayOrder);
            Map(x => x.StatusType);


            References(x => x.CasinoGame).Column("CasinoGameId").ReadOnly();
            References(x => x.Tournament).Column("TournamentId").ReadOnly();

            Table("TournamentGame");
        }
    }
}
