using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entitites
{
    public class Message : Entity<int>
    {
        public virtual int MessageTopicId { get; set; }
        public virtual int MemberId { get; set; }
        public virtual int MessageType { get; set; }
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime? ReadDate { get; set; }
        public virtual int? CreatedBy { get; set; }
        public virtual int StatusType { get; set; }
        public virtual MessageTopic MessageTopic { get; set; }
        public virtual Member Member { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
    }
}
