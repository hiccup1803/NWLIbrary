﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class MessageTopic : Entity<int>
    {
        public virtual int MessageType { get; set; }
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int? CreatedBy { get; set; }
        public virtual int StatusType { get; set; }
        public virtual string SystemName { get; set; }
        public virtual int CompanyId { get; set; }
    }
}
