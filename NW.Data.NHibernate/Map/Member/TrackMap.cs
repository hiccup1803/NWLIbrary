using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.Member
{
    public class TrackMap : ClassMap<NW.Core.Entities.Track>
    {

        public TrackMap()
        {
            Id(x => x.Id);
            Map(x => x.ActionType);
            Map(x => x.CompanyId);
            Map(x => x.SessionId);
            Map(x => x.UserIdentity);
            Map(x => x.CreateDate);
            Map(x => x.Value);
            Map(x => x.Ip);
            Map(x => x.ExtraFields);
            
            Table("Track");
            //Bag(x => x.MemberDetails, colmap =>  { colmap.Key(x => x.Column("MemberId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
