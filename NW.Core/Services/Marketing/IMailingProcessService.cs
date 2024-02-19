using NW.Core.Entities;
using NW.Core.Enum;
using NW.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface IMailingProcessService
    {
        ResultModel SendForgottenPasswordMail(int companyId, string language, string usernameOrEmail);
        ResultModel SendConfirmationMail(int companyId, string language, string usernameOrEmail, Guid confirmationGuid);
        ResultModel SendGenericBOMail(int companyId, string language, Member member, string content);
        ResultModel SendMail(int companyId, string language, string toMail, EmailType emailType, Dictionary<string, string> mailTokens);
        ResultModel SendExceptionMail(int companyId, string language, string toMail, Dictionary<string, string> mailTokens, string action, string controller);
    }
}
