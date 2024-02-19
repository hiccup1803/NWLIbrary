using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NW.Helper.SMS
{
    public class DicleSMSRequest
    {
        public static SMSResponse Send(string number, string text)
        {
            string xml = "<BILGI><KULLANICI_ADI>34B8365</KULLANICI_ADI><SIFRE>123456</SIFRE><GONDERIM_TARIH></GONDERIM_TARIH><BASLIK>BAYMAVI</BASLIK></BILGI><ISLEM><YOLLA><MESAJ>Dogrulama kodu: {0} , bu kodu siz talep etmediyseniz canli yardima baglanin. BAYMAVI</MESAJ><NO>{1}</NO></YOLLA></ISLEM>";

            WebRequest request = WebRequest.Create("http://www.diclemesaj.com/services/api.php?islem=sms");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(string.Format(xml, text, number));
            request.ContentType = "application/xml";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return new SMSResponse() { Success = responseFromServer.Contains("OK") };

        }
    }
}
