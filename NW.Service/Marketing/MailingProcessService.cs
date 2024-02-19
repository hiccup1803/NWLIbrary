using NHibernate;
using NW.Core.Entities;
using NW.Core.Entities.Marketing;
using NW.Core.Enum;
using NW.Core.Model;
using NW.Core.Repositories;
using NW.Core.Services;
using NW.Core.Work;
using NW.Helper;
using NW.Service;
using NW.Service.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NW.Services
{
    public class MailingProcessService : BaseService, IMailingProcessService
    {
        IMemberRepository MemberRepository { get; set; }
        IMemberDetailRepository MemberDetailRepository { get; set; }
        ICompanyDomainRepository CompanyDomainRepository { get; set; }
        IEmailAccountRepository EmailAccountRepository { get; set; }
        public MailingProcessService(IEmailAccountRepository _emailAccountRepository, IMemberRepository _memberRepository, ICompanyDomainRepository _companyDomainRepository, IMemberDetailRepository _memberDetailRepository, IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            EmailAccountRepository = _emailAccountRepository;
            MemberRepository = _memberRepository;
            CompanyDomainRepository = _companyDomainRepository;
            MemberDetailRepository = _memberDetailRepository;
        }

        private Dictionary<string, string> GeneralPageTokens(string protocol, string domain, string language)
        {
            return new Dictionary<string, string>()
            {
                { "domain_supportpage", (protocol + domain + "/" + language + "/static/support") },
                { "domain_contactus", (protocol + domain + "/" + language + "/static/support") },
                { "domain_promotion", (protocol + domain + "/" + language + "/promotions") },
                { "domain_casino", (protocol + domain + "/" + language + "/casino") },
                { "domain_livecasino", (protocol + domain + "/" + language + "/livecasino") },
                { "domain_mainpage", (protocol + domain + "/" + language) },
                { "domain_lostpassword", (protocol + domain + "/" + language + "/member/forgotpassword") },
                { "domain_link", (protocol + domain) },
            };
        }

        private string ReadContent(string url)
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
        private string ReplaceContent(string content, Dictionary<string, string> keyValues)
        {
            if (keyValues != null && keyValues.Count > 0)
            {
                foreach (KeyValuePair<string, string> kvp in keyValues)
                {
                    content = content.Replace(string.Format("${0}", kvp.Key), kvp.Value);
                }
            }
            return content;
        }

        public ResultModel SendForgottenPasswordMail(int companyId, string language, string usernameOrEmail)
        {
            ResultModel resultModel = new ResultModel();
            EmailType emailType = EmailType.ForgotPassword;
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    Member member = MemberRepository.ActiveMember(companyId, usernameOrEmail);

                    if (member != null)
                    {
                        string key = Guid.NewGuid().ToString();
                        MemberDetailRepository.InsertOrUpdate(member.Id, "LostPassword.Key", key);

                        string domain = CompanyDomainRepository.GetLiveDomain(companyId);
                        string protocol = "https://";
                        string protocolDomain = protocol + domain;

                        Dictionary<string, string> mailTokens = GeneralPageTokens(protocol, protocolDomain, language);
                        mailTokens.Add("FULLNAME", string.Format("{0} {1}", member.FirstName, member.LastName));
                        mailTokens.Add("USERNAME", string.Format("{0}", member.Username));
                        mailTokens.Add("domain_passwordreset", (protocolDomain + "/" + language + "/member/passwordrecovery?u=" + member.Id + "&k=" + key));

                        EmailAccount emailAccount = EmailAccountRepository.EmailAccountByEmailType(companyId, emailType);

                        resultModel = SendMail(companyId, emailAccount, language, member.Email, emailType, mailTokens);
                    }
                    else
                    {
                        resultModel.IsSuccess = false;
                        resultModel.Message = LocalizationHelper.Value(language, "Error", "WrongMailOrUsername");
                    }


                    unitOfWork.Commit(transaction);
                }


            }
            return resultModel;
        }


        public ResultModel SendConfirmationMail(int companyId, string language, string usernameOrEmail, Guid confirmationGuid)
        {
            ResultModel resultModel = new ResultModel();
            EmailType emailType = EmailType.ConfirmEmail;
            using (var unitOfWork = UnitOfWork.Current)
            {

                Member member = MemberRepository.ActiveMember(companyId, usernameOrEmail);

                if (member != null)
                {
                    string domain = CompanyDomainRepository.GetLiveDomain(companyId);
                    string protocol = "https://";
                    string protocolDomain = protocol + domain;

                    Dictionary<string, string> mailTokens = GeneralPageTokens(protocol, domain, language);
                    mailTokens.Add("username", member.Username);
                    mailTokens.Add("confirmation_link", string.Format("{0}/{1}/member/confirmation?e={2}&g={3}", protocolDomain, language, member.Email, confirmationGuid));

                    EmailAccount emailAccount = EmailAccountRepository.EmailAccountByEmailType(companyId, emailType);

                    resultModel = SendMail(companyId, emailAccount, language, member.Email, emailType, mailTokens);
                }
                else
                {
                    resultModel.IsSuccess = false;
                    resultModel.Message = LocalizationHelper.Value(language, "Error", "WrongMailOrUsername");
                }
            }
            return resultModel;
        }

        public ResultModel SendGenericBOMail(int companyId, string language, Member member, string content)
        {
            ResultModel resultModel = new ResultModel();
            EmailType emailType = EmailType.GenericBOMail;

            string domain = CompanyDomainRepository.GetLiveDomain(companyId);
            string protocol = "https://";
            string protocolDomain = protocol + domain;
            if (member != null)
            {
                Dictionary<string, string> mailTokens = GeneralPageTokens(protocol, domain, language);
                mailTokens.Add("fullname", member.FirstName + " " + member.LastName);
                mailTokens.Add("content", content);


                EmailAccount emailAccount = EmailAccountRepository.EmailAccountByEmailType(companyId, emailType);

                resultModel = SendMail(companyId, emailAccount, language, member.Email, emailType, mailTokens);
            }
            else
            {
                resultModel.IsSuccess = false;
                resultModel.Message = LocalizationHelper.Value(language, "Error", "WrongMailOrUsername");
            }
            return resultModel;
        }
        public ResultModel SendMail(int companyId, string language, string toMail, EmailType emailType, Dictionary<string, string> mailTokens)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                EmailAccount emailAccount = EmailAccountRepository.EmailAccountByEmailType(companyId, emailType);
                return SendMail(companyId, emailAccount, language, toMail, emailType, mailTokens);
            }
        }

        public ResultModel SendExceptionMail(int companyId, string language, string toMail, Dictionary<string, string> mailTokens, string action, string controller)
        {

            ResultModel resultModel = new ResultModel();
            EmailType emailType = EmailType.WebsiteError;
            using (var unitOfWork = UnitOfWork.Current)
            {
                EmailAccount emailAccount = EmailAccountRepository.EmailAccountByEmailType(companyId, emailType);

                string subject = LocalizationHelper.Value(language, "TransactionMail", string.Format("Subject.{0}", emailType));
                string content = ReplaceContent(ReadContent(LocalizationHelper.Value(language, "TransactionMail", string.Format("ContentPath.{0}", emailType))), mailTokens);

                subject = String.Format("{0} @ {1}/{2}", subject, controller, action);

                resultModel.IsSuccess = EmailSenderHelper.SendEmail(emailAccount.Email, emailAccount.SenderName, toMail, subject, content);
                if (!resultModel.IsSuccess)
                    resultModel.Message = LocalizationHelper.Value(language, "Error", "GenericErrorTryAgain");
                else
                    resultModel.Message = LocalizationHelper.Value(language, "Success", "CheckMail");

                return resultModel;
            }
        }
        private ResultModel SendMail(int companyId, EmailAccount emailAccount, string language, string toMail, EmailType emailType, Dictionary<string, string> mailTokens)
        {
            ResultModel resultModel = new ResultModel();

            string subject = LocalizationHelper.Value(language, "TransactionMail", string.Format("Subject.{0}.{1}", emailType, companyId));
            string content = ReplaceContent(ReadContent(LocalizationHelper.Value(language, "TransactionMail", string.Format("ContentPath.{0}.{1}", emailType, companyId))), mailTokens);

            resultModel.IsSuccess = EmailSenderHelper.SendEmail(emailAccount.Email, emailAccount.SenderName, toMail, subject, content);
            if (!resultModel.IsSuccess)
                resultModel.Message = LocalizationHelper.Value(language, "Error", "GenericErrorTryAgain");
            else
                resultModel.Message = LocalizationHelper.Value(language, "Success", "CheckMail");

            return resultModel;
        }
    }
}
