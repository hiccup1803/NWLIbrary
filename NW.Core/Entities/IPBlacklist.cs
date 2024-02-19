﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class IPBlacklist : Entity<int>
    {
        public virtual string IP { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime BlockTo { get; set; }
    }
}
