using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Game
{
    public class GameMap : ClassMap<NW.Core.Entities.Game>
    {

        public GameMap()
        {
            Id(x => x.Id);
            Map(x => x.VoltronGameId);
            Map(x => x.CasinoGameTypeId);
            Map(x => x.Alias);
            Map(x => x.Name);
            Map(x => x.LogoURL);
            Map(x => x.ThumbnailURL);
            Map(x => x.ThumbnailHoverURL);
            Map(x => x.ImageURL);
            Map(x => x.Active);
            Map(x => x.Description);
            Map(x => x.CreateDate);
            Map(x => x.Width);
            Map(x => x.Height);
            Map(x => x.ResourceName);
            Map(x => x.IsMobile);
            Map(x => x.IsNewGame);
            Map(x => x.Vendor);

            HasMany(x => x.GameTags).KeyColumn("GameId").Inverse().Cascade.All();
            HasMany(x => x.GameCategories).KeyColumn("GameId").Inverse().Cascade.All();
            HasMany(x => x.GameDetails).KeyColumn("CasinoGameId").Inverse().Cascade.All();

            HasManyToMany(x => x.FavouritedByMembers)
                .Cascade.All()
                .Table("MemberFavouriteGame")
                .ParentKeyColumn("GameId")
                .ChildKeyColumn("MemberId");

            HasManyToMany(x => x.RestrictedCountries)
                .Cascade.All()
                .Table("CasinoGameRestrictedCountry")
                .ParentKeyColumn("GameId")
                .ChildKeyColumn("CountryId");

            Table("CasinoGame");
        }
    }
}
