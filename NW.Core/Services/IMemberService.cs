using NW.Core.Contracts;
using NW.Core.Contracts.Game;
using NW.Core.Contracts.Member;
using NW.Core.Contracts.Payment;
using NW.Core.Entities;
using NW.Core.Entities.Report;
using NW.Core.Enum;
using NW.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface IMemberService
    {
        #region Keys
        string RegisterAddressKey { get; }
        string RegisterAddressFullLineKey { get; }
        string RegisterPostCodeKey { get; }
        string RegisterBirthdayKey { get; }
        string RegisterGenderKey { get; }
        string RegisterCityKey { get; }
        string RegisterCountryKey { get; }
        string RegisterPhoneKey { get; }
        string RegisterNewsletterKey { get; }
        string RegisterSMSKey { get; }
        string RegisterPromotionsKey { get; }
        string RegisterIdKey { get; }
        string RegisterConfirmationKey { get; }
        string VerificationEmailKey { get; }
        string VerificationPhoneKey { get; }
        string VerificationTelegramKey { get; }
        string RegisterEmailVerifiedKey { get; }
        string RegisterPhoneVerifiedKey { get; }
        string RegisterTelegramUsername { get; }
        string RegisterTelegramVerificationCode { get; }
        string SportsBetLimit { get; }
        string DepositBonusPercentage { get; }
        string IntegrationVoltronId { get; }
        string InteractionLoginDateKey { get; }
        string InteractionLogoutDateKey { get; }
        string BackOfficeNote { get; }
        string AffiliateRevenueCoefficient { get; }
        string DepositWithDifferentAccountEnabledKey { get; }
        string WithdrawWithDifferentAccountEnabledKey { get; }
        string AttentionOnWithdraw { get; }
        string WithdrawFlagUserKey { get; }
        string KycBirthdayStatusType { get; }

        string SMS2FAEnabledKey { get; }
        string TwoFAEnabledKey { get; }

        string IsEnabledCryptoWithdrawEvenNotDeposit { get; }
        string WithdrawBTCAddress { get; }


        string PassCode { get; }

        #endregion

        Contracts.GenericResult EligibleForFreespin(string domain, string username, int spinType, bool isProduction);
        Contracts.GenericResult EligibleForDepositBonus(int memberId, int voltronTransactionTypeId);
        bool UpdateDesktopNotification(string domain, string username, bool granted, string subscriptionId, string endpoint);
        string GetMemberEcoPayzNumber(string domain, string usernameOrEmail);
        int UpdateMemberCandidateWithPhoneNumber(string domain, string language, string mobilePhone, int memberId);
        bool IsMemberCandidateRegistered(string domain, string language, string mobilePhone);
        ResultModel Step1Register(string domain, string language, string username, string email, string password,string aff);

        Contracts.GenericResult ApplyWelcomeFreespins(string domain, string username, int numberOfFreeRound, int spinType, int spinValue, int voltronGameId, bool isProduction);
        Contracts.GenericResult ApplyFreebet(string domain, string username, string couponCode, long amount, DateTime expirationDate, string currency, string country, bool isProduction);

        ResultModel Register(string domain, string language, string ip, string email, string password, string username,
            string firstName,
            string lastName, string day, string month, string year, string gender, string country, string city,
            string phone, string address,
            bool isProduction, string hash = "", string promoCode = "", string affCode = "", string cSource = "", string cMedium = "",string cName = "", string refUrl="");
        ResultModel Login(string domain, string language, string email, string password, string ip, string userAgent, bool isProduction, string hash = null);
        ResultModel MemberEmailVerification(string domain, string language, string email, string hash);
        ResultModel MemberEmailVerificationInformation(string domain, string language, string username);
        void InsertMemberTrack(string domain, int memberId, string ip, string absoluteUri);
        void InsertMemberSessionTracking(string domain, string username, string device, string os, string country, string osVersion, string referral, string promoCode, string @namespace, string ip, string userAgent, bool isProduction);
        PagingModel<MemberTracking> GetMemberTrackingInfo(int pageIndex, int pageSize);
        PagingModel<MemberTracking> GetMemberTrackingInfoForMember(int memberId, int pageIndex, int pageSize);
        PagingModel<MemberDeviceFingerPrint> GetMemberDeviceFingerPrintForMember(int memberId, int pageIndex, int pageSize);
        void UpdateMemberDeviceFingerPrint(int memberId, int memberDeviceFingerPrintId, int statusType);
        IList<Member> GetMemberListByHash(int id, string hash);
        PagingModel<MemberLoginLogout> GetMemberLoginLogoutsForMember(int memberId, int pageIndex, int pageSize);
        bool UpdateBrowserConnection(string username, string connectionId, string currentUrl, string device,string ipAddress);
        //bool CloseBrowserConnection(string connectionId);
        List<string> GetMemberCandidates(int annotationId, int skip, int take);
        List<MemberCandidate> GetMemberCandidates(int skip, int take);
        List<string> GetMembersEmails(string domain);
        Member GetActiveMember(string domain, string emailOrUsername);
        Member GetActiveMember(int companyId, string emailOrUsername);
        Member GetMember(int companyId, string emailOrUsername);
        Member GetMemberByMemberDetail(string key, string value);
        List<string> GetUsernameListByMemberDetailList(string key, string[] valueList);
        Member GetMember(int memberId);
        void UpdateMember(Member member);
        IList<int> GetGameHistory(string domain, string username);
        bool AddGameHistory(string domain, string username, int gameId);
        //BalanceResult GetBalance(string domain, string emailOrUsername, bool isProduction);
        MemberBalanceResult GetBalance(string domain, string username, bool isProduction);
        BalanceResult GetBalance(Member member, bool? isProduction = null);
        FinancialInfoResult GetFinancialInfo(string domain, string username, bool isProduction); 
        MemberBalanceResult GetMemberBalance(Member member, bool? isProduction = null);
        //BalanceResult UpdateBalance(string domain, string usernameOrEmail, long amountToAdd, long partnerRef, bool isProduction);
        PlayGameResult Play(string domain, string language, string username, int id, string sessionId, bool pff, bool isProduction);
        PlayGameResult SportsbookSessionId(string domain, string uniqueId, bool isProduction);
        PlayGameResult Sportsbook(string domain, string language, string username, bool isProduction);
        PlayGameResult SportsbookHistory(string domain, string language, string username, bool isProduction);
        ResultModel ValidateUsername(string domain, string language, string username);
        ResultModel ValidateEmail(string domain, string language, string email);
        Dictionary<string, string> MemberDetails(string domain, string email, params string[] keys);
        Dictionary<string, string> MemberDetails(int memberId, params string[] keys);
        ResultModel UpdateMemberDetails(string domain, string username, Dictionary<string, string> memberDetails);
        ResultModel UpdateMemberDetails(int memberId, Dictionary<string, string> memberDetails);
        PagingModel<Transaction> Transactions(string domain, int pageIndex, int pageSize, int transactionTypeId, DateTime startDate, DateTime endDate, string username, bool isProduction);
        PagingModel<MemberSummaryReport> AffiliateMemberSummaryReport(string domain, int pageIndex, int pageSize, string username, bool isProduction);
        PagingModel<AffiliateSummaryReport> AffiliateSummaryReport(string domain, int pageIndex, int pageSize, string username, bool isProduction, DateTime startDate, DateTime endDate);
        IList<Level> LevelList();
        void CheckLevelAfterDeposit(string domain, int memberId, bool isProduction);
        void CheckLevelAfterDeposit(string domain, Member member, bool isProduction);
        bool IsFavourited(int gameId, string domain, string username);
        bool AddFavouriteGame(int gameId, string domain, string username);
        bool RemoveFavouriteGame(int gameId, string domain, string username);
        ResultModel ValidatePassword(string domain, string password, string firstName, string lastName, string username);
        int PhoneChallangeAccept(int memberId, string countryCode, string userAgent, string ipAddress);
        bool PhoneChallangeReqiured(int memberId, string countryCode, string userAgent, string ipAddress);
        Contracts.GenericResult SendValidatePhoneCode(int memberId, string phoneNumberWithCountryCode);
        Contracts.GenericResult SendValidatePhoneCodeByCall(int memberId, string phoneNumberWithCountryCode);
        Contracts.GenericResult ValidatePhoneWithCode(int memberId, string code);
        bool PhoneVerified(int memberId);
        bool TelegramVerified(int memberId);
        bool EmailVerified(int memberId);
        //BonusResult BonusList(Member member, string bonusPlaceSystemName, bool? isProduction = null);
        //BonusResult BonusList(string domain, string username, string bonusPlaceSystemName, bool isProduction);
        ActivatedBonusResult ActivatedBonusList(string domain, string username, bool isProduction);
        BonusResult BonusEngineBonusList(string domain, string username, bool isProduction);
        //ResultModel ActivateBonus(string domain, string username, bool isProduction, int bonusId, long? relatedVoltronTransactionId, long? amount);
        ResultModel ActivateBonus(int companyId, string username, bool isProduction, int bonusId, long? relatedVoltronTransactionId, long? amount, long? allocatedAmount, bool? checkEligibility);
        Contracts.GenericResult ApplyFreeSpinBonusEngine(string domain, string username, int numberOfFreeRound, int bonusId, int spinValue, int voltronGameId, bool isProduction);
        ResultModel ForfeitBonus(string domain, string username, bool isProduction, int memberBonusId);
        bool GiveFreeSpinAfterDepositForEligibleUsers(int companyId, string username, long amount);
        bool GiveFreeSpinAfterDepositForEligibleUsersCepBank(int companyId, string username, long amount);
        bool GiveFreeSpinAfterDepositForEligibleUsersReyPay(int companyId, string username, long amount);
        bool GiveBonusAfterDepositForEligibleUsers(int companyId, string username, bool isProduction, long amount, int voltronTransactionTypeId, int providerId);
        bool CreateLoginCode(int companyId, string language, string channel, string phone, string email, Member member, int vMemberId);
        bool CheckLoginCode(string code, int memberId);
        string LoginCode(int memberId);



        Tuple<bool, string> SavePasscode(int memberId, Guid guid, string device, string passcode);
        Tuple<bool, string> CheckPasscode(int memberId, Guid guid, string device, string passcode);
        Tuple<bool, string> ForgetPasscode(int memberId, Guid guid, string device);


        bool IsHashValidByMemberId(int memberId, string hash);
        bool IsBlacklistIP(string ip);
        bool IsLastHashValidByMemberId(int memberId, string hash);
        void MoveMemberDeviceFingerPrintHistory(int memberId, string hash, bool reset);
        void InsertFingerPrintWebhook(string hash, bool botProbability, string requestId, string ip, string data);
        IList<Tuple<DeviceFingerPrint, int, IList<Member>>> GetTopFraudHashList(string hash);
        bool UpdateDeviceFingerPrint(int id, int statusType);
        IList<Member> MemberListByHash(string hash);
        bool Is2FAEnabled(int companyId);
        bool Is2FAEnabledByMemberId(int companyId, int memberId);
        MemberDeviceFingerPrint MemberDeviceFingerPrint(int memberId, string hash);
        void SetMemberDeviceFingerPrintStatusType(int memberId, string hash, MemberDeviceFingerPrintStatusType memberDeviceFingerPrintStatusType);
        void SetNeutralAllWhitelistedMemberDeviceFingerPrintStatusType(int memberId);
    }
}