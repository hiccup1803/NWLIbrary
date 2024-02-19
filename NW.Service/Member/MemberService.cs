using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Security.Permissions;
using Logging;
using Newtonsoft.Json;
using NW.Core.Contracts;
using NW.Core.Contracts.Game;
using NW.Core.Contracts.Member;
using NW.Core.Contracts.Payment;
using NW.Core.Entities;
using NW.Core.Entities.Payment;
using NW.Core.Enum;
using NW.Core.Model;
using NW.Core.Repositories;
using NW.Core.Services;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW.Helper.SMS;
using NW.Security;
using NW.Service.Localization;
using NW.Service;
using RestSharp;
using NHibernate;
using System.Configuration;
using System.Globalization;
using NW.Helper;
using NW.Core.Services.Payment;
using NW.Core.Entities.Report;
using Newtonsoft.Json.Linq;
using NW.Helper.Kafka;
using NW.Core.Model.KafkaModel;
using NW.Helper.ServiceBus;

namespace NW.Services
{
    public class MemberService : BaseService, IMemberService
    {
        IMemberSegmentContainerService MemberSegmentContainerService { get; set; }
        IWithdrawContainerService WithdrawContainerService { get; set; }
        private IMailingProcessService MailingProcessService { get; set; }
        private IMemberRepository MemberRepository { get; set; }
        private IRepository<MemberPhoneVerification, int> MemberPhoneVerificationRepository { get; set; }
        private IRepository<MemberDisabledPaymentMethod, int> MemberDisabledPaymentMethodRepository { get; set; }
        private IMemberDetailRepository MemberDetailRepository { get; set; }
        private ICompanyService CompanyService { get; set; }
        private ICompanyDomainRepository CompanyDomainRepository { get; set; }
        private IGameRepository GameRepository { get; set; }
        private IRepository<MemberGameHistory, int> MemberGameHistoryRepository { get; set; }
        private IRepository<MemberTracking, int> MemberTrackingRepository { get; set; }
        private IRepository<MemberLoginLogout, int> MemberLoginLogoutRepository { get; set; }
        private IRepository<MemberPasscode, int> MemberPasscodeRepository { get; set; }
        private IRepository<MemberConnection, int> MemberConnectionRepository { get; set; }
        private IRepository<MemberNotification, int> MemberNotificationRepository { get; set; }
        private IRepository<MemberDeviceToken, int> MemberDeviceTokenRepository { get; set; }
        private IRepository<MemberCandidate, int> MemberCandidateRepository { get; set; }
        private IRepository<CustomStuff, int> CustomStuffRepository { get; set; }
        private ILevelRepository LevelRepository { get; set; }

        private IRepository<EcoPayzRequest, int> EcoPayzRepository { get; set; }
        private IRepository<MemberLoginCode, int> MemberLoginCodeRepository { get; set; }
        private IRepository<DepositBonusHistory, int> DepositBonusHistoryRepository { get; set; }
        private IMemberDeviceFingerPrintRepository MemberDeviceFingerPrintRepository { get; set; }
        private IRepository<DeviceFingerPrintWebhook, int> MemberDeviceFingerPrintWebhookRepository { get; set; }
        private IRepository<DeviceFingerPrintWebhookHistory, int> MemberDeviceFingerPrintWebhookHistoryRepository { get; set; }
        private IDeviceFingerPrintRepository DeviceFingerPrintRepository { get; set; }
        private IRepository<MemberDeviceFingerPrintHistory, int> MemberDeviceFingerPrintHistoryRepository { get; set; }
        private IRepository<IPLoginLog, int> IPLoginLogRepository { get; set; }
        private IRepository<IPBlacklist, int> IPBlacklistRepository { get; set; }
        private IRepository<MemberSegmentMember, int> MemberSegmentMemberRepository { get; set; }
        private IRepository<MemberSegment, int> MemberSegmentRepository { get; set; }



        private Logger Logger { get; set; }
        private const int HASH_COUNT = 10;
        private const int LOGIN_CODE_EXPIRY_MINUTE = -5;

        #region Keys

        public string RegisterAddressKey
        {
            get { return "Register.Address"; }
        }

        public string RegisterAddressFullLineKey
        {
            get { return "Register.AddressFullLine"; }
        }

        public string RegisterPostCodeKey
        {
            get { return "Register.PostCode"; }
        }

        public string RegisterBirthdayKey
        {
            get { return "Register.Birthday"; }
        }

        public string RegisterGenderKey
        {
            get { return "Register.Gender"; }
        }

        public string RegisterCityKey
        {
            get { return "Register.City"; }
        }

        public string RegisterCountryKey
        {
            get { return "Register.Country"; }
        }

        public string RegisterPhoneKey
        {
            get { return "Register.Phone"; }
        }

        public string RegisterNewsletterKey
        {
            get { return "Register.Newsletter"; }
        }

        public string RegisterSMSKey
        {
            get { return "Register.SMS"; }
        }

        public string RegisterPromotionsKey
        {
            get { return "Register.Promotions"; }
        }

        public string RegisterIdKey
        {
            get { return "Register.Identity"; }
        }

        public string RegisterConfirmationKey
        {
            get { return "Register.Confirmation"; }
        }

        public string VerificationEmailKey
        {
            get { return "Verification.Email"; }
        }

        public string VerificationPhoneKey
        {
            get { return "Verification.Phone"; }
        }

        public string VerificationTelegramKey
        {
            get { return "Verification.Telegram"; }
        }

        public string RegisterEmailVerifiedKey
        {
            get { return "Register.EmailVerifiedKey"; }
        }

        public string RegisterPhoneVerifiedKey
        {
            get { return "Register.PhoneVerifiedKey"; }
        }

        public string RegisterTelegramUsername
        {
            get { return "Register.TelegramUsername"; }
        }
        public string RegisterTelegramVerificationCode
        {
            get { return "Register.TelegramUserVerificationCode"; }
        }

        public string IntegrationVoltronId
        {
            get { return "Integration.Voltron"; }
        }

        public string InteractionLoginDateKey
        {
            get { return "Interaction.LoginDate"; }
        }

        public string InteractionLogoutDateKey
        {
            get { return "Interaction.LogoutDate"; }
        }

        public string BackOfficeNote
        {
            get { return "BackOffice.Note"; }
        }

        public string SportsBetLimit
        {
            get { return "Member.SportsBetLimit"; }
        }
        public string DepositBonusPercentage
        {
            get { return "Member.DepositBonusPercentage"; }
        }

        public string AffiliateRevenueCoefficient
        {
            get { return "Partner.AffiliateRevenueCoefficient"; }
        }
        public string DepositWithDifferentAccountEnabledKey
        {
            get { return "BackOffice.DepositWithDifferentAccountEnabled"; }
        }
        public string WithdrawWithDifferentAccountEnabledKey
        {
            get { return "BackOffice.WithdrawWithDifferentAccountEnabled"; }
        }
        public string AttentionOnWithdraw
        {
            get { return "Member.AttentionOnWithdraw"; }
        }
        public string WithdrawFlagUserKey
        {
            get { return "Withdraw.FlagUser"; }
        }
        public string KycBirthdayStatusType
        {
            get { return "KYC.Birthday.KycStatusType"; }
        }

        public string PassCode
        {
            get { return "Member.PassCode"; }
        }
        public string SMS2FAEnabledKey
        {
            get { return "2FA.SMS"; }
        }
        public string TwoFAEnabledKey
        {
            get { return "2FA.Enabled"; }
        }
        public string IsEnabledCryptoWithdrawEvenNotDeposit
        {
            get { return "Withdraw.CryptoEnabled"; }
        }
        public string WithdrawBTCAddress
        {
            get { return "Withdraw.BTC.Address"; }
        }
        #endregion

        public MemberService(IMemberSegmentContainerService _memberSegmentContainerService, IWithdrawContainerService _withdrawContainerService, IMailingProcessService _mailingProcessService, IMemberRepository _memberRepository,
            IMemberDetailRepository _memberDetailRepository,
            ICompanyService _companyService, ICompanyDomainRepository _companyDomainRepository,
            IGameRepository _gameRepository, IRepository<MemberGameHistory, int> _memberGameHistoryRepository, IRepository<MemberTracking, int> _memberTrackingRepository,
            IRepository<MemberLoginLogout, int> _memberLoginLogoutRepository, IRepository<MemberPasscode, int> _memberPasscodeRepository,
            IRepository<MemberConnection, int> _memberConnectionRepository,
            IRepository<MemberNotification, int> _memberNotificationRepository,
            IRepository<MemberDeviceToken, int> _memberDeviceTokenRepository,
            IRepository<EcoPayzRequest, int> _ecoPayzRepository,
            IRepository<MemberLoginCode, int> _memberLoginCodeRepository,
            IRepository<MemberPhoneVerification, int> _memberPhoneVerificationRepository,
            IRepository<MemberDisabledPaymentMethod, int> _memberDisabledPaymentMethodRepository,
            ILevelRepository _levelRepository, IRepository<MemberCandidate, int> _memberCandidateRepository, IRepository<CustomStuff, int> _customStuffRepository,
            IRepository<DepositBonusHistory, int> _depositBonusHistoryRepository,
            IMemberDeviceFingerPrintRepository _memberDeviceFingerPrintRepository,
            IRepository<DeviceFingerPrintWebhook, int> _memberDeviceFingerPrintWebhookRepository,
            IRepository<DeviceFingerPrintWebhookHistory, int> _memberDeviceFingerPrintWebhookHistoryRepository,
            IDeviceFingerPrintRepository _deviceFingerPrintRepository,
            IRepository<MemberDeviceFingerPrintHistory, int> _memberDeviceFingerPrintHistoryRepository,
            IRepository<IPLoginLog, int> _ipLoginLogRepository,
            IRepository<IPBlacklist, int> _ipBlacklistRepository,
            IRepository<MemberSegmentMember, int> _memberSegmentMemberRepository,
            IRepository<MemberSegment, int> _memberSegmentRepository,
            IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            MemberSegmentContainerService = _memberSegmentContainerService;
            WithdrawContainerService = _withdrawContainerService;
            MailingProcessService = _mailingProcessService;
            MemberRepository = _memberRepository;
            MemberDetailRepository = _memberDetailRepository;
            CompanyService = _companyService;
            CompanyDomainRepository = _companyDomainRepository;
            GameRepository = _gameRepository;
            MemberGameHistoryRepository = _memberGameHistoryRepository;
            MemberTrackingRepository = _memberTrackingRepository;
            MemberLoginLogoutRepository = _memberLoginLogoutRepository;
            MemberPasscodeRepository = _memberPasscodeRepository;
            LevelRepository = _levelRepository;
            MemberConnectionRepository = _memberConnectionRepository;
            MemberCandidateRepository = _memberCandidateRepository;
            CustomStuffRepository = _customStuffRepository;
            MemberNotificationRepository = _memberNotificationRepository;
            MemberDeviceTokenRepository = _memberDeviceTokenRepository;
            EcoPayzRepository = _ecoPayzRepository;
            MemberLoginCodeRepository = _memberLoginCodeRepository;
            MemberPhoneVerificationRepository = _memberPhoneVerificationRepository;
            MemberDisabledPaymentMethodRepository = _memberDisabledPaymentMethodRepository;
            DepositBonusHistoryRepository = _depositBonusHistoryRepository;
            MemberDeviceFingerPrintRepository = _memberDeviceFingerPrintRepository;
            MemberDeviceFingerPrintWebhookRepository = _memberDeviceFingerPrintWebhookRepository;
            MemberDeviceFingerPrintWebhookHistoryRepository = _memberDeviceFingerPrintWebhookHistoryRepository;
            DeviceFingerPrintRepository = _deviceFingerPrintRepository;
            MemberDeviceFingerPrintHistoryRepository = _memberDeviceFingerPrintHistoryRepository;
            IPLoginLogRepository = _ipLoginLogRepository;
            IPBlacklistRepository = _ipBlacklistRepository;
            MemberSegmentMemberRepository = _memberSegmentMemberRepository;
            MemberSegmentRepository = _memberSegmentRepository;
            Logger = new Logger("MemberService");
        }

