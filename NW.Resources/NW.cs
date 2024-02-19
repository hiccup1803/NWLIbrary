using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NW.Resources
{
    public class R
    {
        public static string Get(string _class, string name)
        {
            string cacheKey = string.Format("NW.Resources.Get-class:{0}-name:{1}-culture:{2}", _class, name, CultureInfo.CurrentCulture.ToString());
            string returnValue = string.Empty;
            if (HttpContext.Current.Cache[cacheKey] == null)
            {
                var val = HttpContext.GetGlobalResourceObject(_class, name, CultureInfo.CurrentCulture);
                if (val != null)
                {
                    returnValue = val.ToString();
                    HttpContext.Current.Cache.Insert(cacheKey, returnValue);
                }
                else
                {
                    returnValue = name;
                }
            }
            else
            {
                returnValue = HttpContext.Current.Cache[cacheKey].ToString();
            }
            return returnValue;
        }
        public static string Get(string _class, string name, string culture)
        {
            var val = HttpContext.GetGlobalResourceObject(_class, name, new CultureInfo(culture));
            return val == null ? name : val.ToString();
        }
    }
}
