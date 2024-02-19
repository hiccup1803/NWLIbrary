using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using NW.Core.Entities;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Member{
    
    
    public class MemberWithDetailMap : ClassMap<NW.Core.Entities.MemberWithDetail> {

        public MemberWithDetailMap()
        {
			Id(x => x.Id);
			Map(x => x.Username);
			Map(x => x.Email);
			Map(x => x.StatusType);
			Map(x => x.Password);
			Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.UniqueId);
            Map(x => x.CompanyId);
            Map(x => x.LevelId);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.Currency);
            Map(x => x.BirthDate);
            Map(x => x.NetEntId);
            Map(x => x.Host);
            Map(x => x.AffCode);
            Map(x => x.IsTestAccount,"IsTest");
            Map(x => x.Phone);
            Map(x => x.ST);



            References(m => m.Company).Column("CompanyId").ReadOnly();
            References(m => m.Level).Column("LevelId").ReadOnly();


            HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();


            HasManyToMany(x => x.FavoriteGames)
                .Cascade.All()
                .Table("MemberFavouriteGame")
                .ParentKeyColumn("MemberId")
                .ChildKeyColumn("GameId");

            Table("SortableMember2");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