        public bool UpdateDesktopNotification(string domain, string username, bool granted, string subscriptionId, string endpoint)
        {
            bool v = false;
            var member = GetActiveMember(domain, username);
            if (member != null)
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        var entity = MemberNotificationRepository.GetAll().SingleOrDefault(w => w.MemberId == member.Id && w.Endpoint == endpoint);
                        if (entity != null)
                        {
                            entity.DesktopPushEnabled = granted;
                            //entity.Endpoint = endpoint;
                            //entity.SubscriptionId = subscriptionId;
                            //entity.IsGCM = endpoint.Contains("android.googleapis.com/gcm/send");
                            uniOfWork.Commit(transaction);
                            v = granted;
                        }
                        else
                        {
                            entity = new MemberNotification
                            {
                                MemberId = member.Id,
                                DesktopPushEnabled = granted,
                                FacebookPushEnabled = granted,
                                MobilePushEnabled = granted,
                                CreateDate = DateTime.UtcNow,
                                Endpoint = endpoint,
                                SubscriptionId = subscriptionId,
                                IsGCM = endpoint.Contains("android.googleapis.com/gcm/send")

                            };
                            MemberNotificationRepository.Insert(entity);
                            uniOfWork.Commit(transaction);
                        }
                    }
                }
            }
            return v;
        }

        public List<string> GetMemberCandidates(int annotationId, int skip, int take)
        {
            var members =
                MemberCandidateRepository.GetAll()
                    .Where(w => w.AnnotationId == annotationId && w.Email != null && w.EmailSent != true)
                    .Skip(skip)
                    .Take(take).Select(w => w.Email).ToList();
            return members;
        }

        public List<MemberCandidate> GetMemberCandidates(int skip, int take)
        {
            var members =
                MemberCandidateRepository.GetAll().Where(w => w.Email != null && w.EmailSent != true).Skip(skip)
                    .Take(take).ToList();
            return members;
        }

        public bool IsMemberCandidateRegistered(string domain, string language, string mobilePhone)
        {
            var entity =
                MemberCandidateRepository.GetAll().SingleOrDefault(w => w.Phone == mobilePhone && w.MemberId == null);
            return entity == null;
        }

        public int UpdateMemberCandidateWithPhoneNumber(string domain, string language, string mobilePhone, int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    var entity =
                        MemberCandidateRepository.GetAll()
                            .SingleOrDefault(w => w.Phone == mobilePhone && w.MemberId == null);
                    if (entity != null)
                    {
                        entity.ConvertedDate = DateTime.Now;
                        entity.MemberId = memberId;

                        uniOfWork.Commit(transaction);
                        return entity.Id;
                    }
                }
            }
            return 0;
        }

        public ResultModel Step1Register(string domain, string language, string username, string email, string password,
            string aff)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                ResultModel result = new ResultModel();
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    int? comapanyId = CompanyService.CompanyId(domain);
                    if (!comapanyId.HasValue)
                    {
                        result.Message = LocalizationHelper.Value(language, "Generic", "SiteDomain.NotMatched");
                    }
                    else if (!MemberRepository.ValidateStep1RegisterEmail(comapanyId.Value, email))
                    {
                        result.Message = LocalizationHelper.Value(language, "Validation", "RegisterStep1Model.UsedEmail");
                    }
                    else if (!MemberRepository.ValidateStep1RegisterUsername(comapanyId.Value, username))
                    {
                        result.Message = LocalizationHelper.Value(language, "Validation",
                            "RegisterStep1Model.UsedUsername");
                    }
                    else if (
                        !MemberRepository.ValidateStep1Password(comapanyId.Value, password,
                            username))
                    {
                        result.Message = LocalizationHelper.Value(language, "Validation", "Register.Password");
                    }
                    else
                    {

                        NW.Core.Entities.Member member = new NW.Core.Entities.Member();
                        member.Email = email;
                        member.Username = username.Trim();
                        member.Password = SecurityHelper.MD5Encryption(password);
                        member.UniqueId = string.Empty;
                        member.StatusType = (int)StatusType.RegistrationStep1;
                        member.CompanyId = comapanyId.HasValue ? comapanyId.Value : 0;
                        member.CreateDate = DateTime.Now;
                        member.UpdateDate = member.CreateDate;
                        member.Currency = string.Empty;
                        //member.LevelId = LevelRepository.Level("pff").Id;
                        member.Host = domain;
                        member.AffCode = aff;
                        member.UnformattedUsername = username;

                        try
                        {
                            MemberRepository.Insert(member);
                            result.IsSuccess = true;

                            uniOfWork.Commit(transaction);
                        }
                        catch (Exception ex)
                        {
                            result.Message = ex.Message;

                            uniOfWork.Rollback(transaction);
                        }
                    }
                }

                return result;

            }
        }

        public virtual List<string> GetMembersEmails(string domain)
        {
            var emails = new List<string>();

            int? comapanyId = CompanyService.CompanyId(domain);
            if (comapanyId.HasValue)
            {
                emails =
                    MemberRepository.GetAll()
                        .Where(x => !x.IsTestAccount && x.StatusType == 1)
                        .Select(w => w.Email)
                        .ToList();
            }

            return emails;
        }

        public virtual Member GetActiveMember(string domain, string emailOrUsername)
        {
            Member member = null;
            using (var uniOfWork = UnitOfWork.Current)
            {
                //uniOfWork.BeginTransaction(Session, IsolationLevel.Snapshot);
                int? companyId = CompanyService.CompanyId(domain);
                if (companyId.HasValue)
                {
                    member = MemberRepository.ActiveMember(companyId.Value, emailOrUsername);
                }
            }
            return member;
        }

        public virtual Member GetActiveMember(int companyId, string emailOrUsername)
        {
            Member member = null;
            using (var uniOfWork = UnitOfWork.Current)
            {
                member = MemberRepository.ActiveMember(companyId, emailOrUsername);
            }
            return member;
        }
        public virtual Member GetMember(int companyId, string emailOrUsername)
        {
            Member member = null;
            using (var uniOfWork = UnitOfWork.Current)
            {
                member = MemberRepository.Member(companyId, emailOrUsername, new int[] { (int)StatusType.Active, (int)StatusType.Passive, (int)StatusType.KYC, (int)StatusType.BonusAbuser });
            }
            return member;
        }


        public virtual Member GetMemberByMemberDetail(string key, string value)
        {
            Member member = null;
            using (var uniOfWork = UnitOfWork.Current)
            {
                //uniOfWork.BeginTransaction(Session, IsolationLevel.Snapshot);
                int memberId = MemberDetailRepository.MemberId(key, value);
                member = MemberRepository.Get(memberId);
            }
            return member;
        }

        public List<string> GetUsernameListByMemberDetailList(string key, string[] valueList)
        {
            List<string> usernameList = new List<string>();
            using (var uniOfWork = UnitOfWork.Current)
            {
                foreach (var item in valueList.Select((value, index) => new { value, index })
                    .GroupBy(x => x.index / 2000)
                    .Select(x => x.Select(v => v.value).ToList())
                    .ToList())
                {
                    var memberIdList = MemberDetailRepository.GetAll().Where(md => md.Key == key && item.Contains(md.Value)).Select(md => md.MemberId).ToArray();
                    var tempUserList = MemberRepository.GetAll().Where(m => memberIdList.Contains(m.Id)).Select(m => m.Username).ToArray();
                    usernameList.AddRange(tempUserList.Distinct());//double result idk why
                }
            }
            return usernameList;
        }

        public Member GetMember(int memberId)
        {
            Member member = null;
            using (var uniOfWork = UnitOfWork.Current)
            {
                //uniOfWork.BeginTransaction(Session, IsolationLevel.Snapshot);
                member = MemberRepository.Get(memberId);
            }
            return member;
        }

        public void UpdateMember(Member member)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    MemberRepository.Update(member); // @TODO: kilit vuruyor mu burayi bir kontrol etmek lazim @matt 20160727
                    uniOfWork.Commit(transaction);
                }
            }
        }

        public string GetMemberEcoPayzNumber(string domain, string usernameOrEmail)
        {
            int? companyId = CompanyService.CompanyId(domain);
            if (companyId != null)
            {
                var member = MemberRepository.ActiveMember(companyId.Value, usernameOrEmail);
                if (member != null)
                {
                    var eco =
                        EcoPayzRepository.GetAll()
                            .FirstOrDefault(
                                w =>
                                    w.MemberId == member.Id && w.RequestType == 1 && w.PaymentTransactionId > 0 &&
                                    w.EcoClientAccountNumber > 0);
                    if (eco != null)
                    {
                        return eco.EcoClientAccountNumber.ToString();
                    }
                }
            }
            return "";
        }

        public ResultModel ValidatePassword(string domain, string password, string firstName, string lastName,
            string username)
        {
            ResultModel result = new ResultModel();
            int? comapanyId = CompanyService.CompanyId(domain);
            bool validPassword = MemberRepository.ValidatePassword(comapanyId.Value, password, firstName, lastName,
                username);
            if (validPassword)
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = LocalizationHelper.Value("TR", "Validation", "Register.Password");
            }
            return result;
        }

        public ResultModel Register(string domain, string language, string ip, string email, string password,
            string username, string firstName, string lastName, string day, string month, string year, string gender,
            string country, string city, string phone, string address, bool isProduction, string hash = "",
            string promoCode = "", string affCode = "", string cSource = "", string cMedium = "", string cName = "", string refUrl = "")
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                ResultModel result = new ResultModel();
                DateTime parseResult;
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    int? comapanyId = CompanyService.CompanyId(domain);
                    if (!comapanyId.HasValue)
                    {
                        result.Message = LocalizationHelper.Value(language, "Generic", "SiteDomain.NotMatched");
                    }
                    else if (!MemberRepository.ValidateStep1RegisterEmail(comapanyId.Value, email))
                    {
                        result.Message = LocalizationHelper.Value(language, "Validation", "RegisterStep1Model.UsedEmail");
                    }
                    else if (!MemberRepository.ValidateStep1RegisterUsername(comapanyId.Value, username))
                    {
                        result.Message = LocalizationHelper.Value(language, "Validation",
                            "RegisterStep1Model.UsedUsername");
                    }
                    else if (!DateTime.TryParse(string.Format("{0}.{1}.{2}", day, month, year), out parseResult))
                    {
                        result.Message = LocalizationHelper.Value(language, "Validation", "Register.NotValidDate");
                    }
                    else if (DateTime.Now.Year -
                             DateTime.ParseExact(string.Format("{0}.{1}.{2}", day, month, year), "dd.MM.yyyy",
                                 CultureInfo.InvariantCulture).Year < 18)
                    {
                        result.Message = LocalizationHelper.Value(language, "Validation", "Register.NotOver18Years");
                    }
                    //else if (
                    //    !MemberRepository.ValidatePassword(comapanyId.Value, password, firstName, lastName,
                    //        username))
                    //{
                    //    result.Message = LocalizationHelper.Value(language, "Validation", "Register.Password");
                    //}
                    else
                    {
                        Member member = MemberRepository.Member(comapanyId.Value, email, (int)StatusType.RegistrationStep1);
                        try
                        {
                            var partnerUrlToRegisterUser = CompanyService.GetValue(comapanyId.Value,
                                "Voltron.ServiceURL", isProduction);
                            var apiUsername = CompanyService.GetValue(comapanyId.Value, "Voltron.APIUsername",
                                isProduction);
                            var apiPassword = CompanyService.GetValue(comapanyId.Value, "Voltron.APIPassword",
                                isProduction);
                            var vCompanyId = CompanyService.GetValue(comapanyId.Value, "Voltron.CompanyId",
                                isProduction);

                            string disabledPaymentMethodList = CompanyService.GetValue(comapanyId.Value, "Register.DisabledPaymentMethodIdList",
                                isProduction);
                            int[] defaultDisabledPaymentProviderIdList = string.IsNullOrEmpty(disabledPaymentMethodList) ? new int[0] : disabledPaymentMethodList.Split(',').Select(i => int.Parse(i)).ToArray();

                            // TODO: Change this currency when system supports multiple currencies. @cem
                            string currency = CompanyService.GetValue(comapanyId.Value, "Register.DefaultCurrency", isProduction);
                            string birthdayDate = parseResult.ToString("yyyy-MM-dd");
                            string secondaryUniqueId = Guid.NewGuid().ToString("N");

                            var response =
                                HttpServiceHelper.PostJsonRequest(partnerUrlToRegisterUser + "register",
                                    JsonConvert.SerializeObject(new
                                    {
                                        companyId = vCompanyId,
                                        apiUsername = apiUsername,
                                        username = member.Username,
                                        email = member.Email,
                                        title = string.Empty,
                                        password = member.Password,
                                        firstName = firstName,
                                        lastName = lastName,
                                        gender = gender,
                                        birthdate = birthdayDate,
                                        mobileCode = "00",
                                        mobileNumber = phone,
                                        address = "00",
                                        zipCode = "00",
                                        city = "00",
                                        country = country,
                                        currency = currency,
                                        ip = ip,
                                        promoCode = promoCode,
                                        affCode = affCode,
                                        cSource = cSource,
                                        cMedium = cMedium,
                                        cName = cName,
                                        refUrl = refUrl,
                                        secondaryUniqueId = secondaryUniqueId,
                                        checksum = SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, member.Username, currency)
                                    }));

                            if (response.status != null && Convert.ToBoolean(response.status) ||
                                response.status == null)
                            {
                                if (Convert.ToBoolean(response.success))
                                {
                                    member.FirstName = firstName;
                                    member.LastName = lastName;
                                    member.CompanyId = comapanyId.Value;
                                    member.UpdateDate = DateTime.Now;
                                    member.Currency = currency;
                                    member.StatusType = (int)StatusType.Active;
                                    member.UniqueId = response.uniqueIdentifier;
                                    member.SecondaryUniqueId = secondaryUniqueId;
                                    member.AffCode = affCode;
                                    member.LevelId = LevelRepository.Level("pff").Id;
                                    MemberRepository.Update(member);

                                    Guid confirmationGuid = Guid.NewGuid();
                                    MemberDetailRepository.InsertOrUpdate(member.Id, IntegrationVoltronId,
                                        Convert.ToString(response.userId));
                                    if (!string.IsNullOrEmpty(birthdayDate))
                                        MemberDetailRepository.InsertOrUpdate(member.Id, RegisterBirthdayKey, birthdayDate);
                                    if (!string.IsNullOrEmpty(gender))
                                        MemberDetailRepository.InsertOrUpdate(member.Id, RegisterGenderKey, gender);
                                    if (!string.IsNullOrEmpty(country))
                                        MemberDetailRepository.InsertOrUpdate(member.Id, RegisterCountryKey, country);
                                    if (!string.IsNullOrEmpty(phone))
                                        MemberDetailRepository.InsertOrUpdate(member.Id, RegisterPhoneKey, phone);
                                    MemberDetailRepository.InsertOrUpdate(member.Id, RegisterNewsletterKey, true.ToString());
                                    MemberDetailRepository.InsertOrUpdate(member.Id, RegisterSMSKey, true.ToString());
                                    MemberDetailRepository.InsertOrUpdate(member.Id, RegisterPromotionsKey, true.ToString());
                                    MemberDetailRepository.InsertOrUpdate(member.Id, SMS2FAEnabledKey, true.ToString());
                                    if (!string.IsNullOrEmpty(address))
                                        MemberDetailRepository.InsertOrUpdate(member.Id, RegisterAddressKey, address);
                                    if (!string.IsNullOrEmpty(city))
                                        MemberDetailRepository.InsertOrUpdate(member.Id, RegisterCityKey, city);
                                    MemberDetailRepository.InsertOrUpdate(member.Id, RegisterConfirmationKey, confirmationGuid.ToString());
                                    MemberDetailRepository.InsertOrUpdate(member.Id, "Register.IP", ip);
                                    foreach (int providerId in defaultDisabledPaymentProviderIdList)
                                    {
                                        MemberDisabledPaymentMethodRepository.Insert(new MemberDisabledPaymentMethod() { CreateDate = DateTime.UtcNow, MemberId = member.Id, ProviderId = providerId });
                                    }
                                    //MemberDisabledPaymentMethodRepository.Insert(new MemberDisabledPaymentMethod() { CreateDate = DateTime.UtcNow, MemberId = member.Id, ProviderId = 33 });//disabled bank transfer for every registered person
                                    //MemberDisabledPaymentMethodRepository.Insert(new MemberDisabledPaymentMethod() { CreateDate = DateTime.UtcNow, MemberId = member.Id, ProviderId = 66 });//disabled cashlink bank transfer for every registered person 
                                    //MemberDisabledPaymentMethodRepository.Insert(new MemberDisabledPaymentMethod() { CreateDate = DateTime.UtcNow, MemberId = member.Id, ProviderId = 47 });//disabled paymino bank transfer for every registered person
                                    //MemberDisabledPaymentMethodRepository.Insert(new MemberDisabledPaymentMethod() { CreateDate = DateTime.UtcNow, MemberId = member.Id, ProviderId = 32 });//disabled default cepbank for every registered person
                                    //MemberDisabledPaymentMethodRepository.Insert(new MemberDisabledPaymentMethod() { CreateDate = DateTime.UtcNow, MemberId = member.Id, ProviderId = 38 });//disabled default reypay credit card for every registered person

                                    if (!string.IsNullOrEmpty(hash))
                                        MemberDeviceFingerPrintRepository.Insert(new MemberDeviceFingerPrint() { CreateDate = DateTime.UtcNow, Hash = hash, MemberId = member.Id, StatusType = (int)NW.Core.Enum.MemberDeviceFingerPrintStatusType.Whitelisted, UpdateDate = DateTime.UtcNow });


                                    uniOfWork.Commit(transaction);
                                    result.IsSuccess = true;

                                    ServiceBusHelper.InsertQueue(isProduction, "memberregistered", JsonConvert.SerializeObject(new { id = member.Id, username = member.Username, uniqueId = member.UniqueId, firstName = member.FirstName, lastName = member.LastName, affCode = member.AffCode, birthdate= birthdayDate, gender = gender, phone = phone, ip = ip, fingerprintHash = hash, timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                                    try
                                    {
                                        SendConfirmationMail(domain, language, member.Email, confirmationGuid);
                                    }
                                    catch (Exception)
                                    {
                                        // couldnt send email.
                                    }
                                }
                                else
                                {
                                    result.Message = response.message + response.message2;
                                    uniOfWork.Rollback(transaction);
                                }
                            }
                            else
                            {
                                throw new Exception("Request failed when trying to login to partner:" +
                                                    partnerUrlToRegisterUser);
                            }
                        }
                        catch (Exception ex)
                        {
                            result.Message = ex.Message;
                            uniOfWork.Rollback(transaction);
                        }

                    }

                }

                return result;
            }
        }


        public ResultModel MemberEmailVerification(string domain, string language, string email, string hash)
        {
            ResultModel result = new ResultModel();
            int? comapanyId = CompanyService.CompanyId(domain);
            Member member;
            using (var uniOfWork = UnitOfWork.Current)
            {
                member = MemberRepository.ActiveMember(comapanyId.Value, email);
            }
            if (member != null)
            {
                KeyValuePair<string, string> memberDetail =
                   MemberDetails(member.Id, RegisterConfirmationKey).FirstOrDefault();
                result.IsSuccess = memberDetail.Value == hash;
                if (result.IsSuccess)
                {
                    result.Message = LocalizationHelper.Value(language, "MemberEmailVerification", "Success");
                    UpdateMemberDetails(member.Id,
                        new Dictionary<string, string>()
                    {
                        {RegisterConfirmationKey, string.Empty},
                        {VerificationEmailKey, true.ToString()}
                    });
                    //EmailSenderHelper.SendEmail(domain, member.Email, EmailSenderHelper.EmailType.ConfirmEmailSuccess, new Dictionary<string, string>() { { "USERNAME", member.Username } });
                }
                else
                {
                    if (string.IsNullOrEmpty(memberDetail.Value))
                    {
                        result.Message = LocalizationHelper.Value(language, "MemberEmailVerification", "AlreadyVerified");
                    }
                    else
                    {
                        result.Message = LocalizationHelper.Value(language, "MemberEmailVerification", "HashHasBeenChanged");
                    }
                }

            }
            else
            {
                result.Message = LocalizationHelper.Value(language, "Login", "Login.EmailNotActive");
            }


            return result;
        }

        public ResultModel MemberEmailVerificationInformation(string domain, string language, string username)
        {
            ResultModel result = new ResultModel();
            int? comapanyId = CompanyService.CompanyId(domain);
            Member member;
            Guid confirmationGuid = Guid.NewGuid();
            using (var uniOfWork = UnitOfWork.Current)
            {
                member = MemberRepository.ActiveMember(comapanyId.Value, username);
            }

            KeyValuePair<string, string> memberDetail = MemberDetails(member.Id, VerificationEmailKey).FirstOrDefault();

            if (string.IsNullOrEmpty(memberDetail.Value))
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    MemberDetailRepository.InsertOrUpdate(member.Id, RegisterConfirmationKey,
                        confirmationGuid.ToString());
                }

                result.Message = LocalizationHelper.Value(language, "MemberEmailVerification", "MailHasBeenSent");
                SendConfirmationMail(domain, language, member.Email, confirmationGuid);
            }
            else
            {
                result.IsSuccess = true;
                result.Message = LocalizationHelper.Value(language, "MemberEmailVerification", "AccountHasBeenVerified");
            }
            return result;
        }

        private void SendConfirmationMail(string domain, string language, string email, Guid confirmationGuid)
        {
            int? comapanyId = CompanyService.CompanyId(domain);

            MailingProcessService.SendConfirmationMail(comapanyId.Value, language, email, confirmationGuid);
        }

        public bool AddGameHistory(string domain, string username, int gameId)
        {
            Member member = GetActiveMember(domain, username);
            int memberId = member != null ? member.Id : 0;

            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    MemberGameHistoryRepository.Insert(new MemberGameHistory()
                    {
                        MemberId = memberId,
                        CreateDate = DateTime.UtcNow,
                        GameId = gameId,
                        PFF = false,
                        SessionId = ""
                    });

                    transaction.Commit();
                }
            }


            return true;
        }

        public Core.Contracts.GenericResult ApplyWelcomeFreespins(string domain, string username, int numberOfFreeRound, int spinType, int spinValue, int voltronGameId, bool isProduction)
        {
            Core.Contracts.GenericResult result = new Core.Contracts.GenericResult { IsSuccess = false };

            int? comapanyId = CompanyService.CompanyId(domain);
            if (comapanyId.HasValue)
            {
                var partnerUrl = CompanyService.GetValue(comapanyId.Value, "Voltron.ServiceURL", isProduction);
                var voltronCompanyId = CompanyService.GetValue(comapanyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(comapanyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(comapanyId.Value, "Voltron.APIPassword", isProduction);

                var member = MemberRepository.Member(comapanyId.Value, username);


                string partnerLoginUrl = (partnerUrl + "ApplyFreeSpin/?username=" + username +
                                          "&apiUsername=" + apiUsername + "&companyId=" +
                                          voltronCompanyId + "&freeRound=" + numberOfFreeRound + "&gameId=" + voltronGameId + "&spinType=" + spinType + "&spinValue=" + spinValue + "&jurisdictionCode=&checksum=" +
                                          SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, voltronCompanyId,
                                              apiUsername, username, voltronGameId.ToString(), spinType.ToString(), numberOfFreeRound.ToString(), spinValue.ToString(), string.Empty).ToUpper()//jurisdictionCode=string.Empty
                                              );

                var response = HttpServiceHelper.GetJsonRequest((partnerLoginUrl));

                if (response.status != null && Convert.ToBoolean(response.status) || response.status == null)
                {
                    if (Convert.ToBoolean(response.Success))
                    {
                        result.IsSuccess = true;
                    }
                    result.Message = JsonConvert.SerializeObject(response) + "," + response.Message;
                }
                else
                {
                    throw new Exception("Request failed to the partner:" + partnerLoginUrl);
                }
            }
            else
            {
                throw new Exception("Company is not populated");
            }

            return result;
        }
        public Core.Contracts.GenericResult ApplyFreebet(string domain, string username, string couponCode, long amount, DateTime expirationDate, string currency, string country, bool isProduction)
        {
            Core.Contracts.GenericResult result = new Core.Contracts.GenericResult { IsSuccess = false };

            int? comapanyId = CompanyService.CompanyId(domain);
            if (comapanyId.HasValue)
            {
                var partnerUrl = CompanyService.GetValue(comapanyId.Value, "Voltron.ServiceURL", isProduction);
                var voltronCompanyId = CompanyService.GetValue(comapanyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(comapanyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(comapanyId.Value, "Voltron.APIPassword", isProduction);

                var member = MemberRepository.Member(comapanyId.Value, username);


                string partnerLoginUrl = (partnerUrl + "ApplyFreebet/?username=" + username +
                                          "&apiUsername=" + apiUsername + "&companyId=" +
                                          voltronCompanyId + "&amount=" + amount + "&couponCode=" + couponCode + "&expirationDate=" + expirationDate.ToString("yyyy-MM-dd") + "&currency=" + currency + "&country=" + country + "&checksum=" +
                                          SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, voltronCompanyId,
                                              apiUsername, username, amount.ToString(), couponCode, expirationDate.ToString("yyyy-MM-dd"), currency, country).ToUpper()
                                              );

                var response = HttpServiceHelper.GetJsonRequest((partnerLoginUrl));

                if (response.status != null && Convert.ToBoolean(response.status) || response.status == null)
                {
                    if (Convert.ToBoolean(response.Success))
                    {
                        result.IsSuccess = true;
                    }
                }
                else
                {
                    throw new Exception("Request failed to the partner:" + partnerLoginUrl);
                }
            }
            else
            {
                throw new Exception("Company is not populated");
            }

            return result;
        }

        public Core.Contracts.GenericResult EligibleForFreespin(string domain, string username, int spinType, bool isProduction)
        {
            Core.Contracts.GenericResult result = new Core.Contracts.GenericResult { IsSuccess = false };

            int? comapanyId = CompanyService.CompanyId(domain);
            if (comapanyId.HasValue)
            {
                var partnerUrl = CompanyService.GetValue(comapanyId.Value, "Voltron.ServiceURL", isProduction);
                var voltronCompanyId = CompanyService.GetValue(comapanyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(comapanyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(comapanyId.Value, "Voltron.APIPassword", isProduction);

                var member = MemberRepository.Member(comapanyId.Value, username);


                string partnerLoginUrl = (partnerUrl + "EligibleForFreeSpin/?username=" + username +
                                          "&apiUsername=" + apiUsername + "&companyId=" +
                                          voltronCompanyId + "&spinType=" + spinType + "&checksum=" +
                                          SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, voltronCompanyId,
                                              apiUsername, username, spinType.ToString()).ToUpper()
                                              );

                //int memberId = member != null ? member.Id : 0;
                //MemberGameHistoryRepository.Insert(new MemberGameHistory()
                //{
                //    MemberId = memberId,
                //    CreateDate = DateTime.Now,
                //    GameId = game.Id,
                //    PFF = pff,
                //    SessionId = sessionId
                //});


                var response = HttpServiceHelper.GetJsonRequest((partnerLoginUrl));

                if (response.status != null && Convert.ToBoolean(response.status) || response.status == null)
                {
                    if (Convert.ToBoolean(response.Success))
                    {
                        result.IsSuccess = true;
                        result.Message = response.Message;
                    }
                }
                else
                {
                    throw new Exception("Request failed to the partner:" + partnerLoginUrl);
                }

            }
            else
            {
                throw new Exception("Company is not populated");
            }


            return result;

        }
        public Core.Contracts.GenericResult EligibleForFreebet(string domain, string username, string couponCode, long amount, DateTime expirationDate, string currency, string country, bool isProduction)
        {
            Core.Contracts.GenericResult result = new Core.Contracts.GenericResult { IsSuccess = false };

            int? comapanyId = CompanyService.CompanyId(domain);
            if (comapanyId.HasValue)
            {
                var partnerUrl = CompanyService.GetValue(comapanyId.Value, "Voltron.ServiceURL", isProduction);
                var voltronCompanyId = CompanyService.GetValue(comapanyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(comapanyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(comapanyId.Value, "Voltron.APIPassword", isProduction);

                var member = MemberRepository.Member(comapanyId.Value, username);


                string partnerLoginUrl = (partnerUrl + "EligibleForFreebet/?username=" + username +
                                          "&apiUsername=" + apiUsername + "&companyId=" +
                                          voltronCompanyId + "&amount=" + amount + "&couponCode=" + couponCode + "&expirationDate=" + expirationDate.ToString("yyyy-MM-dd") + "&currency=" + currency + "&country=" + country + "&checksum=" +
                                          SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, voltronCompanyId,
                                              apiUsername, username, amount.ToString(), couponCode, expirationDate.ToString("yyyy-MM-dd"), currency, country).ToUpper()
                                              );


                var response = HttpServiceHelper.GetJsonRequest((partnerLoginUrl));

                if (response.status != null && Convert.ToBoolean(response.status) || response.status == null)
                {
                    if (Convert.ToBoolean(response.Success))
                    {
                        result.IsSuccess = true;
                        result.Message = response.Message;
                    }
                }
                else
                {
                    throw new Exception("Request failed to the partner:" + partnerLoginUrl);
                }

            }
            else
            {
                throw new Exception("Company is not populated");
            }


            return result;

        }
        public Core.Contracts.GenericResult EligibleForDepositBonus(int memberId, int voltronTransactionTypeId)
        {
            Core.Contracts.GenericResult result = new Core.Contracts.GenericResult { IsSuccess = false };

            result.IsSuccess = !DepositBonusHistoryRepository.GetAll().Any(dbh => dbh.MemberId == memberId && dbh.VoltronTransactionTypeId == voltronTransactionTypeId);

            return result;

        }
        public PlayGameResult Play(string domain, string language, string username, int slug, string sessionId,
            bool pff, bool isProduction)
        {

            PlayGameResult result = new PlayGameResult { IsSuccess = false };
            string URL = string.Empty;
            using (var uniOfWork = UnitOfWork.Current)
            {
                int? comapanyId = CompanyService.CompanyId(domain);
                if (comapanyId.HasValue)
                {
                    var partnerUrlToRegisterUser = CompanyService.GetValue(comapanyId.Value, "Voltron.ServiceURL",
                        isProduction);
                    var voltronCompanyId = CompanyService.GetValue(comapanyId.Value, "Voltron.CompanyId", isProduction);
                    var apiUsername = CompanyService.GetValue(comapanyId.Value, "Voltron.APIUsername", isProduction);
                    var apiPassword = CompanyService.GetValue(comapanyId.Value, "Voltron.APIPassword", isProduction);
                    var game = GameRepository.Game(slug);
                    var member = MemberRepository.Member(comapanyId.Value, username);
                    var voltronGameId = game.VoltronGameId;

                    if (game.CasinoGameTypeId == 17)
                        partnerUrlToRegisterUser = partnerUrlToRegisterUser.Replace("-ja.", ".").Replace("live.", "live-us.");

                    string partnerLoginUrl = (partnerUrlToRegisterUser + "GameURL/?username=" + username +
                                              "&apiUsername=" + apiUsername + "&companyId=" +
                                              voltronCompanyId + "&language=" + language + "&gameId=" +
                                              voltronGameId + "&pff=" + pff + "&returnDomain=" + domain + "&jurisdictionCode=&memberToken=&checksum=" +
                                              SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword,
                                                  voltronCompanyId, apiUsername, username, language,
                                                  voltronGameId.ToString(), pff.ToString(), domain, string.Empty, string.Empty));//jurisdictionCode+memberToken

                    Logger.Fatal("username: " + username + " url: " + partnerLoginUrl);

                    //int memberId = member != null ? member.Id : 0;
                    //MemberGameHistoryRepository.Insert(new MemberGameHistory()
                    //{
                    //    MemberId = memberId,
                    //    CreateDate = DateTime.Now,
                    //    GameId = game.Id,
                    //    PFF = pff,
                    //    SessionId = sessionId
                    //});


                    var response = HttpServiceHelper.GetJsonRequest((partnerLoginUrl));

                    if (response.status != null && Convert.ToBoolean(response.status) || response.status == null)
                    {
                        result.IsSuccess = Convert.ToBoolean(response.success);
                        if (Convert.ToBoolean(response.success))
                        {
                            URL = response.GameURL;
                            result.GameLaunchUrl = URL;
                            result.BackgroundImage = game.ImageURL;
                            result.Description = game.Description;
                            result.Width = game.Width == 0 ? 640 : game.Width;
                            result.Height = game.Height == 0 ? 480 : game.Height;
                        }
                        Logger.Fatal("username: " + username + " game url: " + result.GameLaunchUrl + " response: " + response);
                    }
                    else
                    {
                        Logger.Fatal("EX: username: " + username + " url: " + partnerLoginUrl + " response: " + response);
                        throw new Exception("Request failed when trying to login to partner:" + partnerLoginUrl);
                    }

                }
                else
                {
                    throw new Exception("Company is not populated");
                }

                return result;
            }
        }
        public PlayGameResult Sportsbook(string domain, string language, string username, bool isProduction)
        {

            PlayGameResult result = new PlayGameResult { IsSuccess = true };
            string URL = string.Empty;
            using (var uniOfWork = UnitOfWork.Current)
            {
                int? comapanyId = CompanyService.CompanyId(domain);
                if (comapanyId.HasValue)
                {
                    //if(!string.IsNullOrEmpty(username))
                    //{
                    var partnerUrlToRegisterUser = CompanyService.GetValue(comapanyId.Value, "Voltron.ServiceURL",
                       isProduction);
                    var voltronCompanyId = CompanyService.GetValue(comapanyId.Value, "Voltron.CompanyId", isProduction);
                    var apiUsername = CompanyService.GetValue(comapanyId.Value, "Voltron.APIUsername", isProduction);
                    var apiPassword = CompanyService.GetValue(comapanyId.Value, "Voltron.APIPassword", isProduction);

                    string uniqueId = string.Empty;
                    if (!string.IsNullOrEmpty(username))
                    {
                        var member = MemberRepository.Member(comapanyId.Value, username);
                        uniqueId = member == null ? string.Empty : (voltronCompanyId + "~" + member.UniqueId.ToString());
                    }

                    string partnerLoginUrl = (partnerUrlToRegisterUser + "SportsURL/?apiUsername=" + apiUsername + "&companyId=" +
                                              voltronCompanyId + "&urlType=1&language=" + language + "&memberUniqueId=" +
                                              uniqueId + "&returnDomain=" + domain + "&checksum=" +
                                              SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword,
                                                  voltronCompanyId, 1.ToString(), apiUsername, uniqueId, language, domain));//jurisdictionCode+memberToken

                    Logger.Fatal("username: " + username + " url: " + partnerLoginUrl);

                    //int memberId = member != null ? member.Id : 0;
                    //MemberGameHistoryRepository.Insert(new MemberGameHistory()
                    //{
                    //    MemberId = memberId,
                    //    CreateDate = DateTime.Now,
                    //    GameId = game.Id,
                    //    PFF = pff,
                    //    SessionId = sessionId
                    //});


                    var response = HttpServiceHelper.GetJsonRequest((partnerLoginUrl));

                    if (response.status != null && Convert.ToBoolean(response.status) || response.status == null)
                    {
                        result.IsSuccess = Convert.ToBoolean(response.success);
                        if (Convert.ToBoolean(response.success))
                        {
                            result.GameLaunchUrl = response.GameURL;
                        }
                        Logger.Fatal("username: " + username + " game url: " + result.GameLaunchUrl + " response: " + response);
                    }
                    else
                    {
                        Logger.Fatal("EX: username: " + username + " url: " + partnerLoginUrl + " response: " + response);
                        throw new Exception("Request failed when trying to login to partner:" + partnerLoginUrl);
                    }
                    //}
                }
                else
                {
                    throw new Exception("Company is not populated");
                }

                return result;
            }
        }
        public PlayGameResult SportsbookSessionId(string domain, string uniqueId, bool isProduction)
        {

            PlayGameResult result = new PlayGameResult { IsSuccess = false };
            string sessionId = string.Empty;
            using (var uniOfWork = UnitOfWork.Current)
            {
                int? comapanyId = CompanyService.CompanyId(domain);
                if (comapanyId.HasValue)
                {
                    var partnerUrlToRegisterUser = CompanyService.GetValue(comapanyId.Value, "Voltron.ServiceURL",
                        isProduction);
                    var voltronCompanyId = CompanyService.GetValue(comapanyId.Value, "Voltron.CompanyId", isProduction);
                    var apiUsername = CompanyService.GetValue(comapanyId.Value, "Voltron.APIUsername", isProduction);
                    var apiPassword = CompanyService.GetValue(comapanyId.Value, "Voltron.APIPassword", isProduction);


                    string partnerLoginUrl = (partnerUrlToRegisterUser + "SportsSessionId/?apiUsername=" + apiUsername + "&companyId=" +
                                              voltronCompanyId + "&memberUniqueId=" +
                                              uniqueId + "&checksum=" +
                                              SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword,
                                                  voltronCompanyId, apiUsername, uniqueId));//jurisdictionCode+memberToken

                    Logger.Fatal("uniqueId: " + uniqueId + " url: " + partnerLoginUrl);

                    //int memberId = member != null ? member.Id : 0;
                    //MemberGameHistoryRepository.Insert(new MemberGameHistory()
                    //{
                    //    MemberId = memberId,
                    //    CreateDate = DateTime.Now,
                    //    GameId = game.Id,
                    //    PFF = pff,
                    //    SessionId = sessionId
                    //});


                    var response = HttpServiceHelper.GetJsonRequest((partnerLoginUrl));

                    if (response.status != null && Convert.ToBoolean(response.status) || response.status == null)
                    {
                        result.IsSuccess = Convert.ToBoolean(response.success);
                        if (Convert.ToBoolean(response.success))
                        {
                            sessionId = response.sessionId;
                            result.GameLaunchUrl = sessionId;
                        }
                        Logger.Fatal("username: " + uniqueId + " game url: " + result.GameLaunchUrl + " response: " + response);
                    }
                    else
                    {
                        Logger.Fatal("EX: username: " + uniqueId + " url: " + partnerLoginUrl + " response: " + response);
                        throw new Exception("Request failed when trying to login to partner:" + partnerLoginUrl);
                    }

                }
                else
                {
                    throw new Exception("Company is not populated");
                }

                return result;
            }
        }
        public PlayGameResult SportsbookHistory(string domain, string language, string username, bool isProduction)
        {

            PlayGameResult result = new PlayGameResult { IsSuccess = false };
            string URL = string.Empty;
            using (var uniOfWork = UnitOfWork.Current)
            {
                int? comapanyId = CompanyService.CompanyId(domain);
                if (comapanyId.HasValue)
                {
                    var partnerUrlToRegisterUser = CompanyService.GetValue(comapanyId.Value, "Voltron.ServiceURL",
                        isProduction);
                    var voltronCompanyId = CompanyService.GetValue(comapanyId.Value, "Voltron.CompanyId", isProduction);
                    var apiUsername = CompanyService.GetValue(comapanyId.Value, "Voltron.APIUsername", isProduction);
                    var apiPassword = CompanyService.GetValue(comapanyId.Value, "Voltron.APIPassword", isProduction);

                    string uniqueId = string.Empty;
                    if (!string.IsNullOrEmpty(username))
                    {
                        var member = MemberRepository.Member(comapanyId.Value, username);
                        uniqueId = member == null ? string.Empty : (voltronCompanyId + "~" + member.UniqueId.ToString());
                    }

                    string partnerLoginUrl = (partnerUrlToRegisterUser + "SportsURL/?apiUsername=" + apiUsername + "&companyId=" +
                                              voltronCompanyId + "&urlType=2&language=" + language + "&memberUniqueId=" +
                                              uniqueId + "&returnDomain=" + domain + "&checksum=" +
                                              SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword,
                                                  voltronCompanyId, 2.ToString(), apiUsername, uniqueId, language, domain));//jurisdictionCode+memberToken

                    Logger.Fatal("username: " + username + " url: " + partnerLoginUrl);

                    //int memberId = member != null ? member.Id : 0;
                    //MemberGameHistoryRepository.Insert(new MemberGameHistory()
                    //{
                    //    MemberId = memberId,
                    //    CreateDate = DateTime.Now,
                    //    GameId = game.Id,
                    //    PFF = pff,
                    //    SessionId = sessionId
                    //});


                    var response = HttpServiceHelper.GetJsonRequest((partnerLoginUrl));

                    if (response.status != null && Convert.ToBoolean(response.status) || response.status == null)
                    {
                        result.IsSuccess = Convert.ToBoolean(response.success);
                        if (Convert.ToBoolean(response.success))
                        {
                            URL = response.GameURL;
                            result.GameLaunchUrl = URL;
                        }
                        Logger.Fatal("username: " + username + " game url: " + result.GameLaunchUrl + " response: " + response);
                    }
                    else
                    {
                        Logger.Fatal("EX: username: " + username + " url: " + partnerLoginUrl + " response: " + response);
                        throw new Exception("Request failed when trying to login to partner:" + partnerLoginUrl);
                    }

                }
                else
                {
                    throw new Exception("Company is not populated");
                }

                return result;
            }
        }
        //public BalanceResult UpdateBalance(string domain, string usernameOrEmail, long amountToAdd, long partnerRef, bool isProduction)
        //{
        //    BalanceResult result = new BalanceResult { IsSuccess = false, Message = "" };
        //    using (var uniOfWork = UnitOfWork.Current)
        //    {


        //        int? comapanyId = CompanyDomainRepository.CompanyId(domain);
        //        //var member = MemberRepository.Member(comapanyId.Value, emailOrUsername, (int)StatusType.Active);

        //        var partnerUrl = CompanyService.GetValue(comapanyId.Value, "Voltron.ServiceURL", isProduction);
        //        var method = "UpdateRealBalance";

        //        var client = new RestClient(partnerUrl);
        //        // client.Authenticator = new HttpBasicAuthenticator(username, password);

        //        var request = new RestRequest(method, Method.POST);
        //        request.AddParameter("companyId", comapanyId); // adds to POST or URL querystring based on Method
        //        request.AddParameter("username", usernameOrEmail); // adds to POST or URL querystring based on Method
        //        request.AddParameter("amountToAdd", amountToAdd);
        //        request.AddParameter("partnerReferenceId", partnerRef);

        //        // execute the request
        //        var response = client.Execute<dynamic>(request);
        //        //var content = response.Content; // raw content as string

        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            result.RealBalance = response.Data["RealBalance"];
        //            result.BonusBalance = response.Data["BonusBalance"];

        //        }
        //        else
        //        {
        //            result.IsSuccess = false;
        //            result.Message = "Status code: " + response.StatusCode + ", Err: " + response.ErrorMessage;
        //        }
        //    }
        //    return result;
        //}


        public MemberBalanceResult GetBalance(string domain, string username, bool isProduction)
        {
            int? companyId = CompanyService.CompanyId(domain);
            Member member = null;
            using (var unitofwork = UnitOfWork.Current)
            {
                member = MemberRepository.ActiveMember(companyId.Value, username);
            }
            return GetMemberBalance(member);
        }
        public FinancialInfoResult GetFinancialInfo(string domain, string username, bool isProduction)
        {
            FinancialInfoResult result = new FinancialInfoResult() { MemberBalanceResult = new MemberBalanceResult() };
            try
            {
                int? companyId = CompanyService.CompanyId(domain);
                Member member = null;
                using (var unitofwork = UnitOfWork.Current)
                {
                    member = MemberRepository.ActiveMember(companyId.Value, username);
                }


                var partnerUrl = CompanyService.GetValue(member.CompanyId, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(member.CompanyId, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(member.CompanyId, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(member.CompanyId, "Voltron.APIPassword", isProduction);


                string vId = MemberDetails(member.Id, IntegrationVoltronId).FirstOrDefault().Value;


                var getBalanceUrl = "GetMemberFinancalInfo";

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                var request = new RestRequest(getBalanceUrl, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("memberId", vId); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, vId));
                // adds to POST or URL querystring based on Method
                //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                //var content = response.Content; // raw content as string

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        result.MemberBalanceResult.BonusBalance = response.Data["BalanceInfo"]["BonusBalance"];
                        result.MemberBalanceResult.WithdrawableBalance = response.Data["BalanceInfo"]["WithdrawableBalance"];
                        result.MemberBalanceResult.AllocatedBalance = response.Data["BalanceInfo"]["AllocatedBalance"];
                        result.MemberBalanceResult.TotalBalance = response.Data["BalanceInfo"]["TotalBalance"];
                        result.MemberBalanceResult.Currency = response.Data["BalanceInfo"]["Currency"];
                        result.PendingWithdrawCount = (int)response.Data["WithdrawInfo"];

                        if (response.Data["MemberBonus"] != null)
                        {
                            result.MemberBonus = new FinancialInfoMemberBonusResult();
                            result.MemberBonus.WageringAmount = Convert.ToInt64(response.Data["MemberBonus"]["WageringAmount"]);
                            result.MemberBonus.WageredAmount = Convert.ToInt64(response.Data["MemberBonus"]["WageredAmount"]);
                            result.MemberBonus.EndDate = response.Data["MemberBonus"]["EndingDate"] != null ? Convert.ToDateTime(response.Data["MemberBonus"]["EndingDate"]) : new Nullable<DateTime>();
                        }

                        result.IsSuccess = true;
                    }
                    catch (Exception ex) { }
                }
                else
                {
                    result.Message = "Status code: " + response.StatusCode + ", Err: " + response.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        //public BonusResult BonusList(string domain, string username, string bonusPlaceSystemName, bool isProduction)
        //{
        //    int? companyId = CompanyService.CompanyId(domain);
        //    var member = MemberRepository.ActiveMember(companyId.Value, username);
        //    return BonusList(member, bonusPlaceSystemName, isProduction);
        //}

        [Obsolete("Use GetMemberBalance instead as this one doesnt contain bonus balances.", false)]
        public BalanceResult GetBalance(Member member, bool? isProduction = null)
        {

            BalanceResult result = new BalanceResult();
            try
            {
                bool production = isProduction.HasValue
                    ? isProduction.Value
                    : Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]);
                var partnerUrl = CompanyService.GetValue(member.CompanyId, "Voltron.ServiceURL", production);
                var vCompanyId = CompanyService.GetValue(member.CompanyId, "Voltron.CompanyId", production);
                var apiUsername = CompanyService.GetValue(member.CompanyId, "Voltron.APIUsername", production);
                var apiPassword = CompanyService.GetValue(member.CompanyId, "Voltron.APIPassword", production);


                string vId = MemberDetails(member.Id, IntegrationVoltronId).FirstOrDefault().Value;


                var getBalanceUrl = "getbalance";

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                var request = new RestRequest(getBalanceUrl, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("memberId", vId); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, vId));
                // adds to POST or URL querystring based on Method
                //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                //var content = response.Content; // raw content as string

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        result.RealBalance = response.Data["RealBalance"];
                        result.BonusBalance = response.Data["BonusBalance"];
                        result.AllocatedBalance = response.Data["AllocatedBalance"];
                        result.TotalBalance = response.Data["TotalBalance"];
                        return result;
                    }
                    catch (Exception ex)
                    {
                        result.RealBalance = 0;
                        result.BonusBalance = 0;
                        result.AllocatedBalance = 0;
                        result.TotalBalance = 0;
                        return result;
                        throw ex;
                    }
                }
                else
                {
                    result.RealBalance = 0;
                    result.BonusBalance = 0;
                    result.AllocatedBalance = 0;
                    result.TotalBalance = 0;
                    result.IsSuccess = false;
                    result.Message = "Status code: " + response.StatusCode + ", Err: " + response.ErrorMessage;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.RealBalance = 0;
                result.BonusBalance = 0;
                result.AllocatedBalance = 0;
                result.TotalBalance = 0;
                result.IsSuccess = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public MemberBalanceResult GetMemberBalance(Member member, bool? isProduction = null)
        {

            MemberBalanceResult result = new MemberBalanceResult();
            try
            {

                bool production = isProduction.HasValue
                    ? isProduction.Value
                    : Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]);
                var partnerUrl = CompanyService.GetValue(member.CompanyId, "Voltron.ServiceURL", production);
                var vCompanyId = CompanyService.GetValue(member.CompanyId, "Voltron.CompanyId", production);
                var apiUsername = CompanyService.GetValue(member.CompanyId, "Voltron.APIUsername", production);
                var apiPassword = CompanyService.GetValue(member.CompanyId, "Voltron.APIPassword", production);


                string vId = MemberDetails(member.Id, IntegrationVoltronId).FirstOrDefault().Value;


                var getBalanceUrl = "GetMemberBalance";

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                var request = new RestRequest(getBalanceUrl, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("memberId", vId); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, vId));
                // adds to POST or URL querystring based on Method
                //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                //var content = response.Content; // raw content as string

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        result.BonusBalance = response.Data["CasinoBonusBalance"];
                        result.SportsBonusBalance = response.Data["SportsBonusBalance"];
                        result.WithdrawableBalance = response.Data["WithdrawableBalance"];
                        result.AllocatedBalance = response.Data["AllocatedBalance"];
                        result.TotalBalance = response.Data["TotalBalance"];
                        result.Currency = response.Data["Currency"];
                        return result;
                    }
                    catch (Exception ex)
                    {
                        result.BonusBalance = 0;
                        result.SportsBonusBalance = 0;
                        result.WithdrawableBalance = 0;
                        result.AllocatedBalance = 0;
                        result.TotalBalance = 0;
                        result.Currency = string.Empty;
                        return result;
                        throw ex;
                    }
                }
                else
                {
                    result.BonusBalance = 0;
                    result.SportsBonusBalance = 0;
                    result.WithdrawableBalance = 0;
                    result.AllocatedBalance = 0;
                    result.TotalBalance = 0;
                    result.Currency = string.Empty;
                    result.IsSuccess = false;
                    result.Message = "Status code: " + response.StatusCode + ", Err: " + response.ErrorMessage;
                    return result;
                }
            }
            catch (Exception ex)
            {

                result.BonusBalance = 0;
                result.SportsBonusBalance = 0;
                result.WithdrawableBalance = 0;
                result.TotalBalance = 0;
                result.IsSuccess = false;
                result.Message = ex.Message;
                return result;
            }
        }

        //public BonusResult BonusList(Member member, string bonusPlaceSystemName, bool? isProduction = null)
        //{
        //    BonusResult result = new BonusResult { IsSuccess = true, Message = "Generic_Error" };
        //    try
        //    {

        //        bool production = isProduction.HasValue
        //            ? isProduction.Value
        //            : Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]);
        //        var partnerUrl = CompanyService.GetValue(member.CompanyId, "Voltron.ServiceURL", production);
        //        var vCompanyId = CompanyService.GetValue(member.CompanyId, "Voltron.CompanyId", production);
        //        var apiUsername = CompanyService.GetValue(member.CompanyId, "Voltron.APIUsername", production);
        //        var apiPassword = CompanyService.GetValue(member.CompanyId, "Voltron.APIPassword", production);


        //        //string vId = MemberDetails(member.Id, IntegrationVoltronId).FirstOrDefault().Value;


        //        var getUrl = "BonusList";

        //        var client = new RestClient(partnerUrl);
        //        // client.Authenticator = new HttpBasicAuthenticator(username, password);

        //        var request = new RestRequest(getUrl, Method.GET);
        //        request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
        //        request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
        //        request.AddParameter("username", member.UnformattedUsername); // adds to POST or URL querystring based on Method
        //        request.AddParameter("bonusPlaceSystemName", bonusPlaceSystemName); // adds to POST or URL querystring based on Method
        //        request.AddParameter("checksum",
        //            SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, member.UnformattedUsername, bonusPlaceSystemName));
        //        // adds to POST or URL querystring based on Method
        //        //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

        //        //RestResponse<Person> response2 = client.Execute<Person>(request);
        //        //var name = response2.Data.Name;

        //        // execute the request
        //        var response = client.Execute<dynamic>(request);
        //        //var content = response.Content; // raw content as string

        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            try
        //            {
        //                //throw new Exception(response.Data["success"]);
        //                JObject dataObj = (JObject)JObject.FromObject(response.Data);
        //                result.IsSuccess = dataObj["success"].Value<int>() == 1;
        //                result.Message = ""; // no error 
        //                List<Bonus> data = new List<Bonus>();

        //                for (int i = 0; i < (dataObj["data"].Value<JArray>()).Count; i++)
        //                {
        //                    data.Add(new Bonus
        //                    {
        //                        Id = dataObj["data"][i]["Id"].Value<int>(),
        //                        Name = dataObj["data"][i]["Name"].Value<string>(),
        //                        Description = dataObj["data"][i]["Description"].Value<string>(),
        //                        TermsAndConditions = dataObj["data"][i]["TermsAndConditions"].Value<string>(),
        //                        StartDate = dataObj["data"][i]["StartDate"].Type != JTokenType.Null ? dataObj["data"][i]["StartDate"].Value<DateTime>() : new Nullable<DateTime>(),
        //                        EndDate = dataObj["data"][i]["EndDate"].Type != JTokenType.Null ? dataObj["data"][i]["EndDate"].Value<DateTime>() : new Nullable<DateTime>(),
        //                        CompleteAfterDays = dataObj["data"][i]["CompleteAfterDays"].Type != JTokenType.Null ? dataObj["data"][i]["CompleteAfterDays"].Value<int>() : new Nullable<int>(),
        //                        WageringContribution = dataObj["data"][i]["WageringContribution"].Value<int>(),
        //                        FixedAmount = dataObj["data"][i]["FixedAmount"].Type != JTokenType.Null ? (dataObj["data"][i]["FixedAmount"].Value<long>() / 100) : new Nullable<long>(),
        //                        BonusPercentage = dataObj["data"][i]["BonusPercentage"].Type != JTokenType.Null ? dataObj["data"][i]["BonusPercentage"].Value<decimal>() : new Nullable<decimal>(),
        //                        MinBonusAmount = dataObj["data"][i]["MinBonusAmount"].Type != JTokenType.Null ? (dataObj["data"][i]["MinBonusAmount"].Value<long>() / 100) : new Nullable<long>(),
        //                        MaxBonusAmount = dataObj["data"][i]["MaxBonusAmount"].Type != JTokenType.Null ? (dataObj["data"][i]["MaxBonusAmount"].Value<long>() / 100) : new Nullable<long>(),
        //                    });
        //                }

        //                result.Data = data;
        //                //throw new Exception(data.Count.ToString());
        //                //result.Id = response.Data["Id"];
        //                //result.Name = response.Data["Name"];
        //                //result.HeaderThumb = response.Data["HeaderThumb"];
        //                //result.WageredAmount = response.Data["WageredAmount"];
        //                //result.Amount = response.Data["Amount"];
        //                //result.WageringFactor = response.Data["WageringFactor"];
        //                //result.TotalWageringAmount = response.Data["TotalWageringAmount"];
        //                return result;
        //            }
        //            catch (Exception ex)
        //            {
        //                result.IsSuccess = false;
        //                result.Message = ex.Message;
        //                return result;
        //                //throw ex;
        //            }
        //        }
        //        else
        //        {
        //            result.IsSuccess = false;
        //            result.Message = "Status code: " + response.StatusCode + ", Err: " + response.ErrorMessage;
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccess = false;
        //        result.Message = ex.Message;
        //        return result;
        //    }
        //}


        public ActivatedBonusResult ActivatedBonusList(string domain, string username, bool isProduction)
        {
            int? companyId = CompanyService.CompanyId(domain);
            var member = MemberRepository.ActiveMember(companyId.Value, username);



            ActivatedBonusResult result = new ActivatedBonusResult { IsSuccess = true, Message = "Generic_Error" };
            try
            {
                var partnerUrl = CompanyService.GetValue(member.CompanyId, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(member.CompanyId, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(member.CompanyId, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(member.CompanyId, "Voltron.APIPassword", isProduction);


                //string vId = MemberDetails(member.Id, IntegrationVoltronId).FirstOrDefault().Value;


                var getUrl = "ActivatedBonusList";

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                var request = new RestRequest(getUrl, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", member.UnformattedUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, member.UnformattedUsername));
                // adds to POST or URL querystring based on Method
                //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                //var content = response.Content; // raw content as string

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        //throw new Exception(response.Data["success"]);
                        JObject dataObj = (JObject)JObject.FromObject(response.Data);
                        result.IsSuccess = dataObj["success"].Value<int>() == 1;
                        result.Message = ""; // no error 
                        List<MemberBonus> data = new List<MemberBonus>();

                        for (int i = 0; i < (dataObj["data"].Value<JArray>()).Count; i++)
                        {
                            data.Add(new MemberBonus
                            {
                                Id = dataObj["data"][i]["Id"].Value<int>(),
                                BonusId = dataObj["data"][i]["BonusId"].Value<int>(),
                                BonusStatusType = dataObj["data"][i]["BonusStatusType"].Value<int>(),
                                BonusAmount = dataObj["data"][i]["BonusAmount"].Value<long>(),
                                RemainBonusAmount = dataObj["data"][i]["RemainBonusAmount"].Value<long>(),
                                AllocatedAmount = dataObj["data"][i]["AllocatedAmount"].Value<long>(),
                                WageringAmount = dataObj["data"][i]["WageringAmount"].Value<long>(),
                                WageredAmount = dataObj["data"][i]["WageredAmount"].Value<long>(),
                                CreateDate = dataObj["data"][i]["CreateDate"].Value<DateTime>(),
                                WageredDate = dataObj["data"][i]["WageredDate"].Type != JTokenType.Null ? dataObj["data"][i]["WageredDate"].Value<DateTime>() : new Nullable<DateTime>(),
                                RelatedVoltronTransactionId = dataObj["data"][i]["RelatedVoltronTransactionId"].Type != JTokenType.Null ? dataObj["data"][i]["RelatedVoltronTransactionId"].Value<long>() : new Nullable<long>(),
                                EndingDate = dataObj["data"][i]["EndingDate"].Type != JTokenType.Null ? dataObj["data"][i]["EndingDate"].Value<DateTime>() : new Nullable<DateTime>()
                            });
                        }

                        result.Data = data;
                        //throw new Exception(data.Count.ToString());
                        //result.Id = response.Data["Id"];
                        //result.Name = response.Data["Name"];
                        //result.HeaderThumb = response.Data["HeaderThumb"];
                        //result.WageredAmount = response.Data["WageredAmount"];
                        //result.Amount = response.Data["Amount"];
                        //result.WageringFactor = response.Data["WageringFactor"];
                        //result.TotalWageringAmount = response.Data["TotalWageringAmount"];
                        return result;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = ex.Message;
                        return result;
                        //throw ex;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "Status code: " + response.StatusCode + ", Err: " + response.ErrorMessage;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public BonusResult BonusEngineBonusList(string domain, string username, bool isProduction)
        {
            int? companyId = CompanyService.CompanyId(domain);
            var member = MemberRepository.ActiveMember(companyId.Value, username);



            BonusResult result = new BonusResult { IsSuccess = true, Message = "Generic_Error" };
            try
            {
                var partnerUrl = CompanyService.GetValue(member.CompanyId, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(member.CompanyId, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(member.CompanyId, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(member.CompanyId, "Voltron.APIPassword", isProduction);


                //string vId = MemberDetails(member.Id, IntegrationVoltronId).FirstOrDefault().Value;


                var getUrl = "BonusListAsync";

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                var request = new RestRequest(getUrl, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", member.UnformattedUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("requestedFrom", "web"); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, member.UnformattedUsername));
                // adds to POST or URL querystring based on Method
                //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                //var content = response.Content; // raw content as string

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        //throw new Exception(response.Data["success"]);
                        JObject dataObj = (JObject)JObject.FromObject(response.Data);
                        result.IsSuccess = dataObj["success"].Value<int>() == 1;
                        result.Message = ""; // no error 

                        List<Bonus> data = new List<Bonus>();

                        for (int i = 0; i < (dataObj["data"].Value<JArray>()).Count; i++)
                        {
                            data.Add(new Bonus
                            {
                                Id = dataObj["data"][i]["Id"].Value<int>(),
                                Name = dataObj["data"][i]["Name"].Value<string>(),
                                Description = dataObj["data"][i]["Description"].Value<string>(),
                                TermsAndConditions = dataObj["data"][i]["TermsAndConditions"].Value<string>(),
                                ThumbnailImageURL = dataObj["data"][i]["ThumbnailImageURL"].Value<string>(),
                                StartDate = dataObj["data"][i]["StartDate"].Type != JTokenType.Null ? dataObj["data"][i]["StartDate"].Value<DateTime>() : new Nullable<DateTime>(),
                                EndDate = dataObj["data"][i]["EndDate"].Type != JTokenType.Null ? dataObj["data"][i]["EndDate"].Value<DateTime>() : new Nullable<DateTime>(),
                                CompleteAfterDays = dataObj["data"][i]["CompleteAfterDays"].Type != JTokenType.Null ? dataObj["data"][i]["CompleteAfterDays"].Value<int>() : new Nullable<int>(),
                                WageringContribution = dataObj["data"][i]["WageringContribution"].Value<int>(),
                                FixedAmount = dataObj["data"][i]["FixedAmount"].Type != JTokenType.Null ? (dataObj["data"][i]["FixedAmount"].Value<decimal>() / 100) : new Nullable<decimal>(),
                                BonusPercentage = dataObj["data"][i]["BonusPercentage"].Type != JTokenType.Null ? dataObj["data"][i]["BonusPercentage"].Value<decimal>() : new Nullable<decimal>(),
                                MinBonusAmount = dataObj["data"][i]["MinBonusAmount"].Type != JTokenType.Null ? (dataObj["data"][i]["MinBonusAmount"].Value<long>() / 100) : new Nullable<long>(),
                                MaxBonusAmount = dataObj["data"][i]["MaxBonusAmount"].Type != JTokenType.Null ? (dataObj["data"][i]["MaxBonusAmount"].Value<long>() / 100) : new Nullable<long>(),
                                EligibleAfterType = dataObj["data"][i]["EligibleAfterType"].Value<int>(),
                                EligibleAfterEntityIdList = dataObj["data"][i]["EligibleAfterEntityIdList"].Value<string>(),
                                PossibleVoltronTransactionId = dataObj["data"][i]["PossibleVoltronTransactionId"].Value<long>()
                            });
                        }

                        result.Data = data;

                        return result;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = ex.Message;
                        return result;
                        //throw ex;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "Status code: " + response.StatusCode + ", Err: " + response.ErrorMessage;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                return result;
            }
        }
        public ResultModel ActivateBonus(int companyId, string username, bool isProduction, int bonusId, long? relatedVoltronTransactionId, long? amount, long? allocatedAmount, bool? checkEligibility)
        {
            ResultModel result = new ResultModel { IsSuccess = true, Message = "Generic_Error" };
            try
            {
                var partnerUrl = CompanyService.GetValue(companyId, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(companyId, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(companyId, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(companyId, "Voltron.APIPassword", isProduction);


                //string vId = MemberDetails(member.Id, IntegrationVoltronId).FirstOrDefault().Value;


                var getUrl = "ApplyBonus";

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                var request = new RestRequest(getUrl, Method.POST);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", username); // adds to POST or URL querystring based on Method
                request.AddParameter("bonusId", bonusId); // adds to POST or URL querystring based on Method
                if (amount.HasValue)
                    request.AddParameter("bonusAmount", amount * 100); // adds to POST or URL querystring based on Method
                if (allocatedAmount.HasValue)
                    request.AddParameter("allocatedAmount", allocatedAmount * 100); // adds to POST or URL querystring based on Method
                request.AddParameter("relatedVoltronTransactionId", relatedVoltronTransactionId); // adds to POST or URL querystring based on Method
                request.AddParameter("checkEligibility", checkEligibility); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username, bonusId.ToString(),
                    (relatedVoltronTransactionId.HasValue ? relatedVoltronTransactionId.Value.ToString() : string.Empty),
                    (amount.HasValue ? amount.Value.ToString() : string.Empty)
                    ));
                // adds to POST or URL querystring based on Method
                //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                //var content = response.Content; // raw content as string

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        //throw new Exception(response.Data["success"]);
                        JObject dataObj = (JObject)JObject.FromObject(response.Data);
                        result.IsSuccess = dataObj["Success"].Value<int>() == 1;
                        result.Message = result.IsSuccess ? string.Empty : dataObj["Message"].Value<string>();

                        return result;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = ex.Message;
                        return result;
                        //throw ex;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "Status code: " + response.StatusCode + ", Err: " + response.ErrorMessage;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                return result;
            }
        }
        public Core.Contracts.GenericResult ApplyFreeSpinBonusEngine(string domain, string username, int numberOfFreeRound, int bonusId, int spinValue, int voltronGameId, bool isProduction)
        {
            Core.Contracts.GenericResult result = new Core.Contracts.GenericResult { IsSuccess = false };

            int? comapanyId = CompanyService.CompanyId(domain);
            if (comapanyId.HasValue)
            {
                var partnerUrl = CompanyService.GetValue(comapanyId.Value, "Voltron.ServiceURL", isProduction);
                var voltronCompanyId = CompanyService.GetValue(comapanyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(comapanyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(comapanyId.Value, "Voltron.APIPassword", isProduction);

                var member = MemberRepository.Member(comapanyId.Value, username);


                string partnerLoginUrl = (partnerUrl + "ApplyFreeSpinBonusEngineAsync/?username=" + username +
                                          "&apiUsername=" + apiUsername + "&companyId=" +
                                          voltronCompanyId + "&freeRound=" + numberOfFreeRound + "&gameId=" + voltronGameId + "&bonusId=" + bonusId + "&spinValue=" + spinValue + "&jurisdictionCode=&checksum=" +
                                          SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, voltronCompanyId,
                                              apiUsername, username, voltronGameId.ToString(), bonusId.ToString(), numberOfFreeRound.ToString(), spinValue.ToString(), string.Empty).ToUpper()//jurisdictionCode=string.Empty
                                              );

                var response = HttpServiceHelper.GetJsonRequest((partnerLoginUrl));

                if (response.status != null && Convert.ToBoolean(response.status) || response.status == null)
                {
                    if (Convert.ToBoolean(response.Success))
                    {
                        result.IsSuccess = true;
                    }
                    result.Message = JsonConvert.SerializeObject(response) + "," + response.Message;
                }
                else
                {
                    result.Message = response.Message;
                }
            }
            else
            {
                result.Message = "Generic error";
            }

            return result;
        }
        public ResultModel ForfeitBonus(string domain, string username, bool isProduction, int memberBonusId)
        {
            ResultModel result = new ResultModel { IsSuccess = true, Message = "Generic_Error" };
            try
            {
                int? companyId = CompanyService.CompanyId(domain);
                var member = MemberRepository.ActiveMember(companyId.Value, username);

                var partnerUrl = CompanyService.GetValue(member.CompanyId, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(member.CompanyId, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(member.CompanyId, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(member.CompanyId, "Voltron.APIPassword", isProduction);


                //string vId = MemberDetails(member.Id, IntegrationVoltronId).FirstOrDefault().Value;


                var getUrl = "ForfeitBonus";

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                var request = new RestRequest(getUrl, Method.POST);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", member.UnformattedUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("memberBonusId", memberBonusId); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, member.UnformattedUsername, memberBonusId.ToString()
                    ));
                // adds to POST or URL querystring based on Method
                //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                //var content = response.Content; // raw content as string

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        //throw new Exception(response.Data["success"]);
                        JObject dataObj = (JObject)JObject.FromObject(response.Data);
                        result.IsSuccess = dataObj["Success"].Value<int>() == 1;
                        result.Message = dataObj["Message"].Value<string>(); // no error 

                        return result;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = ex.Message;
                        return result;
                        //throw ex;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "Status code: " + response.StatusCode + ", Err: " + response.ErrorMessage;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                return result;
            }
        }


        public ResultModel Login(string domain, string language, string emailOrUsername, string password, string ip, string userAgent, bool isProduction, string hash = null)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                ResultModel result = new ResultModel();
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    Member member = null;

                    int? companyId = CompanyService.CompanyId(domain);
                    if (!companyId.HasValue)
                        result.Message = LocalizationHelper.Value(language, "Generic", "SiteDomain.NotMatched");
                    else
                        member = MemberRepository.ActiveMember(companyId.Value, emailOrUsername);


                    if (member == null)
                    {
                        result.Message = LocalizationHelper.Value(language, "Login", "Login.EmailNotActive");
                    }
                    else if (domain.Contains("mavivip") && member.Level.SystemName == "regular")
                    {
                        result.Message = LocalizationHelper.Value(language, "Login", "Login.PasswordNotMatched");
                    }
                    else if (member.Password != SecurityHelper.MD5Encryption(password))
                    {
                        result.Message = LocalizationHelper.Value(language, "Login", "Login.PasswordNotMatched");
                    }
                    else
                    {
                        result.IsSuccess = true;

                        if (companyId.Value == 1 && string.IsNullOrEmpty(hash) && isProduction)
                        {
                            result.IsSuccess = false;
                            result.Message = LocalizationHelper.Value(language, "Login", "Login.MemberDeviceFingerPrintWebhook");
                        }
                        else if (companyId.Value == 1 && isProduction)
                        {
                            IPLoginLogRepository.Insert(new IPLoginLog() { IP = ip, CreateDate = DateTime.UtcNow, MemberId = member.Id });
                            var groupedIpList = IPLoginLogRepository.GetAll().Where(i => i.IP == ip && i.CreateDate > DateTime.UtcNow.AddHours(-2)).GroupBy(i => i.MemberId).ToList();
                            int differentMemberIpCount = groupedIpList.Count;

                            if (differentMemberIpCount > 5)
                            {
                                result.Message = LocalizationHelper.Value(language, "Login", "Login.PasswordNotMatched");
                                result.IsSuccess = false;
                            }



                            bool is2FAEnabled = Is2FAEnabledByMemberId(companyId.Value, member.Id);
                            int count = MemberDeviceFingerPrintRepository.GetCountHashByMemberId(member.Id);

                            int memberDeviceStatusType = count > HASH_COUNT ? (int)Core.Enum.MemberDeviceFingerPrintStatusType.Force2FA : (is2FAEnabled ? (int)Core.Enum.MemberDeviceFingerPrintStatusType.Neutral : (int)NW.Core.Enum.MemberDeviceFingerPrintStatusType.Whitelisted);

                            MemberDeviceFingerPrintRepository.InsertOrUpdate(member.Id, ip, null, hash, memberDeviceStatusType);//sso

                            DeviceFingerPrintWebhook memberDeviceFingerPrintWebhook = MemberDeviceFingerPrintWebhookRepository.GetAll().FirstOrDefault(mdfpw => mdfpw.Hash == hash && mdfpw.UpdateDate > DateTime.UtcNow.AddSeconds(-120));
                            //DeviceFingerPrintWebhook memberDeviceFingerPrintWebhook = MemberDeviceFingerPrintWebhookRepository.GetAll().FirstOrDefault(mdfpw => mdfpw.Hash == hash && mdfpw.BotProbability == false && mdfpw.UpdateDate > DateTime.UtcNow.AddSeconds(-120));
                            if (memberDeviceFingerPrintWebhook == null)// || memberDeviceFingerPrintWebhook.IP != ip fingerprint doesn't support ipv6 
                            {
                                result.Message = LocalizationHelper.Value(language, "Login", "Login.MemberDeviceFingerPrintWebhook") + "ErrorCode: " + hash;
                                result.IsSuccess = false;
                            }
                            else if (IPBlacklistRepository.GetAll().Any(ib => ib.IP == ip && ib.BlockTo > DateTime.UtcNow))
                            {
                                result.Message = LocalizationHelper.Value(language, "Login", "Login.MemberDeviceFingerPrintWebhook");
                                result.IsSuccess = false;
                            }
                            else
                            {
                                result.IsSuccess = true;
                            }

                            //device hash check
                            int statusType = DeviceFingerPrintRepository.GetStatusTypeByHash(hash);
                            switch (statusType)
                            {
                                case (int)StatusType.Active:
                                    break;
                                case (int)StatusType.Passive:
                                    int countMemberId = MemberDeviceFingerPrintRepository.GetCountMemberIdByHash(hash);
                                    if (countMemberId >= 4)
                                    {
                                        result.Message = LocalizationHelper.Value(language, "Login", "Login.PasswordNotMatched2");
                                        result.IsSuccess = false;
                                        //todo sent it kafka on controller
                                        LoginFailedKafkaModel loginFailedKafkaModel = new LoginFailedKafkaModel() { CompanyId = 6, MemberId = member.Id, UnformattedUsername = member.UnformattedUsername, Url = domain, ErrorMessage = string.Format("{0} hash was attemptted {1} different users for login", hash, countMemberId) };
                                        Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "LoginFailed", 0, JsonConvert.SerializeObject(loginFailedKafkaModel)); });

                                    }
                                    break;
                                case (int)StatusType.Deleted:
                                    result.Message = LocalizationHelper.Value(language, "Login", "Login.PasswordNotMatched2");
                                    result.IsSuccess = false;
                                    break;
                                default:
                                    break;
                            }










                            //it should be end of method to get if result is success
                            string message = "LOGIN: Username: " + member.Username + " - IP: " + ip + " - DifferentMemberIpCount: " + differentMemberIpCount + " - UserAgent: " + userAgent + " - Hash: " + hash + " - IsSuccess: " + result.IsSuccess + " - Message: " + result.Message + " - MemberDeviceStatusType: " + ((MemberDeviceFingerPrintStatusType)memberDeviceStatusType).ToString() + " - GlobalDeviceFingerPrintStatus: " + statusType;
                            LoginNotifierHelper.Notifier(member.Id, hash, DateTime.UtcNow, message);
                            MemberDeviceFingerPrintRepository.InsertOrUpdate(member.Id, ip, result.IsSuccess, hash, memberDeviceStatusType);//sso

                        }

                        if (result.IsSuccess)
                        {
                            MemberDetailRepository.InsertOrUpdate(member.Id, InteractionLoginDateKey,
                                DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss"));

                            result.Model = member.Username;

                        }

                        uniOfWork.Commit(transaction);
                    }

                    if (result.IsSuccess)
                        ServiceBusHelper.InsertQueue(isProduction, "loginsucceeded", JsonConvert.SerializeObject(new { username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, hash = hash, timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));
                    else
                        ServiceBusHelper.InsertQueue(isProduction, "loginfailed", JsonConvert.SerializeObject(new { username = member != null ? member.Username : emailOrUsername, memberId = member != null ? member.Id : 0, memberUniqueId = member != null ? member.UniqueId : string.Empty, hash = hash, timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = result.Message }));

                }

                return result;
            }
        }
        public ResultModel ValidateUsername(string domain, string language, string username)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {


                ResultModel result = new ResultModel();
                int? comapanyId = CompanyService.CompanyId(domain);
                if (!comapanyId.HasValue)
                {
                    result.Message = LocalizationHelper.Value(language, "Generic", "SiteDomain.NotMatched");
                }
                else if (!MemberRepository.ValidateStep1RegisterUsername(comapanyId.Value, username))
                {
                    result.Message = LocalizationHelper.Value(language, "Validation", "RegisterStep1Model.UsedUsername");
                }
                else
                {
                    result.IsSuccess = true;
                }

                return result;
            }
        }

        public ResultModel ValidateEmail(string domain, string language, string email)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {


                ResultModel result = new ResultModel();
                int? comapanyId = CompanyService.CompanyId(domain);
                if (!comapanyId.HasValue)
                {
                    result.Message = LocalizationHelper.Value(language, "Generic", "SiteDomain.NotMatched");
                }
                else if (!MemberRepository.ValidateStep1RegisterEmail(comapanyId.Value, email))
                {
                    result.Message = LocalizationHelper.Value(language, "Validation", "RegisterStep1Model.UsedEmail");
                }
                else
                {
                    result.IsSuccess = true;
                }

                return result;
            }
        }

        public Dictionary<string, string> MemberDetails(string domain, string username, params string[] keys)
        {
            Member member = GetActiveMember(domain, username);
            return MemberDetails(member.Id, keys);
        }

        public Dictionary<string, string> MemberDetails(int memberId, params string[] keys)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                uniOfWork.BeginTransaction(Session, IsolationLevel.ReadUncommitted);
                Dictionary<string, string> details = MemberDetailRepository.GetAll()
                    .Where(md => md.MemberId == memberId && keys.Contains(md.Key))
                    .ToList()
                    .ToDictionary(md => md.Key, md => md.Value);

                return keys.ToDictionary(k => k, k => details.ContainsKey(k) ? details[k] : string.Empty);
            }
        }

        public ResultModel UpdateMemberDetails(string domain, string username, Dictionary<string, string> memberDetails)
        {
            Member member = GetActiveMember(domain, username);
            return UpdateMemberDetails(member.Id, memberDetails);

        }

        public ResultModel UpdateMemberDetails(int memberId, Dictionary<string, string> memberDetails)
        {
            ResultModel model = new ResultModel() { IsSuccess = true };
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {

                    foreach (var kvp in memberDetails)
                    {
                        MemberDetailRepository.InsertOrUpdate(memberId, kvp.Key, kvp.Value);
                    }

                    uniOfWork.Commit(transaction);

                }
            }
            return model;
        }



        public IList<int> GetGameHistory(string domain, string username)
        {
            IList<int> gameHistoryIds = new List<int>();
            Member member = GetActiveMember(domain, username);
            if (member != null)
            {
                var gameList =
                    MemberGameHistoryRepository.GetAll()
                        .Where(mgh => mgh.MemberId == member.Id)
                        .OrderByDescending(mgh => mgh.CreateDate)
                        .Take(20).ToList();

                foreach (var game in gameList)
                {
                    if (!gameHistoryIds.Contains(game.GameId))
                    {
                        gameHistoryIds.Add(game.GameId);
                    }
                }

            }
            return gameHistoryIds;
        }


        public PagingModel<Transaction> Transactions(string domain, int pageIndex, int pageSize, int transactionTypeId, DateTime startDate, DateTime endDate, string username, bool isProduction)
        {
            PagingModel<Transaction> pagingModel = new PagingModel<Transaction>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {

                    int? companyId = CompanyService.CompanyId(domain);

                    var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                    //var partnerUrl = "http://www-local.nwservmodule.com/api/partner/";
                    var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                    var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                    var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                    var method = "TransactionHistory";

                    var client = new RestClient(partnerUrl);
                    // client.Authenticator = new HttpBasicAuthenticator(username, password);

                    var request = new RestRequest(method, Method.GET);
                    request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                    request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                    request.AddParameter("username", username); // adds to POST or URL querystring based on Method
                    request.AddParameter("pageIndex", pageIndex); // adds to POST or URL querystring based on Method
                    request.AddParameter("pageSize", pageSize); // adds to POST or URL querystring based on Method
                    request.AddParameter("transactionTypeId", transactionTypeId);
                    request.AddParameter("startDate", startDate.ToString("yyyy-MM-dd"));
                    request.AddParameter("endDate", endDate.ToString("yyyy-MM-dd"));
                    request.AddParameter("checksum", SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username, pageIndex.ToString(), pageSize.ToString(), transactionTypeId.ToString(), startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"))); // adds to POST or URL querystring based on Method
                    // adds to POST or URL querystring based on Method
                    //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                    //RestResponse<Person> response2 = client.Execute<Person>(request);
                    //var name = response2.Data.Name;

                    // execute the requestx
                    var response = client.Execute<dynamic>(request);
                    //var content = response.Content; // raw content as string

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        foreach (var data in response.Data["data"]["TransactionList"])
                        {
                            Transaction t = new Transaction
                            {
                                Amount = Convert.ToDecimal(data["Amount"] * 0.01m),
                                TransactionTypeId = data["TransactionTypeId"],
                                BonusBalanceAfter = Convert.ToDecimal(data["BonusBalanceAfter"] * 0.01m),
                                Date = Convert.ToDateTime(data["CreateDate"]),
                                Description = data["Description"],
                                Id = data["Id"],
                                TotalBalanceAfter = Convert.ToDecimal(data["TotalBalanceAfter"] * 0.01m),
                                ProviderId = Convert.ToInt32(data["ProviderId"])
                            };

                            if (transactionTypeId == 1)
                            {
                                t.FinancialStatus = data["WithdrawStatus"] != null && data["WithdrawStatus"] != string.Empty ? string.Format("WithdrawStatusType.{0}", data["WithdrawStatus"]) : string.Empty;
                            }


                            result.Add(t);
                        }
                        pagingModel.TotalCount = Convert.ToInt64(response.Data["data"]["TotalCount"]);
                        pagingModel.ItemList = result;
                    }

                }

                return pagingModel;
                // else
                //{
                //result.IsSuccess = false;
                //result.Message = "Status code: " + response.StatusCode + ", Err: " + response.ErrorMessage;
                //return result;
                //}
            }
        }


        public PagingModel<MemberSummaryReport> AffiliateMemberSummaryReport(string domain, int pageIndex, int pageSize, string username, bool isProduction)
        {
            PagingModel<MemberSummaryReport> pagingModel = new PagingModel<MemberSummaryReport>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<MemberSummaryReport> result = new List<MemberSummaryReport>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {

                    int? companyId = CompanyService.CompanyId(domain);

                    var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                    var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                    var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                    var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                    var method = "AffiliateMemberSummaryReport";

                    var client = new RestClient(partnerUrl);
                    // client.Authenticator = new HttpBasicAuthenticator(username, password);

                    var request = new RestRequest(method, Method.GET);
                    request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                    request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                    request.AddParameter("username", username); // adds to POST or URL querystring based on Method
                    request.AddParameter("pageIndex", pageIndex); // adds to POST or URL querystring based on Method
                    request.AddParameter("pageSize", pageSize); // adds to POST or URL querystring based on Method
                    request.AddParameter("checksum", SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username, pageIndex.ToString(), pageSize.ToString())); // adds to POST or URL querystring based on Method
                    // adds to POST or URL querystring based on Method
                    //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                    //RestResponse<Person> response2 = client.Execute<Person>(request);
                    //var name = response2.Data.Name;

                    // execute the requestx
                    var response = client.Execute<dynamic>(request);
                    //var content = response.Content; // raw content as string

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        foreach (var data in response.Data["data"]["ReportList"])
                        {
                            MemberSummaryReport r = new MemberSummaryReport
                            {
                                Id = Convert.ToInt32(data["Id"]),
                                Username = data["Username"],
                                FullName = data["FullName"],
                                RegisterDate = Convert.ToDateTime(data["RegisterDate"]),

                                FromDate = Convert.ToDateTime(data["FromDate"]),
                                ToDate = Convert.ToDateTime(data["ToDate"]),

                                TotalCasinoBet = Convert.ToDecimal(data["TotalCasinoBet"] * 0.01m),
                                TotalCasinoWin = Convert.ToDecimal(data["TotalCasinoWin"] * 0.01m),
                                TotalCasinoCancel = Convert.ToDecimal(data["TotalCasinoCancel"] * 0.01m),

                                TotalSportsBet = Convert.ToDecimal(data["TotalSportsBet"] * 0.01m),
                                TotalSportsWin = Convert.ToDecimal(data["TotalSportsWin"] * 0.01m),
                                TotalSportsCancel = Convert.ToDecimal(data["TotalSportsCancel"] * 0.01m),

                                StartBalance = Convert.ToDecimal(data["StartBalance"] * 0.01m),
                                EndBalance = Convert.ToDecimal(data["EndBalance"] * 0.01m),

                                NoOfDeposit = Convert.ToInt32(data["NoOfDeposit"]),
                                TotalDeposit = Convert.ToDecimal(data["TotalDeposit"] * 0.01m),

                                NoOfBonus = Convert.ToInt32(data["NoOfBonus"]),
                                TotalBonus = Convert.ToDecimal(data["TotalBonus"] * 0.01m),

                                NoOfCredit = Convert.ToInt32(data["NoOfCredit"]),
                                TotalCredit = Convert.ToDecimal(data["TotalCredit"] * 0.01m),
                                TotalCreditPaidCash = Convert.ToDecimal(data["TotalCreditPaidCash"] * 0.01m),
                                NoOfCreditPaidCash = Convert.ToInt32(data["NoOfCreditPaidCash"]),

                                TotalCreditBack = Convert.ToDecimal(data["TotalCreditBack"] * 0.01m),
                                NoOfCreditBack = Convert.ToInt32(data["NoOfCreditBack"]),

                                TotalCreditDiskont = Convert.ToDecimal(data["TotalCreditDiskont"] * 0.01m),
                                NoOfCreditDiskont = Convert.ToInt32(data["NoOfCreditDiskont"]),


                                TotalNegativeAdjustment = Convert.ToDecimal(data["TotalNegativeAdjustment"] * 0.01m),
                                NoOfCashback = Convert.ToInt32(data["NoOfCashback"]),
                                TotalCashback = Convert.ToDecimal(data["TotalCashback"] * 0.01m),

                                TotalWithdraw = Convert.ToDecimal(data["TotalWithdraw"] * 0.01m),

                                CurrentBalance = Convert.ToDecimal(data["CurrentBalance"] * 0.01m)
                            };


                            result.Add(r);
                        }
                        pagingModel.TotalCount = Convert.ToInt64(response.Data["data"]["TotalCount"]);
                        pagingModel.ItemList = result;
                    }

                }

                return pagingModel;
                // else
                //{
                //result.IsSuccess = false;
                //result.Message = "Status code: " + response.StatusCode + ", Err: " + response.ErrorMessage;
                //return result;
                //}
            }
        }

        public PagingModel<AffiliateSummaryReport> AffiliateSummaryReport(string domain, int pageIndex, int pageSize, string username, bool isProduction, DateTime startDate, DateTime endDate)
        {
            PagingModel<AffiliateSummaryReport> pagingModel = new PagingModel<AffiliateSummaryReport>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<AffiliateSummaryReport> result = new List<AffiliateSummaryReport>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {

                    int? companyId = CompanyService.CompanyId(domain);

                    var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                    var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                    var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                    var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                    var method = "AffiliateSummaryReport";

                    var client = new RestClient(partnerUrl);
                    // client.Authenticator = new HttpBasicAuthenticator(username, password);

                    var request = new RestRequest(method, Method.GET);
                    request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                    request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                    request.AddParameter("username", username); // adds to POST or URL querystring based on Method
                    request.AddParameter("pageIndex", pageIndex); // adds to POST or URL querystring based on Method
                    request.AddParameter("pageSize", pageSize); // adds to POST or URL querystring based on Method
                    request.AddParameter("startDate", startDate.ToString("yyyy-MM-dd"));
                    request.AddParameter("endDate", endDate.ToString("yyyy-MM-dd"));
                    request.AddParameter("checksum", SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username, pageIndex.ToString(), pageSize.ToString(), startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"))); // adds to POST or URL querystring based on Method
                    // adds to POST or URL querystring based on Method
                    //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                    //RestResponse<Person> response2 = client.Execute<Person>(request);
                    //var name = response2.Data.Name;

                    // execute the requestx
                    var response = client.Execute<dynamic>(request);
                    //var content = response.Content; // raw content as string

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (response.Data != null && response.Data["data"] != null)
                        {
                            foreach (var data in response.Data["data"]["ReportList"])
                            {
                                AffiliateSummaryReport r = new AffiliateSummaryReport
                                {
                                    ReportDate = Convert.ToDateTime(data["ReportDate"]),
                                    RegisteredCount = Convert.ToInt32(data["RegisteredCount"]),
                                    DepositCount = Convert.ToInt32(data["DepositCount"]),
                                    TotalDeposit = Convert.ToDecimal(data["TotalDeposit"]) * 0.01m,
                                    TotalBet = Convert.ToDecimal(data["TotalBet"]) * 0.01m * -1,
                                    TotalWin = Convert.ToDecimal(data["TotalWin"]) * 0.01m,
                                    TotalCredit = Convert.ToDecimal(data["TotalCredit"]) * 0.01m,
                                    TotalBonus = Convert.ToDecimal(data["TotalBonus"]) * 0.01m,
                                    TotalCashback = Convert.ToDecimal(data["TotalCashback"]) * 0.01m,
                                    TotalWithdraw = Convert.ToDecimal(data["TotalWithdraw"]) * 0.01m * -1,
                                    TotalWithdrawCancelled = Convert.ToDecimal(data["TotalWithdrawCancelled"]) * 0.01m,
                                    NetGameRevenue = Convert.ToDecimal(data["NetGameRevenue"]) * 0.01m,
                                    Income = Convert.ToDecimal(data["Income"]) * 0.01m
                                };
                                result.Add(r);
                            }
                            pagingModel.TotalCount = Convert.ToInt64(response.Data["data"]["TotalCount"]);
                            pagingModel.ItemList = result;
                        }
                    }

                }

                return pagingModel;
                // else
                //{
                //result.IsSuccess = false;
                //result.Message = "Status code: " + response.StatusCode + ", Err: " + response.ErrorMessage;
                //return result;
                //}
            }
        }
        private string GeneratePhoneVerificationCode()
        {
            Random random = new Random();
            return random.Next(1000, 9999).ToString();
        }

        public Core.Contracts.GenericResult ValidatePhoneWithCode(int memberId, string code)
        {
            Core.Contracts.GenericResult result = new Core.Contracts.GenericResult { IsSuccess = false, Message = "GenericError", ResponseCode = -1 };

            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {

                    try
                    {
                        var existingMpv = MemberPhoneVerificationRepository.GetAll().SingleOrDefault(w => w.MemberId == memberId);
                        if (existingMpv != null)
                        {
                            bool codeValid = existingMpv.Code == code.Trim();
                            if (codeValid)
                            {

                                bool verifiedBefore = MemberPhoneVerificationRepository.GetAll().Any(w => w.Phone == existingMpv.Phone && w.Verified);
                                if (verifiedBefore)
                                {
                                    // daha once verify edilmis telefon baska bir member
                                    result.IsSuccess = false;
                                    result.Message = "PHONE_VERIFIED_BYSOMEONEELSE_CONTACT_LIVECHAT";
                                    result.ResponseCode = 0;
                                }
                                else
                                {
                                    existingMpv.Verified = true;
                                    existingMpv.VerifyDate = DateTime.UtcNow;

                                    MemberPhoneVerificationRepository.Update(existingMpv);
                                    uniOfWork.Commit(transaction);

                                    result.IsSuccess = true;
                                    result.Message = "";
                                    result.ResponseCode = 0;
                                }
                            }
                            else
                            {
                                // code is not valid, not same as what is generated for the member.
                                result.Message = "CODE_IS_NOT_VALID";
                            }
                        }
                        else
                        {
                            // code doesnt exist for this member, generic error would be fine for now.
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return result;
        }

        public bool PhoneVerified(int memberId)
        {
            //GenericResult result = new GenericResult {IsSuccess = false, Message = "GenericError", ResponseCode = -1};

            //var member = MemberRepository.Get(memberId);
            Dictionary<string, string> memberDetails = MemberDetails(memberId, VerificationPhoneKey);
            var phoneVerified = memberDetails[VerificationPhoneKey];

            return phoneVerified == "True";
        }
        public bool TelegramVerified(int memberId)
        {
            //GenericResult result = new GenericResult {IsSuccess = false, Message = "GenericError", ResponseCode = -1};

            //var member = MemberRepository.Get(memberId);
            Dictionary<string, string> memberDetails = MemberDetails(memberId, VerificationTelegramKey);
            var telegramVerified = memberDetails[VerificationTelegramKey];

            return telegramVerified == "True";
        }
        public bool EmailVerified(int memberId)
        {
            //GenericResult result = new GenericResult {IsSuccess = false, Message = "GenericError", ResponseCode = -1};

            //var member = MemberRepository.Get(memberId);
            Dictionary<string, string> memberDetails = MemberDetails(memberId, VerificationEmailKey);
            var phoneVerified = memberDetails[VerificationEmailKey];

            return phoneVerified == "True";
        }



        public Core.Contracts.GenericResult SendValidatePhoneCode(int memberId, string phoneNumberWithCountryCode)
        {
            Core.Contracts.GenericResult result = new Core.Contracts.GenericResult { IsSuccess = false, Message = "GenericError", ResponseCode = -1 };

            string code = GeneratePhoneVerificationCode();
            var member = MemberRepository.Get(memberId);

            int vMemberId = Convert.ToInt32(MemberDetails(memberId, IntegrationVoltronId)[IntegrationVoltronId]);
            if (member != null)
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        try
                        {
                            // check if this mobile is verified by another member before. 
                            var existingMpv = MemberPhoneVerificationRepository.GetAll().SingleOrDefault(w => w.Verified && w.MemberId != memberId && w.Phone == phoneNumberWithCountryCode);
                            if (existingMpv != null)
                            {
                                result.Message = "PHONE_ALREADY_VERIFIED_BY_ANOTHER_MEMBER";
                                return result;
                            }


                            var mpv = MemberPhoneVerificationRepository.GetAll().SingleOrDefault(w => w.MemberId == memberId);
                            if (mpv == null)
                            {
                                var item = new MemberPhoneVerification
                                {
                                    MemberId = memberId,
                                    CreateDate = DateTime.UtcNow,
                                    Code = code,
                                    SentCount = 1,
                                    Verified = false,
                                    Phone = phoneNumberWithCountryCode,
                                    VerifyDate = DateTime.UtcNow
                                };
                                MemberPhoneVerificationRepository.Insert(item);
                            }
                            else if (mpv.Verified)
                            {
                                // kick him out, as he already verified their phone
                                result.Message = "PHONE_ALREADY_VERIFIED";
                                return result;
                            }
                            else if (mpv.SentCount > 3)
                            {
                                result.Message = "NOTIFICATION_MAX_SMS_COUNT_REACHED";
                                return result;
                                // reached sending sms limit, contact live chat should be shown to player.
                            }
                            else
                            {
                                code = mpv.Code;
                                mpv.SentCount = mpv.SentCount + 1;
                                MemberPhoneVerificationRepository.Update(mpv);
                            }

                            uniOfWork.Commit(transaction);

                            var sms = new SMSRequest
                            {
                                To = "+" + phoneNumberWithCountryCode,
                                //From = "BayMavi",
                                Message = "BayMavi dogrulama kodunuz: " + code
                            };
                            try
                            {
                                sms.Send(member.Username, vMemberId);
                            }
                            catch (Exception ex)
                            {
                                Logger.Fatal("SMS Sending error " + ex.Message, ex);
                            }

                            result.Message = "";
                            result.IsSuccess = true;
                            return result;
                        }
                        catch (Exception ex)
                        {
                            Logger.Fatal("Err while SendValidatePhoneCode: " + ex.Message, ex);
                            uniOfWork.Rollback(transaction);
                            result.Message = ex.Message;
                            return result;
                        }
                    }
                }

            }

            return result;
        }
        public Core.Contracts.GenericResult SendValidatePhoneCodeByCall(int memberId, string phoneNumberWithCountryCode)
        {
            Core.Contracts.GenericResult result = new Core.Contracts.GenericResult { IsSuccess = false, Message = "GenericError", ResponseCode = -1 };

            string code = GeneratePhoneVerificationCode();
            var member = MemberRepository.Get(memberId);

            int vMemberId = Convert.ToInt32(MemberDetails(memberId, IntegrationVoltronId)[IntegrationVoltronId]);
            if (member != null)
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        try
                        {
                            // check if this mobile is verified by another member before. 
                            var existingMpv = MemberPhoneVerificationRepository.GetAll().SingleOrDefault(w => w.Verified && w.MemberId != memberId && w.Phone == phoneNumberWithCountryCode);
                            if (existingMpv != null)
                            {
                                result.Message = "PHONE_ALREADY_VERIFIED_BY_ANOTHER_MEMBER";
                                return result;
                            }


                            var mpv = MemberPhoneVerificationRepository.GetAll().SingleOrDefault(w => w.MemberId == memberId);
                            if (mpv == null)
                            {
                                var item = new MemberPhoneVerification
                                {
                                    MemberId = memberId,
                                    CreateDate = DateTime.UtcNow,
                                    Code = code,
                                    SentCount = 1,
                                    Verified = false,
                                    Phone = phoneNumberWithCountryCode,
                                    VerifyDate = DateTime.UtcNow
                                };
                                MemberPhoneVerificationRepository.Insert(item);
                            }
                            else if (mpv.Verified)
                            {
                                // kick him out, as he already verified their phone
                                result.Message = "PHONE_ALREADY_VERIFIED";
                                return result;
                            }
                            else if (mpv.SentCount > 3)
                            {
                                result.Message = "NOTIFICATION_MAX_SMS_COUNT_REACHED";
                                return result;
                                // reached sending sms limit, contact live chat should be shown to player.
                            }
                            else
                            {
                                code = mpv.Code;
                                mpv.SentCount = mpv.SentCount + 1;
                                MemberPhoneVerificationRepository.Update(mpv);
                            }

                            uniOfWork.Commit(transaction);


                            try
                            {
                                Helper.HttpHelper.MakeRequestResponseWithHeader("http://login.caller247.com:8000/api/v1/generateOTP", "{\"type\":\"voice\",\"number\":\"" + phoneNumberWithCountryCode + "\",\"otp_message\":\"" + code + "\",\"lang\":\"tr\"}", "POST", "application/json", new System.Collections.Specialized.NameValueCollection() { { "token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2Nlc3NfaWQiOjEsImFwaV91c2VyIjoiNzcxNjY2NTAiLCJhcGlfcGFzc3dvcmQiOiJpTmV4dHJpeEAxMjMiLCJ3aGl0ZWxpc3RfaXAiOiIyNy42MS4xNjUuMzciLCJpYXQiOjE2MTE3Mzc2MjQsImV4cCI6MzMxNjkzMzc2MjR9.a1f1KKM6vPcgf25_gX8X9CLrhCtxpWWW7yZFY8E_Uv8" } });
                            }
                            catch (Exception ex)
                            {
                                Logger.Fatal("SMS Sending error " + ex.Message, ex);
                            }

                            result.Message = "";
                            result.IsSuccess = true;
                            return result;
                        }
                        catch (Exception ex)
                        {
                            Logger.Fatal("Err while SendValidatePhoneCode: " + ex.Message, ex);
                            uniOfWork.Rollback(transaction);
                            result.Message = ex.Message;
                            return result;
                        }
                    }
                }

            }

            return result;
        }

        //AuthenticationResultModel model = new AuthenticationResultModel{ForceToValidateWithPhoneDueToDeviceDifference = true};
        public bool PhoneChallangeReqiured(int memberId, string countryCode, string userAgent, string ipAddress)
        {
            IQueryable<MemberDeviceToken> deviceTokens = MemberDeviceTokenRepository.GetAll().Where(w => w.MemberId == memberId);
            if (deviceTokens.Any()) // there are some devices exist so we can check against them. 
            {
                if (deviceTokens.Select(w => w.IP).ToList().Contains(ipAddress))
                {
                    return false;
                }

                if (deviceTokens.Select(w => w.UserAgent).ToList().Contains(userAgent))
                {
                    return false;
                }
            }
            else
            {
                // first time device record so be gentle but save the devicetoken
                var dId = PhoneChallangeAccept(memberId, countryCode, userAgent, ipAddress);
                if (dId < 1)
                {
                    Logger.Fatal("PhoneChallangeAccept returned false in PhoneChallangeReqiured method");
                }
                return false;
            }

            return true;
        }


        public int PhoneChallangeAccept(int memberId, string countryCode, string userAgent, string ipAddress)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    try
                    {
                        var item = new MemberDeviceToken
                        {
                            MemberId = memberId,
                            CreateDate = DateTime.UtcNow,
                            LastUsedDate = DateTime.UtcNow,
                            UserAgent = userAgent,
                            IP = ipAddress,
                            CountryCode = countryCode
                        };
                        MemberDeviceTokenRepository.Insert(item);

                        uniOfWork.Commit(transaction);
                        return item.Id;
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal("Err while phonechallangeaccept: " + ex.Message, ex);
                        uniOfWork.Rollback(transaction);
                        return 0;
                    }
                }
            }

            return 0;
        }

        public void InsertMemberTrack(string domain, int memberId, string ip, string absoluteUri)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    try
                    {
                        MemberTrackingRepository.Insert(new MemberTracking() { Domain = domain, CreateDate = DateTime.UtcNow, Ip = ip, MemberId = memberId, AbsoluteUri = absoluteUri });

                        uniOfWork.Commit(transaction);

                    }
                    catch (Exception ex)
                    {
                        uniOfWork.Rollback(transaction);
                    }
                }
            }
        }
        public void InsertMemberSessionTracking(string domain, string username, string device, string os, string country, string osVersion, string referral, string promoCode, string @namespace, string ip, string userAgent, bool isProduction)
        {
            int? comapanyId = CompanyService.CompanyId(domain);
            Member member = MemberRepository.ActiveMember(comapanyId.Value, username);
            try
            {
                var partnerUrlToRegisterUser = CompanyService.GetValue(comapanyId.Value,
                    "Voltron.ServiceURL", isProduction);
                var apiUsername = CompanyService.GetValue(comapanyId.Value, "Voltron.APIUsername",
                    isProduction);
                var apiPassword = CompanyService.GetValue(comapanyId.Value, "Voltron.APIPassword",
                    isProduction);
                var vCompanyId = CompanyService.GetValue(comapanyId.Value, "Voltron.CompanyId",
                    isProduction);


                var response =
                    HttpServiceHelper.GetJsonRequest(partnerUrlToRegisterUser + string.Format("MemberSessionTracking?companyId={0}&apiUsername={1}&username={2}&device={3}&os={4}&country={5}&osVersion={6}&referral={7}&promoCode={8}&namespace={9}&domain={10}&ip={11}&userAgent={12}&checksum={13}",
                    vCompanyId, apiUsername, username, device, os, country, osVersion, referral, promoCode, @namespace, domain, ip, userAgent,
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username, device, os, country, osVersion, referral, promoCode, @namespace, domain, ip, userAgent)));

            }
            catch (Exception ex)
            {
            }
        }
        public PagingModel<MemberTracking> GetMemberTrackingInfo(int pageIndex, int pageSize)
        {
            PagingModel<MemberTracking> pagingModel = new PagingModel<MemberTracking>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MemberTrackingRepository.GetAll().Count();
                    pagingModel.ItemList = Session.QueryOver<MemberTracking>()
                            .OrderBy(t => t.Id).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public PagingModel<MemberTracking> GetMemberTrackingInfoForMember(int memberId, int pageIndex, int pageSize)
        {
            PagingModel<MemberTracking> pagingModel = new PagingModel<MemberTracking>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MemberTrackingRepository.GetAll().Where(mt => mt.MemberId == memberId).Count();
                    pagingModel.ItemList = Session.QueryOver<MemberTracking>()
                            .Where(mt => mt.MemberId == memberId)
                            .OrderBy(t => t.Id).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public PagingModel<MemberDeviceFingerPrint> GetMemberDeviceFingerPrintForMember(int memberId, int pageIndex, int pageSize)
        {
            PagingModel<MemberDeviceFingerPrint> pagingModel = new PagingModel<MemberDeviceFingerPrint>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MemberDeviceFingerPrintRepository.GetAll().Where(mt => mt.MemberId == memberId).Count();
                    pagingModel.ItemList = Session.QueryOver<MemberDeviceFingerPrint>()
                            .Where(mt => mt.MemberId == memberId)
                            .OrderBy(t => t.UpdateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public void UpdateMemberDeviceFingerPrint(int memberId, int memberDeviceFingerPrintId, int statusType)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    MemberDeviceFingerPrint memberDeviceFingerPrint = MemberDeviceFingerPrintRepository.GetAll().FirstOrDefault(mp => mp.MemberId == memberId && mp.Id == memberDeviceFingerPrintId);
                    memberDeviceFingerPrint.StatusType = statusType;
                    MemberDeviceFingerPrintRepository.Update(memberDeviceFingerPrint);
                    transaction.Commit();
                }
            }
        }
        public IList<Member> GetMemberListByHash(int id, string hash)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                return MemberDeviceFingerPrintRepository.GetAll().Where(mdfp => mdfp.Hash == hash && mdfp.MemberId != id).Select(m => m.Member).ToList();
            }
        }
        public PagingModel<MemberLoginLogout> GetMemberLoginLogoutsForMember(int memberId, int pageIndex, int pageSize)
        {
            PagingModel<MemberLoginLogout> pagingModel = new PagingModel<MemberLoginLogout>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MemberLoginLogoutRepository.GetAll().Where(mt => mt.MemberId == memberId).Count();
                    pagingModel.ItemList = Session.QueryOver<MemberLoginLogout>()
                        .Where(mt => mt.MemberId == memberId)
                        .OrderBy(t => t.Id).Desc
                        .Skip(pageIndex * pageSize)
                        .Take(pageSize)
                        .List();
                }
            }
            return pagingModel;
        }
        public bool UpdateBrowserConnection(string username, string connectionId, string currentUrl, string device, string ipAddress)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    try
                    {
                        MemberConnection mc =
                            MemberConnectionRepository.GetAll().LastOrDefault(w => w.Username == username);

                        if (mc == null)
                        {
                            MemberConnectionRepository.Insert(new MemberConnection
                            {
                                ConnectionId = connectionId,
                                CreateDate = DateTime.UtcNow,
                                UpdateDate = DateTime.UtcNow,
                                Device = device,
                                MemberId = 0,
                                CurrentUrl = currentUrl,
                                Username = username,
                                IP = ipAddress,
                                IsOnline = true

                            });
                        }
                        else
                        {
                            mc.CurrentUrl = currentUrl;
                            mc.ConnectionId = connectionId;
                            mc.Device = device;
                            mc.UpdateDate = DateTime.UtcNow;
                            mc.IsOnline = true;
                            mc.IP = ipAddress;
                        }
                        // create/update connection
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal("UpdateBrowserConnection: " + ex.Message, ex);
                        transaction.Rollback();
                    }
                }
            }
            return false;
        }

        public bool IsFavourited(int gameId, string domain, string username)
        {
            Member member = GetActiveMember(domain, username);
            using (var uniOfWork = UnitOfWork.Current)
            {


                return member.FavoriteGames.Any(g => g.Id == gameId);
            }
        }

        public bool AddFavouriteGame(int gameId, string domain, string username)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    Member member = MemberRepository.ActiveMember(CompanyService.CompanyId(domain), username);
                    Game game = GameRepository.Game(gameId);

                    member.FavoriteGames.Add(game);

                    MemberRepository.Update(member);
                    uniOfWork.Commit(transaction);
                }
            }
            return true;
        }

        public bool RemoveFavouriteGame(int gameId, string domain, string username)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    Member member = MemberRepository.ActiveMember(CompanyService.CompanyId(domain), username);
                    Game game = member.FavoriteGames.FirstOrDefault(g => g.Id == gameId);
                    if (game != null)
                    {
                        member.FavoriteGames.Remove(game);

                        MemberRepository.Update(member);
                        uniOfWork.Commit(transaction);
                    }
                }
            }
            return true;
        }

        public IList<Level> LevelList()
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return LevelRepository.LevelList().ToList();
            }
        }

        public void CheckLevelAfterDeposit(string domain, int memberId, bool isProduction)
        {
            Member member = GetMember(memberId);
            CheckLevelAfterDeposit(domain, member, isProduction);
        }

        public void CheckLevelAfterDeposit(string domain, Member member, bool isProduction)
        {
            if (member.Level.SystemName == "pff")
            {


                member.Level = LevelRepository.Level("regular");
                member.LevelId = member.Level.Id;

                UpdateMember(member);

                //open 3 providers after first deposit
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        MemberDisabledPaymentMethod memberDisabledPaymentMethodBT1 = MemberDisabledPaymentMethodRepository.GetAll().FirstOrDefault(mdpm => mdpm.MemberId == member.Id && mdpm.ProviderId == 57);
                        MemberDisabledPaymentMethod memberDisabledPaymentMethodBT2 = MemberDisabledPaymentMethodRepository.GetAll().FirstOrDefault(mdpm => mdpm.MemberId == member.Id && mdpm.ProviderId == 58);
                        MemberDisabledPaymentMethod memberDisabledPaymentMethodBT3 = MemberDisabledPaymentMethodRepository.GetAll().FirstOrDefault(mdpm => mdpm.MemberId == member.Id && mdpm.ProviderId == 81);

                        if (memberDisabledPaymentMethodBT1 != null)
                            MemberDisabledPaymentMethodRepository.Delete(memberDisabledPaymentMethodBT1.Id);
                        if (memberDisabledPaymentMethodBT2 != null)
                            MemberDisabledPaymentMethodRepository.Delete(memberDisabledPaymentMethodBT2.Id);
                        if (memberDisabledPaymentMethodBT3 != null)
                            MemberDisabledPaymentMethodRepository.Delete(memberDisabledPaymentMethodBT3.Id);


                        uniOfWork.Commit(transaction);
                    }
                }


                //int? companyId = CompanyService.CompanyId(domain);
                //if (companyId.HasValue)
                //{

                //    var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                //    var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                //    var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                //    var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);
                //    var methodName = "UpdateMemberLevel";

                //    var response2 =
                //        HttpServiceHelper.PostJsonRequest(partnerUrl + methodName,
                //            JsonConvert.SerializeObject(new
                //            {
                //                CompanyId = companyId,
                //                ApiUsername = apiUsername,
                //                ApiPassword = apiPassword,
                //                Username = member.Username,
                //                MemberId = member.Id,
                //                LevelId = member.LevelId,
                //                Checksum = ""
                //            })
                //        );

                //    Logger.Debug("VResponse: " + JsonConvert.SerializeObject(response2));

                //    bool balanceUpdateSuccessful = Convert.ToBoolean(response2.Success);
                //    if (balanceUpdateSuccessful)
                //    {
                //        UpdateMember(member);
                //    }
                //}
            }
        }
        public bool GiveFreeSpinAfterDepositForEligibleUsersCepBank(int companyId, string username, long amount)
        {
            string domain = CompanyDomainRepository.GetLiveDomain(companyId);

            CustomStuff lame = CustomStuffRepository.Get(7);
            JObject jObject;
            Core.Contracts.GenericResult genericResult = new Core.Contracts.GenericResult();
            if (lame != null && ((!lame.ExpiryDate.HasValue) || lame.ExpiryDate > DateTime.UtcNow))
            {
                jObject = (JObject)JsonConvert.DeserializeObject(lame.Data);
                genericResult = new Core.Contracts.GenericResult();
                if (amount >= 2000 && (((JArray)jObject["usernames"]).FirstOrDefault().Value<string>() == "ALL" || ((JArray)jObject["usernames"]).Any(u => u.ToString() == username)))
                {
                    genericResult = EligibleForFreespin(domain, username, Convert.ToInt32(jObject["spinType"]), Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    if (genericResult.IsSuccess)
                    {
                        Game game = GameRepository.Get(Convert.ToInt32(jObject["gameId"]));
                        int spinValue = jObject["spinValue"] != null ? Convert.ToInt32(jObject["spinValue"]) : 1;
                        genericResult = ApplyWelcomeFreespins(domain, username, Convert.ToInt32(jObject["freeRound"]), Convert.ToInt32(jObject["spinType"]), spinValue, game.VoltronGameId, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    }
                }
            }

            lame = CustomStuffRepository.Get(8);
            if (lame != null && ((!lame.ExpiryDate.HasValue) || lame.ExpiryDate > DateTime.UtcNow))
            {
                jObject = (JObject)JsonConvert.DeserializeObject(lame.Data);
                genericResult = new Core.Contracts.GenericResult();
                if (amount >= 5000 && (((JArray)jObject["usernames"]).FirstOrDefault().Value<string>() == "ALL" || ((JArray)jObject["usernames"]).Any(u => u.ToString() == username)))
                {
                    string currency = "TRY";
                    string country = "TR";
                    long voltronAmount = jObject["amount"].Value<long>() * 100;
                    string couponCode = jObject["couponCode"].ToString();
                    genericResult = EligibleForFreebet(domain, username, couponCode, voltronAmount, DateTime.UtcNow.AddDays(jObject["expirationDate"].Value<int>()), currency, country, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    if (genericResult.IsSuccess)
                    {
                        genericResult = ApplyFreebet(domain, username, couponCode, voltronAmount, DateTime.UtcNow.AddDays(jObject["expirationDate"].Value<int>()), currency, country, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    }

                }
            }

            return genericResult.IsSuccess;
        }
        public bool GiveFreeSpinAfterDepositForEligibleUsersReyPay(int companyId, string username, long amount)
        {
            string domain = CompanyDomainRepository.GetLiveDomain(companyId);

            CustomStuff lame = CustomStuffRepository.Get(12);
            JObject jObject;
            Core.Contracts.GenericResult genericResult = new Core.Contracts.GenericResult();
            if (lame != null && ((!lame.ExpiryDate.HasValue) || lame.ExpiryDate > DateTime.UtcNow))
            {
                jObject = (JObject)JsonConvert.DeserializeObject(lame.Data);
                genericResult = new Core.Contracts.GenericResult();
                if (amount >= 5000 && (((JArray)jObject["usernames"]).FirstOrDefault().Value<string>() == "ALL" || ((JArray)jObject["usernames"]).Any(u => u.ToString() == username)))
                {
                    genericResult = EligibleForFreespin(domain, username, Convert.ToInt32(jObject["spinType"]), Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    if (genericResult.IsSuccess)
                    {
                        Game game = GameRepository.Get(Convert.ToInt32(jObject["gameId"]));
                        int spinValue = jObject["spinValue"] != null ? Convert.ToInt32(jObject["spinValue"]) : 1;
                        genericResult = ApplyWelcomeFreespins(domain, username, Convert.ToInt32(jObject["freeRound"]), Convert.ToInt32(jObject["spinType"]), spinValue, game.VoltronGameId, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    }
                }
            }

            lame = CustomStuffRepository.Get(13);
            if (lame != null && ((!lame.ExpiryDate.HasValue) || lame.ExpiryDate > DateTime.UtcNow))
            {
                jObject = (JObject)JsonConvert.DeserializeObject(lame.Data);
                genericResult = new Core.Contracts.GenericResult();
                if (amount >= 5000 && (((JArray)jObject["usernames"]).FirstOrDefault().Value<string>() == "ALL" || ((JArray)jObject["usernames"]).Any(u => u.ToString() == username)))
                {
                    string currency = "TRY";
                    string country = "TR";
                    long voltronAmount = jObject["amount"].Value<long>() * 100;
                    string couponCode = jObject["couponCode"].ToString();
                    genericResult = EligibleForFreebet(domain, username, couponCode, voltronAmount, DateTime.UtcNow.AddDays(jObject["expirationDate"].Value<int>()), currency, country, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    if (genericResult.IsSuccess)
                    {
                        genericResult = ApplyFreebet(domain, username, couponCode, voltronAmount, DateTime.UtcNow.AddDays(jObject["expirationDate"].Value<int>()), currency, country, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    }

                }
            }

            return genericResult.IsSuccess;
        }
        public bool GiveFreeSpinAfterDepositForEligibleUsers(int companyId, string username, long amount)
        {
            string domain = CompanyDomainRepository.GetLiveDomain(companyId);

            CustomStuff lame = CustomStuffRepository.Get(1);
            JObject jObject;
            Core.Contracts.GenericResult genericResult = new Core.Contracts.GenericResult();
            if (lame != null && ((!lame.ExpiryDate.HasValue) || lame.ExpiryDate > DateTime.UtcNow))
            {
                jObject = (JObject)JsonConvert.DeserializeObject(lame.Data);
                genericResult = new Core.Contracts.GenericResult();
                if (amount >= 5000 && (((JArray)jObject["usernames"]).FirstOrDefault().Value<string>() == "ALL" || ((JArray)jObject["usernames"]).Any(u => u.ToString() == username)))
                {
                    genericResult = EligibleForFreespin(domain, username, Convert.ToInt32(jObject["spinType"]), Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    if (genericResult.IsSuccess)
                    {
                        Game game = GameRepository.Get(Convert.ToInt32(jObject["gameId"]));
                        int spinValue = jObject["spinValue"] != null ? Convert.ToInt32(jObject["spinValue"]) : 1;
                        genericResult = ApplyWelcomeFreespins(domain, username, Convert.ToInt32(jObject["freeRound"]), Convert.ToInt32(jObject["spinType"]), spinValue, game.VoltronGameId, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    }
                }
            }


            lame = CustomStuffRepository.Get(6);
            if (lame != null && ((!lame.ExpiryDate.HasValue) || lame.ExpiryDate > DateTime.UtcNow))
            {
                jObject = (JObject)JsonConvert.DeserializeObject(lame.Data);
                genericResult = new Core.Contracts.GenericResult();
                JArray conditionList = (JArray)jObject["conditionList"];
                int freeSpinCount = 0;
                foreach (var condition in conditionList)
                {
                    if (((condition["min"].Value<int>() * 100) <= amount) && ((condition["max"].Value<int>() * 100)) > amount)
                    {
                        freeSpinCount = condition["freeRound"].Value<int>();
                        break;
                    }
                }
                if (freeSpinCount != 0 && ((JArray)jObject["usernames"]).FirstOrDefault().Value<string>() == "ALL" || ((JArray)jObject["usernames"]).Any(u => u.ToString() == username))
                {
                    genericResult = EligibleForFreespin(domain, username, Convert.ToInt32(jObject["spinType"]), Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    if (genericResult.IsSuccess)
                    {
                        Game game = GameRepository.Get(Convert.ToInt32(jObject["gameId"]));
                        int spinValue = jObject["spinValue"] != null ? Convert.ToInt32(jObject["spinValue"]) : 1;
                        genericResult = ApplyWelcomeFreespins(domain, username, freeSpinCount, Convert.ToInt32(jObject["spinType"]), spinValue, game.VoltronGameId, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    }
                }
            }


            lame = CustomStuffRepository.Get(5);
            if (lame != null && ((!lame.ExpiryDate.HasValue) || lame.ExpiryDate > DateTime.UtcNow))
            {
                jObject = (JObject)JsonConvert.DeserializeObject(lame.Data);
                genericResult = new Core.Contracts.GenericResult();
                if ((amount >= jObject["minAmount"].Value<long>() * 100) && (((JArray)jObject["usernames"]).FirstOrDefault().Value<string>() == "ALL" || ((JArray)jObject["usernames"]).Any(u => u.ToString() == username)))
                {
                    string currency = "TRY";
                    string country = "TR";
                    long voltronAmount = jObject["amount"].Value<long>() * 100;
                    string couponCode = jObject["couponCode"].ToString();
                    genericResult = EligibleForFreebet(domain, username, couponCode, voltronAmount, DateTime.UtcNow.AddDays(jObject["expirationDate"].Value<int>()), currency, country, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    if (genericResult.IsSuccess)
                    {
                        genericResult = ApplyFreebet(domain, username, couponCode, voltronAmount, DateTime.UtcNow.AddDays(jObject["expirationDate"].Value<int>()), currency, country, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    }

                }
            }


            lame = CustomStuffRepository.Get(9);
            if (lame != null && ((!lame.ExpiryDate.HasValue) || lame.ExpiryDate > DateTime.UtcNow))
            {
                jObject = (JObject)JsonConvert.DeserializeObject(lame.Data);
                genericResult = new Core.Contracts.GenericResult();
                if ((amount >= jObject["minAmount"].Value<long>() * 100) && (((JArray)jObject["usernames"]).FirstOrDefault().Value<string>() == "ALL" || ((JArray)jObject["usernames"]).Any(u => u.ToString() == username)))
                {
                    string currency = "TRY";
                    string country = "TR";
                    long voltronAmount = jObject["amount"].Value<long>() * 100;
                    string couponCode = jObject["couponCode"].ToString();
                    genericResult = EligibleForFreebet(domain, username, couponCode, voltronAmount, DateTime.UtcNow.AddDays(jObject["expirationDate"].Value<int>()), currency, country, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    if (genericResult.IsSuccess)
                    {
                        genericResult = ApplyFreebet(domain, username, couponCode, voltronAmount, DateTime.UtcNow.AddDays(jObject["expirationDate"].Value<int>()), currency, country, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    }

                }
            }

            lame = CustomStuffRepository.Get(10);
            if (lame != null && ((!lame.ExpiryDate.HasValue) || lame.ExpiryDate > DateTime.UtcNow))
            {
                jObject = (JObject)JsonConvert.DeserializeObject(lame.Data);
                genericResult = new Core.Contracts.GenericResult();
                if ((amount >= jObject["minAmount"].Value<long>() * 100) && (((JArray)jObject["usernames"]).FirstOrDefault().Value<string>() == "ALL" || ((JArray)jObject["usernames"]).Any(u => u.ToString() == username)))
                {
                    string currency = "TRY";
                    string country = "TR";
                    long voltronAmount = jObject["amount"].Value<long>() * 100;
                    string couponCode = jObject["couponCode"].ToString();
                    genericResult = EligibleForFreebet(domain, username, couponCode, voltronAmount, DateTime.UtcNow.AddDays(jObject["expirationDate"].Value<int>()), currency, country, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    if (genericResult.IsSuccess)
                    {
                        genericResult = ApplyFreebet(domain, username, couponCode, voltronAmount, DateTime.UtcNow.AddDays(jObject["expirationDate"].Value<int>()), currency, country, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    }

                }
            }

            lame = CustomStuffRepository.Get(11);
            if (lame != null && ((!lame.ExpiryDate.HasValue) || lame.ExpiryDate > DateTime.UtcNow))
            {
                jObject = (JObject)JsonConvert.DeserializeObject(lame.Data);
                genericResult = new Core.Contracts.GenericResult();
                if ((amount >= jObject["minAmount"].Value<long>() * 100) && (((JArray)jObject["usernames"]).FirstOrDefault().Value<string>() == "ALL" || ((JArray)jObject["usernames"]).Any(u => u.ToString() == username)))
                {
                    string currency = "TRY";
                    string country = "TR";
                    long voltronAmount = jObject["amount"].Value<long>() * 100;
                    string couponCode = jObject["couponCode"].ToString();
                    genericResult = EligibleForFreebet(domain, username, couponCode, voltronAmount, DateTime.UtcNow.AddDays(jObject["expirationDate"].Value<int>()), currency, country, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    if (genericResult.IsSuccess)
                    {
                        genericResult = ApplyFreebet(domain, username, couponCode, voltronAmount, DateTime.UtcNow.AddDays(jObject["expirationDate"].Value<int>()), currency, country, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                    }

                }
            }

            return genericResult.IsSuccess;
        }

        public bool GiveBonusAfterDepositForEligibleUsers(int companyId, string username, bool isProduction, long amount, int voltronTransactionTypeId, int providerId)
        {
            //Jeton 11010, Cepbank 11003, Astropay 11004, Paykasa 11008 veya Qr kodu 11011
            List<int> voltronTransactionTypes = new List<int>() { 11010, 11003, 11004, 11008, 11011 };
            List<int> providerIdList = new List<int>() { 61, 62, 63, 64, 65, 67, 68, 69, 31, 32, 39, 60 };
            string domain = CompanyDomainRepository.GetLiveDomain(companyId);
            Member member = GetActiveMember(companyId, username);

            string voltronMemberId = MemberDetails(member.Id, IntegrationVoltronId).FirstOrDefault().Value;
            JObject jObject;
            Core.Contracts.GenericResult genericResult = new Core.Contracts.GenericResult();
            if (voltronTransactionTypes.Contains(voltronTransactionTypeId) || (voltronTransactionTypeId == 4 && providerIdList.Contains(providerId)))
            {
                CustomStuff lame = CustomStuffRepository.Get(14);
                if (lame != null && ((!lame.ExpiryDate.HasValue) || lame.ExpiryDate > DateTime.UtcNow))
                {
                    jObject = (JObject)JsonConvert.DeserializeObject(lame.Data);
                    genericResult = new Core.Contracts.GenericResult();
                    if ((amount >= jObject["minAmount"].Value<long>() * 100) && (((JArray)jObject["usernames"]).FirstOrDefault().Value<string>() == "ALL" || ((JArray)jObject["usernames"]).Any(u => u.ToString() == username)))
                    {
                        long bonusAmount = (jObject["percent"].Value<long>() * amount) / 100;
                        long maxBonusAmount = jObject["maxBonusAmount"].Value<long>() * 100;
                        if (bonusAmount > maxBonusAmount)
                        {
                            bonusAmount = maxBonusAmount;
                        }

                        int bonusVoltronTransactionTypeId = jObject["voltronTransactionTypeId"].Value<int>();
                        string voltronTransactionTypeResourceName = jObject["voltronTransactionTypeResourceName"].Value<string>();
                        genericResult = EligibleForDepositBonus(member.Id, bonusVoltronTransactionTypeId);
                        if (genericResult.IsSuccess)
                        {
                            // use bonusAmount (dont multiply with 100)

                            Core.Contracts.GenericResult result = new Core.Contracts.GenericResult { IsSuccess = false };

                            int? comapanyId = CompanyService.CompanyId(domain);
                            if (comapanyId.HasValue)
                            {
                                var partnerUrl = CompanyService.GetValue(comapanyId.Value, "Voltron.BOServiceURL", isProduction);
                                var voltronCompanyId = CompanyService.GetValue(comapanyId.Value, "Voltron.CompanyId", isProduction);
                                var apiUsername = CompanyService.GetValue(comapanyId.Value, "Voltron.APIUsername", isProduction);
                                var apiPassword = CompanyService.GetValue(comapanyId.Value, "Voltron.APIPassword", isProduction);

                                partnerUrl = partnerUrl + string.Format("InsertVoltronTransaction/?companyId={0}&apiUsername={1}&transactionTypeId={2}&providerId={3}&memberId={4}&transaction3rdPartyRef={5}&transactionRefId={6}&amount={7}&description={8}&checksum={9}",
                                voltronCompanyId, apiUsername, bonusVoltronTransactionTypeId, providerId, voltronMemberId, "0", 0, bonusAmount, Uri.EscapeUriString(voltronTransactionTypeResourceName), SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, voltronCompanyId.ToString(), apiUsername, bonusVoltronTransactionTypeId.ToString(), providerId.ToString(), voltronMemberId.ToString(), "0", 0.ToString(), bonusAmount.ToString(), Uri.EscapeUriString(voltronTransactionTypeResourceName)));

                                var response = HttpServiceHelper.GetJsonRequest(partnerUrl);

                                if (response.status != null && Convert.ToBoolean(response.status) || response.status == null)
                                {
                                    if (Convert.ToBoolean(response.Success))
                                    {
                                        using (var uniOfWork = UnitOfWork.Current)
                                        {
                                            using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                                            {
                                                DepositBonusHistoryRepository.Insert(new DepositBonusHistory() { MemberId = member.Id, Amount = bonusAmount, PaymentProviderId = providerId, VoltronTransactionTypeId = bonusVoltronTransactionTypeId, CreateDate = DateTime.UtcNow });
                                                transaction.Commit();
                                            }
                                        }
                                    }
                                }

                            }
                        }

                    }
                }
            }

            // bo bitcoin and cubits
            if (voltronTransactionTypeId == 40 || voltronTransactionTypeId == 11012)
            {
                CustomStuff lame = CustomStuffRepository.Get(15);
                if (lame != null && ((!lame.ExpiryDate.HasValue) || lame.ExpiryDate > DateTime.UtcNow))
                {
                    jObject = (JObject)JsonConvert.DeserializeObject(lame.Data);
                    genericResult = new Core.Contracts.GenericResult();
                    JArray conditionList = (JArray)jObject["conditionList"];
                    int freeSpinCount = 0;
                    foreach (var condition in conditionList)
                    {
                        if (((condition["min"].Value<int>() * 100) <= amount) && ((condition["max"].Value<int>() * 100)) > amount)
                        {
                            freeSpinCount = condition["freeRound"].Value<int>();
                            break;
                        }
                    }
                    if (freeSpinCount != 0 && ((JArray)jObject["usernames"]).FirstOrDefault().Value<string>() == "ALL" || ((JArray)jObject["usernames"]).Any(u => u.ToString() == username))
                    {
                        genericResult = EligibleForFreespin(domain, username, Convert.ToInt32(jObject["spinType"]), Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                        if (genericResult.IsSuccess)
                        {
                            Game game = GameRepository.Get(Convert.ToInt32(jObject["gameId"]));
                            int spinValue = jObject["spinValue"] != null ? Convert.ToInt32(jObject["spinValue"]) : 1;
                            genericResult = ApplyWelcomeFreespins(domain, username, freeSpinCount, Convert.ToInt32(jObject["spinType"]), spinValue, game.VoltronGameId, Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));
                        }
                    }
                }
            }

            return genericResult.IsSuccess;
        }
        public bool CreateLoginCode(int companyId, string language, string channel, string phone, string email, Member member, int vMemberId)
        {
            bool result = false;
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    MemberLoginCode memberLoginCode = MemberLoginCodeRepository.GetAll().Where(lc => lc.MemberId == member.Id && lc.CreateDate > DateTime.UtcNow.AddMinutes(LOGIN_CODE_EXPIRY_MINUTE) && lc.StatusType == (int)StatusType.Passive).FirstOrDefault();
                    if (memberLoginCode == null)
                    {
                        string code = new Random().Next(10000, 99999).ToString();
                        MemberLoginCodeRepository.Insert(new MemberLoginCode() { MemberId = member.Id, StatusType = (int)StatusType.Passive, CreateDate = DateTime.UtcNow, LoginCode = code, TryCount = 1, TryLoginCodeCount = 0 });

                        if (channel == "sms")
                        {
                            var sms = new SMSRequest
                            {
                                To = "+90" + phone,
                                //From = "BayMavi",
                                Message = string.Format("Dogrulama kodu: {0} , bu kodu siz talep etmediyseniz canli yardima baglanin.", code)
                            };
                            try
                            {
                                sms.Send(member.Username, vMemberId);
                            }
                            catch (Exception ex)
                            {
                                Logger.Fatal("SMS Sending error " + ex.Message, ex);
                            }

                            //PasifikSMSRequest.Send(phone, code);
                            //SMSRequest smsRequest = new SMSRequest();
                            //smsRequest.To = phone;
                            //smsRequest.Message = code;
                            //smsRequest.Send();

                            uniOfWork.Commit(transaction);
                        }
                        else if (channel == "email")
                        {
                            MailingProcessService.SendMail(companyId, language, email, EmailType.TwoWayAuth, new Dictionary<string, string>() { { "code", code } });
                            uniOfWork.Commit(transaction);
                        }
                        result = true;
                    }
                    else
                    {
                        if (memberLoginCode.TryCount <= 3)
                        {
                            if (channel == "sms")
                            {
                                var sms = new SMSRequest
                                {
                                    To = "+90" + phone,
                                    //From = "BayMavi",
                                    Message = string.Format("Dogrulama kodu: {0} , bu kodu siz talep etmediyseniz canli yardima baglanin.", memberLoginCode.LoginCode)
                                };
                                try
                                {
                                    sms.Send(member.Username, vMemberId);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Fatal("SMS Sending error " + ex.Message, ex);
                                }
                            }
                            else if (channel == "email")
                            {
                                MailingProcessService.SendMail(companyId, language, email, EmailType.TwoWayAuth, new Dictionary<string, string>() { { "code", memberLoginCode.LoginCode } });
                            }
                            memberLoginCode.TryCount++;
                            MemberLoginCodeRepository.Update(memberLoginCode);
                            uniOfWork.Commit(transaction);
                            result = true;
                        }
                    }
                }
            }


            return result;
        }


        public bool CheckLoginCode(string code, int memberId)
        {
            bool result = false;
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    MemberLoginCode memberLoginCode = MemberLoginCodeRepository.GetAll().Where(lc => lc.MemberId == memberId && lc.StatusType == (int)StatusType.Passive && lc.CreateDate > DateTime.UtcNow.AddMinutes(LOGIN_CODE_EXPIRY_MINUTE)).FirstOrDefault();
                    if (memberLoginCode != null && memberLoginCode.TryLoginCodeCount <= 3)
                    {
                        if (memberLoginCode.LoginCode == code)
                        {
                            memberLoginCode.StatusType = (int)StatusType.Active;
                            result = true;
                        }
                        else
                        {
                            memberLoginCode.TryLoginCodeCount++;
                        }
                        MemberLoginCodeRepository.Update(memberLoginCode);
                        uniOfWork.Commit(transaction);
                    }
                }
            }
            return result;
        }


        public void MoveMemberDeviceFingerPrintHistory(int memberId, string hash, bool reset)
        {
            if (!string.IsNullOrEmpty(hash) || reset)
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        IEnumerable<MemberDeviceFingerPrint> memberDeviceFingerPrintList = MemberDeviceFingerPrintRepository.GetAll().Where(mdfp => mdfp.MemberId == memberId && (mdfp.Hash != hash || reset));
                        foreach (MemberDeviceFingerPrint md in memberDeviceFingerPrintList)
                        {
                            MemberDeviceFingerPrintHistoryRepository.Insert(new MemberDeviceFingerPrintHistory()
                            {
                                MemberId = md.MemberId,
                                StatusType = md.StatusType,
                                Hash = md.Hash,
                                CreateDate = md.CreateDate,
                                UpdateDate = md.UpdateDate,
                                IsLoggedIn = md.IsLoggedIn,
                                IP = md.IP,
                                Geolocation = md.Geolocation
                            });

                            MemberDeviceFingerPrintRepository.Delete(md.Id);
                        }


                        transaction.Commit();
                    }
                }
            }
        }


        public string LoginCode(int memberId)
        {
            string loginCode = string.Empty;
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    MemberLoginCode memberLoginCode = MemberLoginCodeRepository.GetAll().Where(lc => lc.MemberId == memberId && lc.StatusType == (int)StatusType.Passive && lc.CreateDate > DateTime.UtcNow.AddMinutes(LOGIN_CODE_EXPIRY_MINUTE)).FirstOrDefault();
                    if (memberLoginCode != null)
                    {
                        loginCode = memberLoginCode.LoginCode;
                    }
                }
            }

            return loginCode;
        }


        public Tuple<bool, string> SavePasscode(int memberId, Guid guid, string device, string passcode)
        {
            Tuple<bool, string> result;
            try
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        MemberPasscodeRepository.Insert(new MemberPasscode() { CreateDate = DateTime.UtcNow, StatusType = 1, Passcode = passcode, Guid = guid, Device = device, MemberId = memberId });
                        transaction.Commit();

                        result = Tuple.Create<bool, string>(true, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                result = Tuple.Create<bool, string>(false, ex.Message);
            }
            return result;
        }

        public Tuple<bool, string> CheckPasscode(int memberId, Guid guid, string device, string passcode)
        {
            Tuple<bool, string> result;
            try
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        MemberPasscode memberPasscode = MemberPasscodeRepository.GetAll().FirstOrDefault(mp => mp.MemberId == memberId && mp.Guid == guid && mp.Device == device && mp.StatusType == 1 && mp.Passcode == passcode);
                        result = Tuple.Create<bool, string>(memberPasscode != null, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                result = Tuple.Create<bool, string>(false, ex.Message);
            }
            return result;
        }

        public Tuple<bool, string> ForgetPasscode(int memberId, Guid guid, string device)
        {
            Tuple<bool, string> result;
            try
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        MemberPasscode memberPasscode = MemberPasscodeRepository.GetAll().FirstOrDefault(mp => mp.MemberId == memberId && mp.Guid == guid && mp.Device == device && mp.StatusType == 1);
                        memberPasscode.StatusType = -1;
                        MemberPasscodeRepository.Update(memberPasscode);
                        transaction.Commit();
                        result = Tuple.Create<bool, string>(true, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                result = Tuple.Create<bool, string>(false, ex.Message);
            }
            return result;
        }



        public bool IsHashValidByMemberId(int memberId, string hash)
        {
            int[] validStatusList = new int[] { (int)NW.Core.Enum.MemberDeviceFingerPrintStatusType.Whitelisted, (int)NW.Core.Enum.MemberDeviceFingerPrintStatusType.Neutral, (int)NW.Core.Enum.MemberDeviceFingerPrintStatusType.Force2FA };
            return MemberDeviceFingerPrintRepository.GetAll().Any(mdf => mdf.MemberId == memberId && validStatusList.Contains(mdf.StatusType) && mdf.Hash == hash);
        }

        public bool IsBlacklistIP(string ip)
        {
            return IPBlacklistRepository.GetAll().Any(ib => ib.IP == ip && ib.BlockTo > DateTime.UtcNow);
        }
        public bool IsLastHashValidByMemberId(int memberId, string hash)
        {
            MemberDeviceFingerPrint memberDeviceFingerPrint = MemberDeviceFingerPrintRepository.GetAll().OrderByDescending(mdf => mdf.UpdateDate).FirstOrDefault(mdf => mdf.MemberId == memberId);
            return memberDeviceFingerPrint != null && memberDeviceFingerPrint.Hash == hash;
        }
        public void InsertFingerPrintWebhook(string hash, bool botProbability, string requestId, string ip, string data)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    int countRequest = MemberDeviceFingerPrintWebhookHistoryRepository.GetAll().Count(mdfpw => mdfpw.RequestId == requestId && mdfpw.CreateDate > DateTime.UtcNow.AddDays(-1));


                    MemberDeviceFingerPrintWebhookHistoryRepository.Insert(new DeviceFingerPrintWebhookHistory() { Hash = hash, Data = data, CreateDate = DateTime.UtcNow, BotProbability = botProbability, RequestId = requestId, IP = ip });

                    if (countRequest == 0 && requestId.Length == 20 && hash.Length == 20)
                    {
                        DeviceFingerPrintWebhook memberDeviceFingerPrintWebhook = MemberDeviceFingerPrintWebhookRepository.GetAll().FirstOrDefault(mdfpw => mdfpw.Hash == hash);
                        if (memberDeviceFingerPrintWebhook == null)
                        {
                            memberDeviceFingerPrintWebhook = new DeviceFingerPrintWebhook();
                            memberDeviceFingerPrintWebhook.Hash = hash;
                            memberDeviceFingerPrintWebhook.CreateDate = DateTime.UtcNow;
                            memberDeviceFingerPrintWebhook.UpdateDate = DateTime.UtcNow;
                            memberDeviceFingerPrintWebhook.Data = data;
                            memberDeviceFingerPrintWebhook.BotProbability = botProbability;
                            memberDeviceFingerPrintWebhook.RequestId = requestId;
                            memberDeviceFingerPrintWebhook.IP = ip;
                            MemberDeviceFingerPrintWebhookRepository.Insert(memberDeviceFingerPrintWebhook);
                            transaction.Commit();
                        }
                        else
                        {
                            memberDeviceFingerPrintWebhook.UpdateDate = DateTime.UtcNow;
                            memberDeviceFingerPrintWebhook.Data = data;
                            memberDeviceFingerPrintWebhook.BotProbability = botProbability;
                            memberDeviceFingerPrintWebhook.RequestId = requestId;
                            memberDeviceFingerPrintWebhook.IP = ip;
                            MemberDeviceFingerPrintWebhookRepository.Update(memberDeviceFingerPrintWebhook);
                            transaction.Commit();
                        }
                    }
                    else
                    {
                        transaction.Commit();
                    }



                }
            }
        }

        public IList<Tuple<DeviceFingerPrint, int, IList<Member>>> GetTopFraudHashList(string hash)
        {
            Dictionary<string, int> topFraudHashList = MemberDeviceFingerPrintRepository.GetAll().Where(md => string.IsNullOrEmpty(hash) || md.Hash == hash).GroupBy(dfp => dfp.Hash).Select(dfp => new { Hash = dfp.Key, Count = dfp.Count() }).OrderByDescending(d => d.Count).Take(50).ToDictionary(d => d.Hash, d => d.Count);

            IList<Tuple<DeviceFingerPrint, int, IList<Member>>> result = new List<Tuple<DeviceFingerPrint, int, IList<Member>>>();

            foreach (var deviceFingerPrint in topFraudHashList)
            {
                result.Add(Tuple.Create<DeviceFingerPrint, int, IList<Member>>(DeviceFingerPrintRepository.DeviceFingerPrint(deviceFingerPrint.Key), deviceFingerPrint.Value, MemberListByHash(deviceFingerPrint.Key)));
            }
            return result;
        }

        public bool UpdateDeviceFingerPrint(int id, int statusType)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {

                    DeviceFingerPrint deviceFingerPrint = DeviceFingerPrintRepository.Get(id);
                    deviceFingerPrint.UpdateDate = DateTime.UtcNow;
                    deviceFingerPrint.StatusType = statusType;


                    DeviceFingerPrintRepository.Update(deviceFingerPrint);


                    transaction.Commit();
                }
            }

            return true;
        }


        public IList<Member> MemberListByHash(string hash)
        {
            return MemberDeviceFingerPrintRepository.GetAll().Where(mdfp => mdfp.Hash == hash).Select(mdfp => mdfp.Member).ToList();
        }

        public bool Is2FAEnabled(int companyId)
        {
            bool result = false;
            string value = CompanyService.GetValue(companyId, "Site.2FAEnabled", Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"]));

            bool.TryParse(value, out result);

            return result;
        }
        public bool Is2FAEnabledByMemberId(int companyId, int memberId)
        {
            bool result = Is2FAEnabled(companyId);

            bool memberResult = false;
            string key = MemberDetails(memberId, TwoFAEnabledKey)[TwoFAEnabledKey];

            bool.TryParse(key, out memberResult);

            return result && memberResult;
        }
        public MemberDeviceFingerPrint MemberDeviceFingerPrint(int memberId, string hash)
        {
            MemberDeviceFingerPrint memberDeviceFingerPrint = MemberDeviceFingerPrintRepository.GetAll().FirstOrDefault(mdfp => mdfp.MemberId == memberId && mdfp.Hash == hash);
            return memberDeviceFingerPrint;
        }
        public void SetMemberDeviceFingerPrintStatusType(int memberId, string hash, MemberDeviceFingerPrintStatusType memberDeviceFingerPrintStatusType)
        {
            int count = MemberDeviceFingerPrintRepository.GetCountHashByMemberId(memberId);

            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    MemberDeviceFingerPrint memberDeviceFingerPrint = MemberDeviceFingerPrintRepository.GetAll().FirstOrDefault(mdfp => mdfp.MemberId == memberId && mdfp.Hash == hash);
                    memberDeviceFingerPrint.UpdateDate = DateTime.UtcNow;
                    memberDeviceFingerPrint.IsLoggedIn = true;

                    if (memberDeviceFingerPrint.StatusType != (int)MemberDeviceFingerPrintStatusType.Force2FA)
                    {
                        memberDeviceFingerPrint.StatusType = (int)memberDeviceFingerPrintStatusType;
                    }

                    MemberDeviceFingerPrintRepository.Update(memberDeviceFingerPrint);


                    transaction.Commit();
                }
            }

        }


        public void SetNeutralAllWhitelistedMemberDeviceFingerPrintStatusType(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    foreach (MemberDeviceFingerPrint memberDeviceFingerPrint in MemberDeviceFingerPrintRepository.GetAll().Where(mdfp => mdfp.MemberId == memberId && mdfp.StatusType == (int)NW.Core.Enum.MemberDeviceFingerPrintStatusType.Whitelisted))
                    {
                        memberDeviceFingerPrint.UpdateDate = DateTime.UtcNow;
                        memberDeviceFingerPrint.StatusType = (int)NW.Core.Enum.MemberDeviceFingerPrintStatusType.Neutral;
                        MemberDeviceFingerPrintRepository.Update(memberDeviceFingerPrint);
                    }


                    transaction.Commit();
                }
            }
        }
    }
}
