using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class LevelAttribute : Entity<int>
    {
        public virtual int LevelId { get; set; }
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }


        public virtual Level Level { get; set; }
    }
}
