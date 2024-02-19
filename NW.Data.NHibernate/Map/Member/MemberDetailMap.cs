using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NW.Core.Entities;
using FluentNHibernate.Mapping;


namespace NW.Data.NHibernate.Map.Member {


    public class MemberDetailMap : ClassMap<MemberDetail>
    {
        
        public MemberDetailMap() {
            Id(x => x.Id);
            Map(x => x.MemberId);
            Map(x => x.Key).Column("[Key]");
            Map(x => x.Value);
            Map(x => x.CreateDate);
            Map(x => x.UpdateDate);
            HasOne(x => x.Member).ForeignKey("MemberId").Cascade.None(); 

            Table("MemberDetail");
			//ManyToOne(x => x.Member, map => { map.Column("MemberId"); map.Cascade(Cascade.None); });

        }
    }
}
