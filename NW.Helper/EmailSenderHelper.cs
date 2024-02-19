using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NW.Helper
{
    public class EmailSenderHelper
    {
        public enum EmailType
        {
            Welcome = 1,
            DepositPending = 20,
            DepositSuccess = 21,
            DepositFail = 22,
            ForgotPassword = 4,
            LoginFailedMultipleTimes = 5,
            ConfirmEmail = 6,
            ConfirmEmailSuccess = 7,
            WithdrawRequested = 8,
            WithdrawRejected = 9,
            WithdrawSuccess = 10,
            FreeSpins15 = 80,
            Davet1 = 81,
            Davet2 = 82,
            Davet3 = 83,
            Davet5 = 85,

            Freespin75 = 86,
            Freespin50 = 87,
            Freespin20 = 88,

            EcoPayz20 = 90,
            EcoPayz666 = 91,

            WebsiteError = 100,
            DomainBlocked = 110
        }


        public static string ReadContent(string url)
        {
            var webRequest = WebRequest.Create(url);
            var strContent = string.Empty;
            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                strContent = reader.ReadToEnd();
            }

            return strContent;
        }

        public static string GetHtmlEmailContent(string path, Dictionary<string, string> keyValues = null)
        {
            string content = ReplaceContent(ReadContent(path), keyValues);
            return content;
        }

        public static string GetHtmlEmailContent(string domain, string path, Dictionary<string, string> keyValues = null)
        {
            string baseContentUrl = "http://navycdn.azurewebsites.net/Newsletter/";
            string content = ReplaceContent(ReadContent(baseContentUrl + path), keyValues);
            return content;
        }

        public static string GetHtmlEmailContent(string domain, EmailType emailType, Dictionary<string, string> keyValues = null)
        {
            string baseContentUrl = "http://navycdn.azurewebsites.net/Newsletter/";
            string content = string.Empty;

            switch (emailType)
            {
                case EmailType.FreeSpins15:
                    content = ReplaceContent(ReadContent(baseContentUrl + "15freespin/15freespin.html"), keyValues);
                    break;
                case EmailType.Welcome:
                    content = ReplaceContent(ReadContent(baseContentUrl + "Welcome.html"), keyValues);
                    break;
                case EmailType.DepositPending:
                    content = ReplaceContent(ReadContent(baseContentUrl + "successfull_depozit/successfull_depozit.html"), keyValues);
                    break;
                case EmailType.DepositSuccess:
                    content = ReplaceContent(ReadContent(baseContentUrl + "successfull_depozit/successfull_depozit.html"), keyValues);
                    break;
                case EmailType.DepositFail:
                    content = ReplaceContent(ReadContent(baseContentUrl + "successfull_depozit/rejected_depozit.html"), keyValues);
                    break;
                case EmailType.ForgotPassword:
                    content = ReplaceContent(ReadContent(baseContentUrl + "Errors/forgotpassword.html"), keyValues);
                    break;
                case EmailType.LoginFailedMultipleTimes:
                    content = ReplaceContent(ReadContent(baseContentUrl + "Welcome.html"), keyValues);
                    break;
                case EmailType.ConfirmEmail:
                    content = ReplaceContent(ReadContent(baseContentUrl + "confirm_email/confirm_mail.html"), keyValues);
                    break;
                case EmailType.ConfirmEmailSuccess:
                    content = ReplaceContent(ReadContent(baseContentUrl + "confirm_email/successfull_mail.html"), keyValues);
                    break;
                case EmailType.WithdrawRequested:
                    //content = ReplaceContent(ReadContent(baseContentUrl + "Withdrawal/successfull_withdrawal.html"), keyValues);
                    content = ReplaceContent(ReadContent(baseContentUrl + "trans/withdraw/new.html"), keyValues);
                    break;
                case EmailType.WithdrawRejected:
                    content = ReplaceContent(ReadContent(baseContentUrl + "Withdrawal/rejected_withdrawal.html"), keyValues);
                    break;
                case EmailType.WithdrawSuccess:

                    content = ReplaceContent(ReadContent(baseContentUrl + "Withdrawal/successfull_withdrawal.html"), keyValues);
                    break;

                case EmailType.Davet1:
                    content = ReplaceContent(ReadContent(baseContentUrl + "sizicagiriyor/davetedildiniz.html"), keyValues);
                    break;

                case EmailType.Davet2:
                    content = ReplaceContent(ReadContent(baseContentUrl + "sizicagiriyor/davetedildiniz2.html"), keyValues);
                    break;
                case EmailType.Davet3:
                    content = ReplaceContent(ReadContent(baseContentUrl + "sizicagiriyor/davetedildiniz3.html"), keyValues);
                    break;

                case EmailType.Davet5:
                    content = ReplaceContent(ReadContent(baseContentUrl + "sizicagiriyor/davetedildiniz5.html"), keyValues);
                    break;

                case EmailType.Freespin75:
                    content = ReplaceContent(ReadContent(baseContentUrl + "invite/freespin75.html"), keyValues);
                    break;

                case EmailType.Freespin50:
                    content = ReplaceContent(ReadContent(baseContentUrl + "invite/freespin50.html"), keyValues);
                    break;

                case EmailType.Freespin20:
                    content = ReplaceContent(ReadContent(baseContentUrl + "invite/freespin20.html"), keyValues);
                    break;

                case EmailType.EcoPayz20:
                    content = ReplaceContent(ReadContent(baseContentUrl + "ecopayz_20/eco.html"), keyValues);
                    break;

                case EmailType.EcoPayz666:
                    content = ReplaceContent(ReadContent(baseContentUrl + "ecopayz_20/eco_666welcome.html"), keyValues);
                    break;

                case EmailType.WebsiteError:
                    content = ReplaceContent(ReadContent(baseContentUrl + "Errors/websiteerror.html"), keyValues);
                    break;
                case EmailType.DomainBlocked:
                    content = ReplaceContent(ReadContent(baseContentUrl + "domain_update/yeniadress.html"), keyValues);
                    break;
            }

            return content;
        }

        public static bool SendEmail(string fromEmail, string fromName, string toEmail, string subject, string content)
        {
            bool result = false;
            MailMessage email = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                IsBodyHtml = true
            };

            try
            {
                JArray array = (JArray)JsonConvert.DeserializeObject(HttpHelper.PostRequest("https://mandrillapp.com/api/1.0/messages/send.json", JsonConvert.SerializeObject(new
                {
                    key = "JLRwRbpuRBDLN35KXzgQEQ",
                    message = new
                    {
                        html = content,
                        subject = subject,
                        from_email = fromEmail,
                        from_name = fromName,
                        to = new[]
                        {
                            new { email = toEmail, type = "to" }
                        }
                    }
                }), "application/json"));


                result = array.FirstOrDefault()["status"].Value<string>() == "sent";





                //string baseContentUrl = "http://navycdn.azurewebsites.net/Newsletter/";

                //switch (emailType)
                //{
                //    case EmailType.Welcome:
                //        subject = "Avrupa'nın en iyi casinosuna hoşgeldiniz.";
                //        content = ReplaceContent(ReadContent(baseContentUrl + "Welcome.html"), keyValues);
                //        break;

                //    case EmailType.DepositPending:
                //        subject = "CepBank işleminiz bize ulaştı";
                //        content = ReplaceContent(ReadContent(baseContentUrl + "successfull_depozit/successfull_depozit.html"), keyValues);
                //        break;

                //    case EmailType.DepositSuccess:
                //        subject = "Tebrikler, para yatırma işleminiz başarılı bir şekilde tamamlandı";
                //        content = ReplaceContent(ReadContent(baseContentUrl + "successfull_depozit/successfull_depozit.html"), keyValues);
                //        break;

                //    case EmailType.DepositFail:
                //        subject = "Para yatırma işleminiz tamamlanamadı";
                //        content = ReplaceContent(ReadContent(baseContentUrl + "successfull_depozit/rejected_depozit.html"), keyValues);
                //        break;

                //    case EmailType.ForgotPassword:
                //        subject = "Şifre hatırlatma";
                //        content = ReplaceContent(ReadContent(baseContentUrl + "Errors/forgotpassword.html"), keyValues);
                //        break;

                //    case EmailType.LoginFailedMultipleTimes:
                //        subject = "Hesabınız bloklandı";
                //        content = ReplaceContent(ReadContent(baseContentUrl + "Welcome.html"), keyValues);
                //        break;

                //    case EmailType.ConfirmEmail:
                //        subject = "Eposta adresinizi doğrulayın";
                //        content = ReplaceContent(ReadContent(baseContentUrl + "confirm_email/confirm_mail.html"), keyValues);
                //        break;

                //    case EmailType.ConfirmEmailSuccess:
                //        subject = "Eposta adresinizi başarılı bir şekilde doğruladınız";
                //        content = ReplaceContent(ReadContent(baseContentUrl + "confirm_email/confirm_mail.html"), keyValues);
                //        break;

                //    case EmailType.WithdrawRequested:
                //        subject = "Para Çekim Talebiniz";
                //        //content = ReplaceContent(ReadContent(baseContentUrl + "Withdrawal/successfull_withdrawal.html"), keyValues);
                //        content = ReplaceContent(ReadContent(baseContentUrl + "trans/withdraw/new.html"), keyValues);
                //        break;

                //    case EmailType.WithdrawRejected:
                //        subject = "Para çekme talebiniz sonuçlandı";
                //        content = ReplaceContent(ReadContent(baseContentUrl + "Withdrawal/rejected_withdrawal.html"), keyValues);
                //        break;

                //    case EmailType.WithdrawSuccess:
                //        subject = "Para çekme talebiniz sonuçlandı";
                //        content = ReplaceContent(ReadContent(baseContentUrl + "Withdrawal/successfull_withdrawal.html"), keyValues);
                //        break;


                //    case EmailType.WebsiteError:
                //        subject = "Website Error";
                //        content = ReplaceContent(ReadContent(baseContentUrl + "Errors/websiteerror.html"), keyValues);
                //        break;
                //}

            }
            catch (Exception ex)
            {
            }

            return result;
        }

        public static string ReplaceContent(string content, Dictionary<string, string> keyValues)
        {
            if (keyValues != null && keyValues.Count > 0)
            {
                foreach (KeyValuePair<string, string> kvp in keyValues)
                {
                    content = content.Replace(string.Format("${0}", kvp.Key), kvp.Value); // this can be used as well ie: $username
                    content = content.Replace(string.Format("[{0}]", kvp.Key), kvp.Value); // this can be used as well ie: [username]
                }
            }
            return content;
        }
    }
}
