using FluentNHibernate.Mapping;
using NW.Core.Entities.Marketing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Marketing
{
    public class TournamentMap : ClassMap<Tournament>
    {
        public TournamentMap()
        {
            Id(x => x.Id);
            Map(x => x.GameTypeId);
            Map(x => x.CompanyId);
            Map(x => x.StartDate);
            Map(x => x.EndDate);
            Map(x => x.ThirdPartyTournamentId);
            Map(x => x.TotalPrize);
            Map(x => x.Terms).Length(4001);
            Map(x => x.Notes).Length(4001);
            Map(x => x.Name);
            Map(x => x.ImagePath);
            Map(x => x.PrizeList);
            Map(x => x.ResultList).Length(4001);
            Map(x => x.RandomPrizeList).Length(4001);
            Map(x => x.DisplayOrder);
            Map(x => x.IsVip);
            Map(x => x.TournamentType);
            Map(x => x.CreateDate);
            Map(x => x.StatusType);

            
            HasMany(x => x.TournamentGames).KeyColumn("TournamentId").Inverse().Cascade.All();


            Table("Tournament");
        }
    }
}
