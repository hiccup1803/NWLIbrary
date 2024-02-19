using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Service
{
    public class PostRequestModel
    {
        public bool Succes { get; set; }
        public JObject Obj { get; set; }
    }
}
