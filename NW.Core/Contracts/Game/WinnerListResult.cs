using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW.Core.Entities;

namespace NW.Core.Contracts.Game
{
    public class WinnerListResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public Data Data { get; set; }
    }

    public class Data
    {
        public IList<Winner> Winners { get; set; }
    }
}
