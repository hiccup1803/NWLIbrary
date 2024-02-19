using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class GameSortable : Entity<int>
    {
        public virtual int VoltronGameId { get; set; }
        public virtual int CasinoGameTypeId { get; set; }
        public virtual string Alias { get; set; }
        public virtual string Name { get; set; }
        public virtual string LogoURL { get; set; }
        public virtual string ThumbnailURL { get; set; }
        public virtual string ThumbnailHoverURL { get; set; }
        public virtual string ImageURL { get; set; }
        public virtual bool Active { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual bool IsMobile { get; set; }
        public virtual bool IsNewGame { get; set; }
        public virtual string Vendor { get; set; }



        //public virtual IEnumerable<GameTag> GameTags { get; set; }
        public virtual IEnumerable<GameCategory> GameCategories { get; set; }
        //public virtual IEnumerable<GameDetail> GameDetails { get; set; }
        //public virtual IEnumerable<Member> FavouritedByMembers { get; set; }
        public virtual IEnumerable<Country> RestrictedCountries { get; set; }
        public virtual decimal? Popularity { get; set; }
    }
}
