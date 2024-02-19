using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using NW.Core.Entities;

namespace NW.Data.NHibernate.Maps
{
    public class PromotionMap : ClassMap<Promotion>
    {

        public PromotionMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.CountryId);
            Map(x => x.Title);
            Map(x => x.Terms).Length(4001);
            Map(x => x.Active);
            Map(x => x.IsVipPromo);
            Map(x => x.Summary);
            Map(x => x.ThumbPicturePath);
            Map(x => x.CreateDate).Column("CreatedDate");
            Map(x => x.StartDate);
            Map(x => x.ExpireDate);
            Map(x => x.OrderNumber);
            Map(x => x.PromotionType);
            Map(x => x.DataFilterCat);

            Map(x => x.UsernameList).Length(4001);


            //HasMany(x => x.MemberDetails).KeyColumn("MemberId").Inverse().Cascade.All();
            Table("Promotion");
            //Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}

