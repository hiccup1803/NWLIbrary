using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts
{
    public class GenericResult
    {
        public bool IsSuccess { get; set; } 
        public string Message { get; set; } 
        public int ResponseCode { get; set; }
    }
}
