using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using NW.Core.Entities;
using FluentNHibernate.Mapping;

namespace NW.Data.NHibernate.Map.Member{
    
    
    public class MemberMap : ClassMap<NW.Core.Entities.Member> {
        
        public MemberMap() {
			Id(x => x.Id);
			Map(x => x.Username);
			Map(x => x.Email);
			Map(x => x.StatusType);
			Map(x => x.Password);
			Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.UniqueId);
            Map(x => x.SecondaryUniqueId);
            Map(x => x.CompanyId);
            Map(x => x.LevelId);
            Map(x => x.AffiliateTypeId);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            Map(x => x.Currency);
            Map(x => x.Host);
            Map(x => x.AffCode);
            Map(x => x.UnformattedUsername);
            Map(x => x.CashbackTypeId);
            Map(x => x.CashbackPercentage);
            Map(x => x.IsTestAccount, "IsTest");

            References(m => m.Company).Column("CompanyId").ReadOnly();
            References(m => m.Level).Column("LevelId").ReadOnly();
            References(m => m.AffiliateType).Column("AffiliateTypeId").ReadOnly();


            HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            HasMany(x => x.MemberTags).KeyColumn("MemberId").Inverse().Cascade.All();

            HasManyToMany(x => x.FavoriteGames)
                .Cascade.All()
                .Table("MemberFavouriteGame")
                .ParentKeyColumn("MemberId")
                .ChildKeyColumn("GameId");

            HasManyToMany(x => x.PowerUsers)
                .Cascade.All()
                .Table("PowerUserMemberLimit")
                .ParentKeyColumn("MemberId")
                .ChildKeyColumn("PowerUserId");

            HasManyToMany(x => x.MemberSegments)
                .Cascade.All()
                .Table("MemberSegmentMember")
                .ParentKeyColumn("MemberId")
                .ChildKeyColumn("MemberSegmentId");


            Table("Member");
			//Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
