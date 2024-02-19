using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Game
{
    public class CountryMap : ClassMap<NW.Core.Entities.Country>
    {
        public CountryMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.A2Code);
            Map(x => x.A3Code);

             HasManyToMany(x => x.RestrictedGames)
                .Cascade.All()
                .Table("CasinoGameRestrictedCountry")
                .ParentKeyColumn("CountryId")
                .ChildKeyColumn("GameId");

             Table("Country");
        }
    }
}
