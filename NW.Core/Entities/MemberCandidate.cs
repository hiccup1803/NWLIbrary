using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MemberCandidate : Entity<int>
    {
        public virtual string Phone { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual string Firstname { get; set; }
        public virtual string Lastname { get; set; }
        public virtual string Code { get; set; }
        public virtual DateTime? ConvertedDate { get; set; }
        public virtual int? MemberId { get; set; }
        public virtual int? AnnotationId { get; set; }
        public virtual DateTime? ExpiryDate { get; set; }


        public virtual string EmailTemplatePath { get; set; }
        public virtual string EmailTemplateParams { get; set; }
        public virtual string EmailSubject { get; set; }

        public virtual bool EmailSent { get; set; }
        
    }
}
