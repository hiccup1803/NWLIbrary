using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Model.Marketing
{
    public class MarketingBannerJsonModel
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string IframeUrl { get; set; }
        public string Url { get; set; }
        public IList<MarketingBannerItemJsonModel> Items{ get; set; }
    }
    public partial class MarketingBannerItemJsonModel
    {
        public string ImagePath { get; set; }
        public string Url { get; set; }
    }
}
