using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Game
{
    public class GameCategoryMap : ClassMap<NW.Core.Entities.GameCategory>
    {

        public GameCategoryMap()
        {
            Id(x => x.Id);
            Map(x => x.CategoryId);
            Map(x => x.GameId);
            Map(x => x.Active);
            Map(x => x.DisplayOrder);
            Map(x => x.CreateDate);

            References(x => x.Category).Column("CategoryId").Cascade.None().ReadOnly();
            References(x => x.Game).Column("GameId").Cascade.None().ReadOnly();

            Table("CasinoCategoryGame");
        }
    }
}
