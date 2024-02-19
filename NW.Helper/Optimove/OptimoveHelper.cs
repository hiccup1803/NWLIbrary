using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Helper.Optimove
{
    public class OptimoveHelper
    {
        public const int SMS_CHANNEL = 505;
        public const int EMAIL_CHANNEL = 504;


        public static bool AddTemplate(int channelId, int templateId, string name)
        {
            bool result = false;
            try
            {
                string token = HttpHelper.PostRequest("https://api5.optimove.net/current/general/login", "{\"UserName\":\"CasinoNavy_api\",\"Password\":\"JSH72j$sn27MSJ$3m2ks8$3JS\"}", "application/json").Trim('\"');
                HttpHelper.PostRequest("https://api5.optimove.net/current/integrations/AddChannelTemplates?ChannelID=" + channelId, "[{\"TemplateID\":" + templateId + ",\"TemplateName\":\"" + name + "\"}]", "application/json", new NameValueCollection() { { "Authorization-Token", token } });
                result = true;
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static bool DeleteTemplate(int channelId, int templateId)
        {
            bool result = false;
            try
            {
                string token = HttpHelper.PostRequest("https://api5.optimove.net/current/general/login", "{\"UserName\":\"CasinoNavy_api\",\"Password\":\"JSH72j$sn27MSJ$3m2ks8$3JS\"}", "application/json").Trim('\"');
                HttpHelper.PostRequest("https://api5.optimove.net/current/integrations/DeleteChannelTemplates", "[{\"ChannelID\":" + channelId + ",\"TemplateID\":" + templateId + "}]", "application/json", new NameValueCollection() { { "Authorization-Token", token } });
                result = true;
            }
            catch (Exception ex)
            {
            }
            return result;
        }
    }
}
