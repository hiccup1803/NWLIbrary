using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using NW.Core.Entities;

namespace NW.Data.NHibernate.Map.Member
{
    public class MemberCandidateMap : ClassMap<MemberCandidate>
    {
        public MemberCandidateMap()
        {
            Id(l => l.Id);
            Map(l => l.Phone);
            Map(l => l.CreateDate);
            Map(l => l.Username);
            Map(l => l.Email);
            Map(l => l.Firstname);
            Map(l => l.Lastname);
            Map(l => l.Code);
            Map(l => l.ConvertedDate);
            Map(l => l.MemberId);
            Map(l => l.ExpiryDate);
            Map(l => l.AnnotationId);

            Map(l => l.EmailTemplatePath);
            Map(l => l.EmailTemplateParams);
            Map(l => l.EmailSubject);
            Map(l => l.EmailSent);
        }
    }
}
