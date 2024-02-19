using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Logging;
using Newtonsoft.Json;
using NHibernate.Type;
using NW.Core.Contracts.Payment;
using NW.Core.Entities;
using NW.Core.Entities.Payment;
using NW.Core.Model;
using NW.Core.Repositories;
using NW.Core.Repositories.Payment;
using NW.Core.Services;
using NW.Core.Services.Payment;
using NW.Core.Work;
using NW.Helper;
using NW.Helper.SMS;
using NW.Payment.Wrappers.Citigate;
using NW.Payment.Wrappers.EcoPayz;
using NW.Payment.Wrappers.EProPayment;
using NW.Security;
using NW.Service;
using NHibernate;
using NW.Core.Enum;
using RestSharp;
using System.Net;
using NW.Core.Model.Finance;
using AutoMapper;
using NW.Payment.Wrappers.PayKasa;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using NW.Payment.Wrappers.BP;
using NW.Payment.Wrappers.ReyPay;
using RestSharp.Extensions.MonoHttp;
using NW.Core.Model.KafkaModel;
using NW.Helper.Kafka;
using System.Configuration;
using System.Globalization;
using NCrontab;
using NW.Helper.ServiceBus;
using NW.Core.Model.Payments;

namespace NW.Services
{

    public class Utf8StringWriter : StringWriter
    {
        public Utf8StringWriter()
            : base()
        {
        }
        public override Encoding Encoding { get { return Encoding.UTF8; } }

    }


    public class PaymentService : BaseService, IPaymentService
    {
        private const int DEFAULT_DEPOSIT_PERCENTAGE = 15;
        public ICompanyService CompanyService { get; set; }
        public ICompanyDomainRepository CompanyDomainRepository { get; set; }
        public IMailingProcessService MailingProcessService { get; set; }
        public IProviderService ProviderService { get; set; }

        public ICreditCardRepository CreditCardRepository { get; set; }
        public ICreditCardRequestRepository CreditCardRequestRepository { get; set; }
        public ICreditCardResponseRepository CreditCardResponseRepository { get; set; }
        public IMemberRepository MemberRepository { get; set; }
        public IMemberService MemberService { get; set; }
        IRepository<Currency, int> CurrencyRepository { get; set; }
        IRepository<CurrencyUpdate, int> CurrencyUpdateRepository { get; set; }
        IRepository<CurrencyRate, int> CurrencyRateRepository { get; set; }
        IRepository<BankAccount, int> BankAccountRepository { get; set; }
        IRepository<PaparaAccount, int> PaparaAccountRepository { get; set; }
        IRepository<WithdrawRequestBankTransfer, int> WithdrawRequestBankTransferRepository { get; set; }
        IRepository<JetRequest, int> JetRequestRepository { get; set; }
        IRepository<BPCRequest, int> BPCRequestRepository { get; set; }
        IRepository<KingCommunityBankTransferRequest, int> KingCommunityBankTransferRequestRepository { get; set; }
        IRepository<KingQRDepositRequest, int> KingQRDepositRequestRepository { get; set; }
        IRepository<BPCPaymentMethodType, int> BPCPaymentMethodTypeRepository { get; set; }
        IRepository<OtoPayTransaction, int> OtoPayRepository { get; set; }
        IRepository<Bank, int> BankRepository { get; set; }
        IRepository<PaparaTransferPaparaAccount, int> PaparaTransferPaparaAccountRepository { get; set; }
        IRepository<BankTransferRequest, int> BankTransferRequestRepository { get; set; }
        IRepository<PaparaTransferRequest, int> PaparaTransferRequestRepository { get; set; }
        IRepository<MemberDisabledPaymentMethod, int> MemberDisabledPaymentMethodRepository { get; set; }
        IRepository<RejectedBin, int> RejectedBinRepository { get; set; }
        IRepository<PayminoRequest, int> PayminoRequestRepository { get; set; }
        IRepository<PayminoRequest2, int> PayminoRequest2Repository { get; set; }
        IRepository<CepBankRequest, int> CepBankRequestRepository { get; set; }
        IRepository<GenericDepositRequest, int> GenericDepositRequestRepository { get; set; }
        IRepository<NewBankTransferRequest, int> NewBankTransferRequestRepository { get; set; }
        IRepository<CMTAccount, int> CMTAccountRepository { get; set; }
        IRepository<PEPAccount, int> PEPAccountRepository { get; set; }
        IRepository<BankTransferRequestProviderHistory, int> BankTransferRequestProviderHistoryRepository { get; set; }
        IRepository<CMTTransferCMTAccount, int> CMTTransferCMTAccountRepository { get; set; }
        IRepository<PEPTransferPEPAccount, int> PEPTransferPEPAccountRepository { get; set; }
        IRepository<CMTTransferRequest, int> CMTTransferRequestRepository { get; set; }
        IRepository<PEPTransferRequest, int> PEPTransferRequestRepository { get; set; }
        IRepository<ReyPayCMTRequest, int> ReyPayCMTRequestRepository { get; set; }
        IRepository<MGCreditCardRequest, int> MGCreditCardRequestRepository { get; set; }
        IRepository<NewPaymentProvider, int> NewPaymentProviderRepository { get; set; }

        IRepository<PayfixAccount, int> PayfixAccountRepository { get; set; }
        IRepository<PayfixTransferPayfixAccount, int> PayfixTransferPayfixAccountRepository { get; set; }
        IRepository<PayfixTransferRequest, int> PayfixTransferRequestRepository { get; set; }
        IRepository<ParazulaAccount, int> ParazulaAccountRepository { get; set; }
        IRepository<ParazulaTransferParazulaAccount, int> ParazulaTransferParazulaAccountRepository { get; set; }
        IRepository<ParazulaTransferRequest, int> ParazulaTransferRequestRepository { get; set; }
        IRepository<MefeteAccount, int> MefeteAccountRepository { get; set; }
        IRepository<MefeteTransferMefeteAccount, int> MefeteTransferMefeteAccountRepository { get; set; }
        IRepository<MefeteTransferRequest, int> MefeteTransferRequestRepository { get; set; }
        IRepository<PaybinRequest, int> PaybinRequestRepository { get; set; }
        IRepository<BankTransferV2Request, int> BankTransferRequestV2Repository { get; set; }
        IRepository<BankTransferV2BankTransferAccount, int> BankTransferV2BankTransferAccountRepository { get; set; }
        Logger Logger { get; set; }

        public PaymentService(ICompanyDomainRepository _companyDomainRepository, IMailingProcessService _mailingProcessService, ICompanyService _companyService,
            ICreditCardRepository _creditCardRepository,
            IMemberRepository _memberRepository,
            ICreditCardRequestRepository _creditCardRequestRepository,
            ICreditCardResponseRepository _creditCardResponseRepository,
            IRepository<Currency, int> _currencyRepository,
            IRepository<CurrencyUpdate, int> _currencyUpdateRepository,
            IRepository<CurrencyRate, int> _currencyRateRepository,
            IRepository<BankAccount, int> _bankAccountRepository,
            IRepository<PaparaAccount, int> _paparaAccountRepository,
            IRepository<WithdrawRequestBankTransfer, int> _withdrawRequestBankTransferRepository,
            IRepository<JetRequest, int> _jetRequestRepository,
            IRepository<BPCRequest, int> _bpcRequestRepository,
            IRepository<KingCommunityBankTransferRequest, int> _kingCommunityBankTransferRequestRepository,
            IRepository<KingQRDepositRequest, int> _kingQRDepositRequestRepository,
            IRepository<BPCPaymentMethodType, int> _bpcPaymentMethodTypeRepository,
            IRepository<Bank, int> _bankRepository,
            IRepository<PaparaTransferPaparaAccount, int> _paparaTransferPaparaAccountRepository,
            IRepository<BankTransferRequest, int> _bankTransferRequestRepository,
            IRepository<PaparaTransferRequest, int> _paparaTransferRequestRepository,
            IRepository<MemberDisabledPaymentMethod, int> _memberDisabledPaymentMethodRepository,
            IRepository<RejectedBin, int> _rejectedBinRepository,
            IRepository<PayminoRequest, int> _payminoRequestRepository,
            IRepository<PayminoRequest2, int> _payminoRequest2Repository,
            IRepository<CepBankRequest, int> _cepBankRequestRepository,
            IRepository<GenericDepositRequest, int> _genericDepositRequestRepository,
            IRepository<NewBankTransferRequest, int> _newBankTransferRequestRepository,
            IRepository<CMTAccount, int> _cmtAccountRepository,
            IRepository<BankTransferRequestProviderHistory, int> _bankTransferRequestProviderHistoryRepository,
            IRepository<CMTTransferCMTAccount, int> _cmtTransferCMTAccountRepository,
            IRepository<CMTTransferRequest, int> _cmtTransferRequestRepository,
            IRepository<PEPAccount, int> _pepAccountRepository,
            IRepository<PEPTransferPEPAccount, int> _pepTransferPEPAccountRepository,
            IRepository<PEPTransferRequest, int> _pepTransferRequestRepository,
            IRepository<ReyPayCMTRequest, int> _reyPayCMTRequestRepository,
            IRepository<NewPaymentProvider, int> _newPaymentProviderRepository,
            IRepository<PayfixAccount, int> _payfixAccountRepository,
            IRepository<PayfixTransferPayfixAccount, int> _payfixTransferPayfixAccountRepository,
            IRepository<PayfixTransferRequest, int> _payfixTransferRequestRepository,
            IRepository<ParazulaAccount, int> _parazulaAccountRepository,
            IRepository<ParazulaTransferParazulaAccount, int> _parazulaTransferParazulaAccountRepository,
            IRepository<ParazulaTransferRequest, int> _parazulaTransferRequestRepository,
            IRepository<MefeteAccount, int> _mefeteAccountRepository,
            IRepository<MefeteTransferMefeteAccount, int> _mefeteTransferMefeteAccountRepository,
            IRepository<MefeteTransferRequest, int> _mefeteTransferRequestRepository,
            IRepository<PaybinRequest, int> _paybinRequestRepository,
            IRepository<BankTransferV2Request, int> _bankTransferRequestV2Repository,
            IMemberService _memberService,
            IProviderService _providerService,
            IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            CompanyDomainRepository = _companyDomainRepository;
            MailingProcessService = _mailingProcessService;
            CompanyService = _companyService;
            CreditCardRepository = _creditCardRepository;
            MemberRepository = _memberRepository;
            MemberService = _memberService;
            ProviderService = _providerService;
            CreditCardResponseRepository = _creditCardResponseRepository;
            CreditCardRequestRepository = _creditCardRequestRepository;
            CurrencyRepository = _currencyRepository;
            CurrencyUpdateRepository = _currencyUpdateRepository;
            CurrencyRateRepository = _currencyRateRepository;
            BankAccountRepository = _bankAccountRepository;
            PaparaAccountRepository = _paparaAccountRepository;
            WithdrawRequestBankTransferRepository = _withdrawRequestBankTransferRepository;
            JetRequestRepository = _jetRequestRepository;
            BPCRequestRepository = _bpcRequestRepository;
            BPCPaymentMethodTypeRepository = _bpcPaymentMethodTypeRepository;
            KingCommunityBankTransferRequestRepository = _kingCommunityBankTransferRequestRepository;
            KingQRDepositRequestRepository = _kingQRDepositRequestRepository;
            BankRepository = _bankRepository;
            PaparaTransferPaparaAccountRepository = _paparaTransferPaparaAccountRepository;
            BankTransferRequestRepository = _bankTransferRequestRepository;
            PaparaTransferRequestRepository = _paparaTransferRequestRepository;
            MemberDisabledPaymentMethodRepository = _memberDisabledPaymentMethodRepository;
            RejectedBinRepository = _rejectedBinRepository;
            PayminoRequestRepository = _payminoRequestRepository;
            PayminoRequest2Repository = _payminoRequest2Repository;
            CepBankRequestRepository = _cepBankRequestRepository;
            GenericDepositRequestRepository = _genericDepositRequestRepository;
            NewBankTransferRequestRepository = _newBankTransferRequestRepository;
            CMTAccountRepository = _cmtAccountRepository;
            BankTransferRequestProviderHistoryRepository = _bankTransferRequestProviderHistoryRepository;
            CMTTransferCMTAccountRepository = _cmtTransferCMTAccountRepository;
            CMTTransferRequestRepository = _cmtTransferRequestRepository;
            PEPAccountRepository = _pepAccountRepository;
            PEPTransferPEPAccountRepository = _pepTransferPEPAccountRepository;
            PEPTransferRequestRepository = _pepTransferRequestRepository;
            ReyPayCMTRequestRepository = _reyPayCMTRequestRepository;
            NewPaymentProviderRepository = _newPaymentProviderRepository;
            PayfixAccountRepository = _payfixAccountRepository;
            PayfixTransferPayfixAccountRepository = _payfixTransferPayfixAccountRepository;
            PayfixTransferRequestRepository = _payfixTransferRequestRepository;
            ParazulaAccountRepository = _parazulaAccountRepository;
            ParazulaTransferParazulaAccountRepository = _parazulaTransferParazulaAccountRepository;
            ParazulaTransferRequestRepository = _parazulaTransferRequestRepository;
            MefeteAccountRepository = _mefeteAccountRepository;
            MefeteTransferMefeteAccountRepository = _mefeteTransferMefeteAccountRepository;
            MefeteTransferRequestRepository = _mefeteTransferRequestRepository;
            PaybinRequestRepository = _paybinRequestRepository;
            BankTransferRequestV2Repository = _bankTransferRequestV2Repository;

            Logger = new Logger("PaymentService");
        }


        private static Tuple<DateTime, string> _envoyToken = null;
        private static Tuple<DateTime, string> EnvoyToken(string postUrl, string clientId, string clientSecret, string username, string password)
        {
            if (_envoyToken == null || _envoyToken.Item1 < DateTime.UtcNow)
            {
                JObject obj = new JObject();
                obj["grant_type"] = "password";
                obj["client_id"] = clientId;
                obj["client_secret"] = clientSecret;
                obj["username"] = username;
                obj["password"] = password;
                obj["scope"] = string.Empty;
                JObject resultObj = (JObject)JsonConvert.DeserializeObject(NW.Helper.HttpHelper.PostRequest((postUrl + "/oauth/token"), JsonConvert.SerializeObject(obj), "application/json"));

                DateTime expiryDate = DateTime.UtcNow.AddSeconds(resultObj["expires_in"].Value<int>() - 180);
                string token = resultObj["access_token"].Value<string>();

                _envoyToken = Tuple.Create(expiryDate, token);
            }

            return _envoyToken;
        }

        /// <summary>
        /// We check this bin list before processing with credit card provider!
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="filteredCardNo">First 6 digit and last 4 digit of the credit card is filteredCard number</param>
        /// <param name="isProduction"></param>
        /// <returns>if yes block the process and return error to customer.</returns>
        public bool IsInRejectedBin(string domain, string filteredCardNo, bool isProduction)
        {
            RejectedBin obj = null;
            obj = RejectedBinRepository.GetAll().SingleOrDefault(x => x.FilteredCardNo == filteredCardNo);

            return obj != null;
        }

        public CreditCard GetCardDetails(string domain, int memberId, int creditCardId, bool isProduction)
        {
            CreditCard creditCards = null;
            creditCards = CreditCardRepository.GetAll().SingleOrDefault(x => x.Id == creditCardId && x.MemberId == memberId);
            return creditCards;
        }
        public bool HasExistingCreditCards(string domain, int memberId, bool isProduction)
        {
            bool memberHasCreditCards = false;


            memberHasCreditCards = CreditCardRepository.GetAll().Any();


            return memberHasCreditCards;
        }

        public List<CreditCard> ExistingCreditCards(string domain, int memberId, bool isProduction)
        {
            List<CreditCard> creditCards = new List<CreditCard>();


            creditCards = CreditCardRepository.GetAll().Where(x => x.StatusType == 1 && x.MemberId == memberId).OrderByDescending(w => w.Id).ToList();


            return creditCards;
        }

        private bool LuhnCheck(string number)
        {
            int[] DELTAS = new int[] { 0, 1, 2, 3, 4, -4, -3, -2, -1, 0 };
            int checksum = 0;
            char[] chars = number.ToCharArray();
            for (int i = chars.Length - 1; i > -1; i--)
            {
                int j = ((int)chars[i]) - 48;
                checksum += j;
                if (((i - chars.Length) % 2) == 0)
                    checksum += DELTAS[j];
            }

            return ((checksum % 10) == 0);
        }

        private bool ValidateCardNumberAndExpiry(string cardNumber, int expiryMonth, int expiryYear, out string message)
        {
            bool valid = false;
            message = "This card is not valid"; //TODO: Use resources @MP

            // expiry check
            DateTime currentDate = DateTime.Now;
            DateTime cardExpiry = new DateTime(expiryYear, expiryMonth, 1);
            if (cardExpiry <= currentDate)
            {
                message = "This card is expired, please try a different credit card.";//TODO: Use resources @MP
            }
            else
            {
                // lucene check
                valid = LuhnCheck(cardNumber);
            }
            //TODO: is card blacklisted? @MP

            return valid;
        }
        public DepositResult StartDepositWithPaybin(string domain, int memberId, string symbol, bool isProduction, long amount, bool withBonus, int? bonusId)
        {
            DepositResult result = new DepositResult { Success = false };

            try
            {
                int? companyId = CompanyService.CompanyId(domain);
                var serviceURL = CompanyService.GetValue(companyId.Value, "Paybin.ServiceURL", isProduction);
                var publicKey = CompanyService.GetValue(companyId.Value, "Paybin.PublicKey", isProduction);
                var apiKey = CompanyService.GetValue(companyId.Value, "Paybin.XApiKey", isProduction);

                Member member = MemberService.GetMember(memberId);

                PaybinRequest paybinRequest = PaybinRequestRepository.Insert(new PaybinRequest()
                {
                    MemberId = memberId,
                    StatusType = 0,
                    CreateDate = DateTime.UtcNow,
                    Currency = member.Currency,
                    Amount = amount,
                    WithBonus = withBonus,
                    BonusId = bonusId
                });

                string data = NW.Helper.HttpHelper.PostRequest(serviceURL, JsonConvert.SerializeObject(new { Symbol = symbol, ReferenceId = member.Id, PublicKey = publicKey, Amount = amount.ToString("#.00"), Currency = member.Currency }), "application/json", new System.Collections.Specialized.NameValueCollection() { { "X-Api-Key", apiKey } });

                using (var unitOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                    {
                        paybinRequest.Data = data;
                        paybinRequest.StatusType = 1;

                        PaybinRequestRepository.Update(paybinRequest);

                        transaction.Commit();
                    }
                }

                JObject obj = JObject.Parse(data);

                result.RedirectURL = obj["data"]["url"].Value<string>();
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public DepositResult CompletePaybinCallback(string domain, bool isProduction, int referenceId, string uniqueId, decimal originalAmount, string symbol, int orderId, string signature, string callbackData, Dictionary<string, decimal> priceList)
        {
            DepositResult result = new DepositResult { Success = false };


            var paybinRequest = PaybinRequestRepository.GetAll().OrderByDescending(a => a.Id).FirstOrDefault(e => e.MemberId == referenceId);
            if (paybinRequest == null)
            {
                return result;
            }

            PaybinRequest lockedPaybinRequest = (PaybinRequest)Session.Load("PaybinRequest", paybinRequest.Id, LockMode.Upgrade);


            PaybinRequest paybinRefIdRequest = PaybinRequestRepository.GetAll().FirstOrDefault(e => e.MemberId == referenceId && e.UniqueId == uniqueId && e.OrderId == orderId);
            if (paybinRefIdRequest != null)
            {
                return result;
            }

            int? companyId = CompanyService.CompanyId(domain);
            var apiKey = CompanyService.GetValue(companyId.Value, "Paybin.XApiKey", isProduction);
            if (SecurityHelper.MD5Encryption(apiKey + uniqueId).ToLower() != signature.ToLower())
            {
                return result;
            }

            Member m = MemberService.GetMember(paybinRequest.MemberId);


            int providerId = 103;
            int? bonusId = paybinRequest.BonusId;
            long amount = (long)(priceList[m.Currency] * 100);

            var partnerUrlToDepositMoneyToAccount = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
            var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
            var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);
            var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);


            long? bonusAmount = GetBonusAmount(companyId.Value, providerId, m.Id, bonusId.HasValue, amount);

            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    if (paybinRefIdRequest == null)
                    {
                        paybinRequest = new PaybinRequest()
                        {
                            MemberId = paybinRequest.MemberId,
                            StatusType = 3,
                            Amount = amount,
                            ActualAmount = amount,
                            Currency = "TRY",
                            UniqueId = uniqueId,
                            Symbol = symbol,
                            OrderId = orderId,
                            OriginalAmount = originalAmount,
                            Data = null,
                            CallbackData = callbackData,
                            CreateDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow,
                            WithBonus = null,
                            BonusId = null
                        };
                        PaybinRequestRepository.Insert(paybinRequest);
                    }
                    else
                    {
                        paybinRequest.UpdateDate = DateTime.UtcNow;
                        paybinRequest.ActualAmount = amount;
                        paybinRequest.CallbackData = callbackData;
                        paybinRequest.OriginalAmount = originalAmount;
                        paybinRequest.Symbol = symbol;
                        paybinRequest.UniqueId = uniqueId;
                        paybinRequest.StatusType = 2;
                        PaybinRequestRepository.Update(paybinRequest);
                    }


                    transaction.Commit();
                }
            }

            string checksum = SecurityHelper.CalculateMD5HashWithPrivateKey(
                apiPassword, companyId.ToString(), apiUsername, m.Username, paybinRequest.Id.ToString(),
                uniqueId.ToString(), providerId.ToString(), amount.ToString() + (bonusAmount.HasValue ? bonusAmount.Value.ToString() : string.Empty), (bonusId.HasValue ? bonusId.ToString() : string.Empty)).ToUpper();

            var response =
            HttpServiceHelper.PostJsonRequest(partnerUrlToDepositMoneyToAccount + "DepositWithBonus",
                JsonConvert.SerializeObject(new
                {
                    CompanyId = vCompanyId,
                    ApiUsername = apiUsername,
                    ApiPassword = apiPassword,
                    Username = m.Username,
                    TransactionId = paybinRequest.Id,
                    ProviderTransactionReference = uniqueId,
                    ProviderId = providerId,
                    Amount = amount,
                    BonusAmount = bonusAmount,
                    BonusId = bonusId,
                    Checksum = checksum
                })
                );

            if (Convert.ToBoolean(response.Success))
            {
                ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (paybinRequest.ActualAmount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = m.Username, memberId = m.Id, memberUniqueId = m.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = symbol, originalAmount = (paybinRequest.OriginalAmount).ToString("0.0000000", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                NWWebHook.PushBackOffice(domain, "g:bo", "", "Player " + m.Username + " made a deposit with bank transfer (Paybin) of " + (paybinRequest.Amount * 0.01) + " TL");
                MemberService.CheckLevelAfterDeposit(domain, m, isProduction);
                MemberService.GiveBonusAfterDepositForEligibleUsers(companyId.Value, m.Username, isProduction, paybinRequest.Amount, 11015, providerId);
                MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId.Value, m.Username, paybinRequest.Amount);
            }
            else
            {
                result.ResponseDescription =
                    "Bakiye güncellerken hata oluştu. Lütfen canlı chat'e bağlanın.";
                Logger.Fatal("Bakiye güncellerken hata oluştu, sebep: " + response.Message +
                             ", obj: " + JsonConvert.SerializeObject(response));


                ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (paybinRequest.Amount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = m.Username, memberId = m.Id, memberUniqueId = m.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = symbol, originalAmount = (paybinRequest.OriginalAmount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = result.ResponseDescription }));
                DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = paybinRequest.Amount * 100, UnformattedUsername = m.Username, MemberId = m.Id, Username = m.Username, ProviderId = providerId, ProviderName = "Paybin", ErrorMessage = result.ResponseDescription };
                Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
            }

            return result;
        }
        public DepositResult VerifyPaybinWithdraw(string domain, bool isProduction, int referenceId, string uniqueId, decimal originalAmount, string symbol, int networkId, string address, string signature, string callbackData)
        {
            int providerId = 103;
            DepositResult result = new DepositResult { Success = false };

            int? companyId = CompanyService.CompanyId(domain);
            Member m = MemberService.GetMember(referenceId);

            var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
            var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
            var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
            var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

            string checksum = SecurityHelper.CalculateMD5HashWithPrivateKey(
                apiPassword, companyId.ToString(), apiUsername, m.Username, providerId.ToString(), uniqueId.ToString()).ToUpper();

            var withdrawRequest = "VerifyWithdraw";//int companyId, string apiUsername, string username, long withdrawAmount, long partnerReferenceId, string checksum

            var client = new RestClient(partnerUrl);
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            string addtionalInfoJSON = JsonConvert.SerializeObject(new CryptoAdditionalInfo() { CryptoWalletAddress = address, Currency = symbol });

            var request = new RestRequest(withdrawRequest, Method.GET);
            request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
            request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
            request.AddParameter("username", m.Username); // adds to POST or URL querystring based on Method
            request.AddParameter("providerId", providerId); // adds to POST or URL querystring based on Method // 49 papara
            request.AddParameter("uniqueId", uniqueId); // adds to POST or URL querystring based on Method // 49 papara
            request.AddParameter("additionalInfo", addtionalInfoJSON); // adds to POST or URL querystring based on Method
            request.AddParameter("checksum",
                SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, m.Username, providerId.ToString(), uniqueId)); // adds to POST or URL querystring based on Method
                                                                                                                                                   //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

            var response = client.Execute<dynamic>(request);
            result.Success = response.StatusCode == HttpStatusCode.OK && Convert.ToBoolean(response.Data["Success"]);
            return result;
        }
        public ResultModel CompleteCreditCardDepositAfter3D(string domain, int providerId, int statusCode, long amount, long transactionId, string providerTransactionReference, bool isProduction)
        {
            ResultModel result = new ResultModel { IsSuccess = false, Message = "Unknown error" };

            CreditCardResponse response3dComplete = CreditCardResponseRepository.GetAll().SingleOrDefault(w => w.CCRequestId == transactionId);
            if (response3dComplete != null)
            {
                if (response3dComplete.BalanceUpdated == false) // checking if already updated before so we wont process it twice.
                {
                    if (providerId == 34 || response3dComplete.Amount == amount) // if epro bypass the amount validation, fix this later. @cem 11/02/17
                    {
                        if (statusCode == 2) // capture
                        {
                            try
                            {
                                CreditCardRequest cr =
                                    CreditCardRequestRepository.GetAll().SingleOrDefault(w => w.Id == transactionId);

                                int? companyId = CompanyService.CompanyId(domain);
                                var partnerUrlToDepositMoneyToAccount = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);
                                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);




                                using (var unitOfWork = UnitOfWork.Current)
                                {
                                    using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                                    {
                                        Member m = MemberService.GetMember(cr.MemberId);

                                        // mark response as complete. Still balanceRefId = 0
                                        response3dComplete.BalanceUpdated = true;
                                        CreditCardResponseRepository.Update(response3dComplete);


                                        long? bonusAmount = GetBonusAmount(companyId.Value, providerId, m.Id, cr.WithBonus, cr.OriginalAmount * 100);

                                        string checksum = SecurityHelper.CalculateMD5HashWithPrivateKey(
                                            apiPassword, companyId.ToString(), apiUsername, m.Username, transactionId.ToString(),
                                            providerTransactionReference.ToString(), providerId.ToString(), amount.ToString() + (bonusAmount.HasValue ? bonusAmount.Value.ToString() : string.Empty), (cr.BonusId.HasValue ? cr.BonusId.ToString() : string.Empty)).ToUpper();


                                        // add amount to the wallet.
                                        var response =
                                            HttpServiceHelper.PostJsonRequest(partnerUrlToDepositMoneyToAccount + "DepositWithBonus",
                                                JsonConvert.SerializeObject(new
                                                {
                                                    CompanyId = vCompanyId,
                                                    ApiUsername = apiUsername,
                                                    ApiPassword = apiPassword,
                                                    Username = m.Username,
                                                    TransactionId = response3dComplete.Id,
                                                    ProviderTransactionReference = providerTransactionReference,
                                                    ProviderId = providerId,
                                                    Amount = cr.OriginalAmount * 100,
                                                    BonusAmount = bonusAmount,
                                                    BonusId = cr.BonusId,
                                                    Checksum = checksum
                                                })
                                                );

                                        if (response.status != null && Convert.ToBoolean(response.status) || response.status == null)
                                        {
                                            if (Convert.ToBoolean(response.Success))
                                            {

                                                // take the id from voltron and update the response BalanceRefId column then commit the transacion.
                                                response3dComplete.BalanceUpdated = true;
                                                response3dComplete.BalanceRefId = Convert.ToInt64(response.Message);

                                                // save
                                                unitOfWork.Commit(transaction);
                                                result.IsSuccess = true;


                                                ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (cr.Amount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = m.Username, memberId = m.Id, memberUniqueId = m.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (cr.OriginalAmount / 100m).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                                                MemberService.CheckLevelAfterDeposit(domain, m, isProduction);
                                                MemberService.GiveBonusAfterDepositForEligibleUsers(companyId.Value, m.Username, isProduction, cr.OriginalAmount * 100, 11002, providerId);
                                                MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId.Value, m.Username, cr.OriginalAmount * 100);
                                            }
                                            else
                                            {
                                                result.Message = response.Message;
                                                unitOfWork.Rollback(transaction);
                                            }
                                        }
                                        else
                                        {
                                            result.Message = "Request failed when trying to deposit to players account, response come back as null.";

                                            //throw new Exception("Request failed when trying to deposit to players account");
                                        }
                                    }


                                }
                            }
                            catch (Exception ex)
                            {
                                result.Message = ex.Message;
                                Logger.Fatal("Err complete deposit req from 3d or non3d:" + ex.Message, ex);
                            }
                        }
                        else
                        {
                            result.Message = "3d failed or not complete after a certain time.";
                            // 
                        }
                    }
                    else
                    {
                        result.Message = "Amounts do not match in order to complete response, txid:" + transactionId;
                        Logger.Fatal("Amounts do not match in order to complete response, " + transactionId);
                    }
                }
            }
            else
            {
                result.Message = "Amounts do not match in order to complete response, txid:" + transactionId;
                Logger.Fatal("Invalid transactionId to complete response, " + transactionId);
                // invalid transactionId
            }


            return result;
        }

        public DepositResult CompleteKingCommunityCallback(string domain, bool isProduction, string qs)
        {
            DepositResult result = new DepositResult { Success = false };
            int providerId = 71;

            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(qs);
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        string refCode = nameValueCollection["ref_code"];
                        int companyId = 1;
                        if (refCode.Contains("~"))
                        {
                            companyId = Convert.ToInt32(refCode.Split('~')[0]);
                            refCode = refCode.Replace((companyId + "~"), string.Empty);
                        }
                        KingCommunityBankTransferRequest kbt = KingCommunityBankTransferRequestRepository.GetAll().FirstOrDefault(k => k.ProviderRefId == refCode);

                        KingCommunityBankTransferRequest lockedKbt = Session.Load<KingCommunityBankTransferRequest>(kbt.Id, LockMode.Upgrade);

                        kbt = KingCommunityBankTransferRequestRepository.GetAll().FirstOrDefault(w => w.Id == kbt.Id && w.StatusType == 1);

                        if (kbt != null)
                        {
                            kbt.CallbackData = qs;
                            kbt.UpdateDate = DateTime.UtcNow;

                            Member member = MemberRepository.Get(kbt.MemberId);

                            var vCompanyId = CompanyService.GetValue(companyId, "Voltron.CompanyId", isProduction);

                            if (nameValueCollection["status"] == "success")
                            {
                                var partnerUrlToDepositMoneyToAccount = CompanyService.GetValue(companyId, "Voltron.ServiceURL", isProduction);
                                var apiUsername = CompanyService.GetValue(companyId, "Voltron.APIUsername", isProduction);
                                var apiPassword = CompanyService.GetValue(companyId, "Voltron.APIPassword", isProduction);


                                long? bonusAmount = GetBonusAmount(companyId, providerId, member.Id, kbt.WithBonus, kbt.Amount * 100);

                                var response2 =
                                                HttpServiceHelper.PostJsonRequest(partnerUrlToDepositMoneyToAccount + "DepositWithBonus",
                                                    JsonConvert.SerializeObject(new
                                                    {
                                                        CompanyId = vCompanyId,
                                                        ApiUsername = apiUsername,
                                                        ApiPassword = apiPassword,
                                                        Username = member.Username,
                                                        TransactionId = kbt.Id,
                                                        ProviderTransactionReference = kbt.ProviderRefId,
                                                        ProviderId = providerId,
                                                        Amount = kbt.Amount * 100,
                                                        BonusAmount = bonusAmount,
                                                        BonusId = kbt.BonusId,
                                                        Checksum = ""
                                                    })
                                                    );


                                Logger.Debug("VResponse: " + JsonConvert.SerializeObject(response2));

                                bool balanceUpdateSuccessful = Convert.ToBoolean(response2.Success);
                                if (balanceUpdateSuccessful)
                                {
                                    kbt.StatusType = 2;
                                    result.Success = true;
                                    result.Amount = Convert.ToInt64(kbt.Amount * 100);

                                    try
                                    {
                                        ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (kbt.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (kbt.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                                        NWWebHook.PushBackOffice(domain, "g:bo", "", "Player " + member.Username + " made a deposit with king community card of " + kbt.Amount + " TL");
                                        MemberService.CheckLevelAfterDeposit(domain, member, isProduction);
                                        MemberService.GiveBonusAfterDepositForEligibleUsers(companyId, member.Username, isProduction, result.Amount, 4, providerId);
                                        MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId, member.Username, result.Amount);
                                    }
                                    catch (Exception ex) { }
                                }
                                else
                                {
                                    result.ResponseDescription =
                                        "Bakiye güncellerken hata oluştu. Lütfen canlı chat'e bağlanın.";
                                    Logger.Fatal("Bakiye güncellerken hata oluştu, sebep: " + response2.Message +
                                                 ", obj: " + JsonConvert.SerializeObject(response2));

                                    ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (kbt.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (kbt.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = result.ResponseDescription }));
                                    DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = Convert.ToInt64(kbt.Amount) * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = providerId, ProviderName = "KingCommunity", ErrorMessage = result.ResponseDescription };
                                    Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                                }
                            }
                            else
                            {
                                kbt.StatusType = -1;


                                ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (kbt.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (kbt.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = nameValueCollection["status"] }));
                                DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = Convert.ToInt64(kbt.Amount) * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = providerId, ProviderName = "KingCommunity", ErrorMessage = nameValueCollection["status"] };
                                Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                            }
                            transaction.Commit();
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Fatal("Complete king community callback failed: " + ex.Message, ex);
                return result;
            }

            return result;
        }
        public DepositResult CompleteKingQRCallback(string domain, bool isProduction, int companyId, string jsonRequest)
        {
            DepositResult result = new DepositResult { Success = false };
            int providerId = 39;

            try
            {
                dynamic request = JsonConvert.DeserializeObject(jsonRequest);
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        string transactionId = request.transactionId;
                        string username = request.username;

                        KingQRDepositRequest kqr = KingQRDepositRequestRepository.GetAll().FirstOrDefault(k => k.ProviderRefId == transactionId);

                        string amount = request.amount;
                        Decimal amount2 = request.amount;

                        Member member = MemberRepository.Member(companyId, username);

                        if (member != null)
                        {
                            if (kqr == null)
                            {
                                kqr = KingQRDepositRequestRepository.Insert(new KingQRDepositRequest()
                                {
                                    MemberId = member.Id,
                                    StatusType = 1,
                                    Amount = (long)amount2,
                                    Currency = "TRY",
                                    ProviderRefId = transactionId,
                                    PaymentMethod = request.paymentMethod,
                                    SystemAdminUsername = request.admin,
                                    CallbackData = jsonRequest,
                                    CreateDate = DateTime.UtcNow,
                                });
                            }


                            KingQRDepositRequest lockedKqr = Session.Load<KingQRDepositRequest>(kqr.Id, LockMode.Upgrade);
                            kqr = KingQRDepositRequestRepository.GetAll().FirstOrDefault(k => k.Id == kqr.Id && k.StatusType == 1);

                            if (kqr != null)
                            {
                                var vCompanyId = CompanyService.GetValue(companyId, "Voltron.CompanyId", isProduction);

                                if (request.status == "APPROVED")
                                {
                                    var partnerUrlToDepositMoneyToAccount = CompanyService.GetValue(companyId, "Voltron.ServiceURL", isProduction);
                                    var apiUsername = CompanyService.GetValue(companyId, "Voltron.APIUsername", isProduction);
                                    var apiPassword = CompanyService.GetValue(companyId, "Voltron.APIPassword", isProduction);

                                    //long? bonusAmount = GetBonusAmount(member.Id, kqr.WithBonus, kqr.Amount * 100);

                                    var response2 =
                                                    HttpServiceHelper.PostJsonRequest(partnerUrlToDepositMoneyToAccount + "deposit",
                                                        JsonConvert.SerializeObject(new
                                                        {
                                                            CompanyId = vCompanyId,
                                                            ApiUsername = apiUsername,
                                                            ApiPassword = apiPassword,
                                                            Username = member.Username,
                                                            TransactionId = kqr.Id,
                                                            ProviderTransactionReference = kqr.ProviderRefId,
                                                            ProviderId = providerId,
                                                            Amount = kqr.Amount * 100,
                                                            Checksum = ""
                                                        })
                                                        );


                                    Logger.Debug("VResponse: " + JsonConvert.SerializeObject(response2));

                                    bool balanceUpdateSuccessful = Convert.ToBoolean(response2.Success);
                                    if (balanceUpdateSuccessful)
                                    {
                                        kqr.StatusType = 2;
                                        result.Success = true;
                                        result.Amount = Convert.ToInt64(kqr.Amount * 100);

                                        try
                                        {
                                            ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (kqr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (kqr.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                                            NWWebHook.PushBackOffice(domain, "g:bo", "", "Player " + member.Username + " made a deposit with king QR of " + kqr.Amount + " TL");
                                            MemberService.CheckLevelAfterDeposit(domain, member, isProduction);
                                            MemberService.GiveBonusAfterDepositForEligibleUsers(companyId, member.Username, isProduction, result.Amount, 4, providerId);
                                            MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId, member.Username, result.Amount);
                                        }
                                        catch (Exception ex) { }
                                    }
                                    else
                                    {
                                        result.ResponseDescription =
                                            "Bakiye güncellerken hata oluştu. Lütfen canlı chat'e bağlanın.";
                                        Logger.Fatal("Bakiye güncellerken hata oluştu, sebep: " + response2.Message +
                                                     ", obj: " + JsonConvert.SerializeObject(response2));

                                        ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (kqr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (kqr.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = result.ResponseDescription }));

                                        DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = Convert.ToInt64(kqr.Amount) * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = 39, ProviderName = "KingQR", ErrorMessage = result.ResponseDescription };
                                        Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                                    }
                                }
                                else
                                {

                                    kqr.StatusType = -1;

                                    ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (kqr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (kqr.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = request.status }));

                                    DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = Convert.ToInt64(kqr.Amount) * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = 39, ProviderName = "KingQR", ErrorMessage = request.status };
                                    Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                                }
                                transaction.Commit();
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Fatal("Complete king qr callback failed: " + ex.Message, ex);
                return result;
            }

            return result;
        }

        public DepositResult CompleteTrustPayCallback(string domain, bool isProduction, int companyId, string jsonRequest)
        {
            DepositResult result = new DepositResult { Success = false };
            int providerId = 97;
            try
            {
                dynamic request = JsonConvert.DeserializeObject(jsonRequest);


                string secretKey = CompanyService.GetValue(companyId, "Trustpay.SecretKey", isProduction);
                if (SecurityHelper.MD5Encryption(Convert.ToString(request.transactionId) + "." + Convert.ToString(request.userId) + "." + Convert.ToString(request.username) + "." + Convert.ToString(request.amount) + "." + secretKey) != Convert.ToString(request.hash))
                    return result;


                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        int id = Convert.ToInt32(request.processId);
                        int memberId = Convert.ToInt32(request.userId);


                        if (Convert.ToString(request.paymentMethod).ToLower() == "payfix" && request.type == "DEPOSIT")
                        {
                            PayfixTransferRequest ptq = PayfixTransferRequestRepository.GetAll().FirstOrDefault(k => k.Id == id && k.MemberId == memberId);
                            Member member = MemberRepository.Member(companyId, memberId);

                            if (ptq != null)
                            {
                                PayfixTransferRequest lockedKqr = Session.Load<PayfixTransferRequest>(ptq.Id, LockMode.Upgrade);
                                ptq = PayfixTransferRequestRepository.GetAll().FirstOrDefault(k => k.Id == ptq.Id && k.PaymentStatusType == 1);

                                if (ptq != null)
                                {
                                    var vCompanyId = CompanyService.GetValue(companyId, "Voltron.CompanyId", isProduction);

                                    if (request.status == "SUCCESSFUL")
                                    {
                                        var partnerUrlToDepositMoneyToAccount = CompanyService.GetValue(companyId, "Voltron.ServiceURL", isProduction);
                                        var apiUsername = CompanyService.GetValue(companyId, "Voltron.APIUsername", isProduction);
                                        var apiPassword = CompanyService.GetValue(companyId, "Voltron.APIPassword", isProduction);

                                        //long? bonusAmount = GetBonusAmount(member.Id, kqr.WithBonus, kqr.Amount * 100);

                                        var response2 =
                                                        HttpServiceHelper.PostJsonRequest(partnerUrlToDepositMoneyToAccount + "deposit",
                                                            JsonConvert.SerializeObject(new
                                                            {
                                                                CompanyId = vCompanyId,
                                                                ApiUsername = apiUsername,
                                                                ApiPassword = apiPassword,
                                                                Username = member.Username,
                                                                TransactionId = ptq.Id,
                                                                ProviderTransactionReference = ptq.ReferenceId,
                                                                ProviderId = providerId,
                                                                Amount = ptq.Amount * 100,
                                                                Checksum = ""
                                                            })
                                                            );


                                        Logger.Debug("VResponse: " + JsonConvert.SerializeObject(response2));

                                        bool balanceUpdateSuccessful = Convert.ToBoolean(response2.Success);
                                        if (balanceUpdateSuccessful)
                                        {
                                            ptq.PaymentStatusType = 2;
                                            result.Success = true;
                                            result.Amount = Convert.ToInt64(ptq.Amount * 100);

                                            try
                                            {
                                                ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (ptq.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (ptq.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                                                NWWebHook.PushBackOffice(domain, "g:bo", "", "Player " + member.Username + " made a deposit with king Payfix of " + ptq.Amount + " TL");
                                                MemberService.CheckLevelAfterDeposit(domain, member, isProduction);
                                                MemberService.GiveBonusAfterDepositForEligibleUsers(companyId, member.Username, isProduction, result.Amount, 4, 97);
                                                MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId, member.Username, result.Amount);
                                            }
                                            catch (Exception ex) { }
                                        }
                                        else
                                        {
                                            result.ResponseDescription =
                                                "Bakiye güncellerken hata oluştu. Lütfen canlı chat'e bağlanın.";
                                            Logger.Fatal("Bakiye güncellerken hata oluştu, sebep: " + response2.Message +
                                                         ", obj: " + JsonConvert.SerializeObject(response2));


                                            ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (ptq.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (ptq.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = result.ResponseDescription }));

                                            DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = Convert.ToInt64(ptq.Amount) * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = 97, ProviderName = "Payfix", ErrorMessage = result.ResponseDescription };
                                            Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                                        }
                                    }
                                    else
                                    {

                                        ptq.PaymentStatusType = -1;

                                        ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (ptq.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (ptq.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = request.status }));
                                        DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = Convert.ToInt64(ptq.Amount) * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = 97, ProviderName = "Payfix", ErrorMessage = request.status };
                                        Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                                    }
                                    transaction.Commit();
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Fatal("Complete trustpay qr callback failed: " + ex.Message, ex);
                return result;
            }

            return result;
        }
        public DepositResult CompleteCepbankCallback(string domain, int companyId, bool isProduction, string jsonRequest, int paymentProviderId)
        {
            DepositResult result = new DepositResult { Success = false };

            try
            {
                NameValueCollection nvCollection = HttpUtility.ParseQueryString(jsonRequest);
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        string transactionId = nvCollection["transactionid"];
                        string status = nvCollection["status"];

                        CepBankRequest cbr = CepBankRequestRepository.GetAll().FirstOrDefault(k => k.PaymentProviderId == paymentProviderId && k.ProviderRefId == transactionId);

                        Member member = cbr.Member;

                        if (member != null)
                        {
                            CepBankRequest lockedCbr = Session.Load<CepBankRequest>(cbr.Id, LockMode.Upgrade);
                            cbr = CepBankRequestRepository.GetAll().FirstOrDefault(k => k.Id == cbr.Id && k.PaymentStatusType == 0);


                            NewPaymentProvider newPaymentProvider = NewPaymentProviderRepository.Get(paymentProviderId);

                            if (cbr != null)
                            {
                                var vCompanyId = CompanyService.GetValue(companyId, "Voltron.CompanyId", isProduction);

                                if (status.ToLower() == "approved")
                                {
                                    var partnerUrlToDepositMoneyToAccount = CompanyService.GetValue(companyId, "Voltron.ServiceURL", isProduction);
                                    var apiUsername = CompanyService.GetValue(companyId, "Voltron.APIUsername", isProduction);
                                    var apiPassword = CompanyService.GetValue(companyId, "Voltron.APIPassword", isProduction);

                                    long? bonusAmount = GetBonusAmount(companyId, newPaymentProvider.VoltronProviderId.Value, member.Id, cbr.WithBonus, cbr.Amount * 100);

                                    var response2 =
                                                    HttpServiceHelper.PostJsonRequest(partnerUrlToDepositMoneyToAccount + "DepositWithBonus",
                                                        JsonConvert.SerializeObject(new
                                                        {
                                                            CompanyId = vCompanyId,
                                                            ApiUsername = apiUsername,
                                                            ApiPassword = apiPassword,
                                                            Username = member.Username,
                                                            TransactionId = cbr.Id,
                                                            ProviderTransactionReference = cbr.ProviderRefId,
                                                            ProviderId = newPaymentProvider.VoltronProviderId,
                                                            Amount = cbr.Amount * 100,
                                                            BonusAmount = bonusAmount,
                                                            BonusId = cbr.BonusId,
                                                            Checksum = ""
                                                        })
                                                        );


                                    Logger.Debug("VResponse: " + JsonConvert.SerializeObject(response2));

                                    bool balanceUpdateSuccessful = Convert.ToBoolean(response2.Success);
                                    if (balanceUpdateSuccessful)
                                    {
                                        cbr.PaymentStatusType = 2;
                                        result.Success = true;
                                        result.Amount = Convert.ToInt64(cbr.Amount * 100);

                                        try
                                        {
                                            ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (cbr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = newPaymentProvider.VoltronProviderId, currency = "TRY", originalCurrency = "TRY", originalAmount = (cbr.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                                            NWWebHook.PushBackOffice(domain, "g:bo", "", "Player " + member.Username + " made a deposit with king Cepbank of " + cbr.Amount + " TL");
                                            MemberService.CheckLevelAfterDeposit(domain, member, isProduction);
                                            MemberService.GiveBonusAfterDepositForEligibleUsers(companyId, member.Username, isProduction, result.Amount, 4, 39);
                                            MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId, member.Username, result.Amount);
                                        }
                                        catch (Exception ex) { }
                                    }
                                    else
                                    {
                                        result.ResponseDescription =
                                            "Bakiye güncellerken hata oluştu. Lütfen canlı chat'e bağlanın.";
                                        Logger.Fatal("Bakiye güncellerken hata oluştu, sebep: " + response2.Message +
                                                     ", obj: " + JsonConvert.SerializeObject(response2));

                                        ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (cbr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = newPaymentProvider.VoltronProviderId, currency = "TRY", originalCurrency = "TRY", originalAmount = (cbr.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = result.ResponseDescription }));

                                        DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = Convert.ToInt64(cbr.Amount) * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = paymentProviderId, ProviderName = "Cepbank", ErrorMessage = result.ResponseDescription };
                                        Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                                    }
                                }
                                else
                                {

                                    cbr.PaymentStatusType = -1;

                                    ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (cbr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = newPaymentProvider.VoltronProviderId, currency = "TRY", originalCurrency = "TRY", originalAmount = (cbr.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = result.ResponseDescription }));
                                    DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = Convert.ToInt64(cbr.Amount) * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = paymentProviderId, ProviderName = "Cepbank", ErrorMessage = status };
                                    Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                                }
                                transaction.Commit();
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Fatal("Complete Cepbank callback failed: " + ex.Message, ex);
                return result;
            }

            return result;
        }
        public DepositResult CompleteBPCallback(string domain, bool isProduction, string jsonRequest)
        {
            DepositResult result = new DepositResult { Success = false };

            try
            {
                dynamic request = JsonConvert.DeserializeObject(jsonRequest);
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        string refNo = request.referenceNo.ToString();
                        int companyId = 1;
                        if (request.referenceNo.ToString().Contains("~"))
                        {
                            companyId = Convert.ToInt32(refNo.Split('~')[0]);
                            refNo = refNo.Replace((companyId + "~"), string.Empty);
                        }
                        int refId = Convert.ToInt32(refNo.Split('-')[0]);

                        BPCRequest cr = Session.Load<BPCRequest>(refId, LockMode.Upgrade);

                        BPCPaymentMethodType bpcPaymentMethodType = BPCPaymentMethodTypeRepository.Get(cr.BPCPaymentMethodTypeId);
                        cr = BPCRequestRepository.GetAll().FirstOrDefault(w => w.Id == refId && w.StatusType == 1);

                        if (cr != null)
                        {
                            cr.CallbackData = jsonRequest;
                            cr.UpdateDate = DateTime.UtcNow;
                            Member member = MemberRepository.Get(cr.MemberId);

                            var vCompanyId = CompanyService.GetValue(companyId, "Voltron.CompanyId", isProduction);
                            string status = request.status.ToString().ToUpper();
                            if (status == "APPROVED")
                            {

                                var partnerUrlToDepositMoneyToAccount = CompanyService.GetValue(companyId, "Voltron.ServiceURL", isProduction);
                                var apiUsername = CompanyService.GetValue(companyId, "Voltron.APIUsername", isProduction);
                                var apiPassword = CompanyService.GetValue(companyId, "Voltron.APIPassword", isProduction);

                                long? bonusAmount = GetBonusAmount(companyId, bpcPaymentMethodType.VoltronProviderId, member.Id, cr.WithBonus, cr.Amount * 100);


                                var response2 =
                                                HttpServiceHelper.PostJsonRequest(partnerUrlToDepositMoneyToAccount + "DepositWithBonus",
                                                    JsonConvert.SerializeObject(new
                                                    {
                                                        CompanyId = vCompanyId,
                                                        ApiUsername = apiUsername,
                                                        ApiPassword = apiPassword,
                                                        Username = member.Username,
                                                        TransactionId = cr.Id,
                                                        ProviderTransactionReference = cr.ProviderRefId,
                                                        ProviderId = bpcPaymentMethodType.VoltronProviderId,
                                                        Amount = cr.Amount * 100,
                                                        BonusAmount = bonusAmount,
                                                        BonusId = cr.BonusId,
                                                        Checksum = ""
                                                    })
                                                    );


                                Logger.Debug("VResponse: " + JsonConvert.SerializeObject(response2));

                                bool balanceUpdateSuccessful = Convert.ToBoolean(response2.Success);
                                if (balanceUpdateSuccessful)
                                {
                                    cr.StatusType = 2;
                                    result.Success = true;
                                    result.Amount = Convert.ToInt64(cr.Amount * 100);

                                    try
                                    {
                                        ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (cr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = bpcPaymentMethodType.VoltronProviderId, currency = "TRY", originalCurrency = "TRY", originalAmount = (cr.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                                        NWWebHook.PushBackOffice(domain, "g:bo", "", "Player " + member.Username + " made a deposit with bestpay card of " + cr.Amount + " TL");
                                        MemberService.CheckLevelAfterDeposit(domain, member, isProduction);
                                        MemberService.GiveBonusAfterDepositForEligibleUsers(companyId, member.Username, isProduction, result.Amount, 4, bpcPaymentMethodType.VoltronProviderId);
                                        MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId, member.Username, result.Amount);
                                    }
                                    catch (Exception ex) { }
                                }
                                else
                                {
                                    result.ResponseDescription =
                                        "Bakiye güncellerken hata oluştu. Lütfen canlı chat'e bağlanın.";
                                    Logger.Fatal("Bakiye güncellerken hata oluştu, sebep: " + response2.Message +
                                                 ", obj: " + JsonConvert.SerializeObject(response2));


                                    ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (cr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = bpcPaymentMethodType.VoltronProviderId, currency = "TRY", originalCurrency = "TRY", originalAmount = (cr.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = result.ResponseDescription }));

                                    DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = Convert.ToInt64(cr.Amount) * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = bpcPaymentMethodType.VoltronProviderId, ProviderName = bpcPaymentMethodType.Name, ErrorMessage = result.ResponseDescription };
                                    Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                                }
                            }
                            else
                            {
                                cr.StatusType = -1;


                                ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (cr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = bpcPaymentMethodType.VoltronProviderId, currency = "TRY", originalCurrency = "TRY", originalAmount = (cr.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = status }));
                                DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = Convert.ToInt64(cr.Amount) * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = bpcPaymentMethodType.VoltronProviderId, ProviderName = bpcPaymentMethodType.Name, ErrorMessage = status };
                                Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                            }
                            transaction.Commit();
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Fatal("Complete bp callback failed: " + ex.Message, ex);
                return result;
            }

            return result;
        }
        public DepositResult CompleteJetonCallback(string domain, bool isProduction, string jsonRequest)
        {
            DepositResult result = new DepositResult { Success = false };
            int providerId = 61;

            try
            {
                JObject request = (JObject)JsonConvert.DeserializeObject(jsonRequest);
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        int orderId = request["orderId"].Value<int>();
                        int companyId = 1;
                        //if (request.referenceNo.ToString().Contains("~"))
                        //{
                        //    companyId = Convert.ToInt32(refNo.Split('~')[0]);
                        //    refNo = refNo.Replace((companyId + "~"), string.Empty);
                        //}
                        //int refId = Convert.ToInt32(refNo.Split('-')[0]);

                        JetRequest jr = Session.Load<JetRequest>(orderId, LockMode.Upgrade);

                        jr = JetRequestRepository.GetAll().FirstOrDefault(w => w.Id == orderId && w.StatusType == 0);

                        if (jr != null)
                        {
                            jr.CallbackData = jsonRequest;
                            jr.UpdateDate = DateTime.UtcNow;
                            jr.JetClientAccountNumber = request["customer"].Value<string>();
                            Member member = MemberRepository.Get(jr.MemberId);

                            var vCompanyId = CompanyService.GetValue(companyId, "Voltron.CompanyId", isProduction);
                            string status = request["status"].Value<string>().ToUpper();
                            if (status == "SUCCESS")
                            {

                                var partnerUrlToDepositMoneyToAccount = CompanyService.GetValue(companyId, "Voltron.ServiceURL", isProduction);
                                var apiUsername = CompanyService.GetValue(companyId, "Voltron.APIUsername", isProduction);
                                var apiPassword = CompanyService.GetValue(companyId, "Voltron.APIPassword", isProduction);

                                long? bonusAmount = GetBonusAmount(companyId, providerId, member.Id, jr.WithBonus, (long)jr.Amount * 100);


                                var response2 =
                                                HttpServiceHelper.PostJsonRequest(partnerUrlToDepositMoneyToAccount + "DepositWithBonus",
                                                    JsonConvert.SerializeObject(new
                                                    {
                                                        CompanyId = vCompanyId,
                                                        ApiUsername = apiUsername,
                                                        ApiPassword = apiPassword,
                                                        Username = member.Username,
                                                        TransactionId = jr.Id,
                                                        ProviderTransactionReference = jr.JetPaymentId,
                                                        ProviderId = providerId,
                                                        Amount = jr.Amount * 100,
                                                        BonusAmount = bonusAmount,
                                                        BonusId = jr.BonusId,
                                                        Checksum = ""
                                                    })
                                                    );


                                Logger.Debug("VResponse: " + JsonConvert.SerializeObject(response2));

                                bool balanceUpdateSuccessful = Convert.ToBoolean(response2.Success);
                                if (balanceUpdateSuccessful)
                                {
                                    jr.StatusType = 1;
                                    result.Success = true;
                                    result.Amount = Convert.ToInt64(jr.Amount * 100);

                                    try
                                    {
                                        ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (jr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (jr.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                                        NWWebHook.PushBackOffice(domain, "g:bo", "", "Player " + member.Username + " made a deposit with jdeposit card of " + jr.Amount + " TL");
                                        MemberService.CheckLevelAfterDeposit(domain, member, isProduction);
                                        MemberService.GiveBonusAfterDepositForEligibleUsers(companyId, member.Username, isProduction, result.Amount, 4, providerId);
                                        MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId, member.Username, result.Amount);
                                    }
                                    catch (Exception ex) { }
                                }
                                else
                                {
                                    result.ResponseDescription =
                                        "Bakiye güncellerken hata oluştu. Lütfen canlı chat'e bağlanın.";
                                    Logger.Fatal("Bakiye güncellerken hata oluştu, sebep: " + response2.Message +
                                                 ", obj: " + JsonConvert.SerializeObject(response2));


                                    ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (jr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (jr.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = result.ResponseDescription }));

                                    DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = Convert.ToInt64(jr.Amount) * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = providerId, ProviderName = "JDeposit", ErrorMessage = result.ResponseDescription };
                                    Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                                }
                            }
                            else
                            {
                                jr.StatusType = -1;

                                ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (jr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (jr.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = result.ResponseDescription }));
                                DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = Convert.ToInt64(jr.Amount) * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = providerId, ProviderName = "JDeposit", ErrorMessage = status };
                                Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                            }
                            transaction.Commit();
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Fatal("Complete jetonV3 callback failed: " + ex.Message, ex);
                return result;
            }

            return result;
        }
        public DepositResult StartDepositWithPaymino(string domain, int memberId, bool isProduction, int bankId, string name, string surname, long amount)
        {
            DepositResult result = new DepositResult { Success = false };

            try
            {
                int? companyId = CompanyService.CompanyId(domain);
                var serviceURL = CompanyService.GetValue(companyId.Value, "Paymino.ServiceURL", isProduction);
                var clientId = CompanyService.GetValue(companyId.Value, "Paymino.ClientId", isProduction);
                var clientSecret = CompanyService.GetValue(companyId.Value, "Paymino.ClientSecret", isProduction);
                Bank bank = BankRepository.Get(bankId);

                string authUrl = serviceURL + "/oauth/authorize?client_id=" + clientId + "&redirect_uri=" + HttpUtility.UrlEncode("https://www.baymavi.com") + "&response_type=code";
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(authUrl);
                myHttpWebRequest.Method = "GET";
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                var query = HttpUtility.ParseQueryString(myHttpWebResponse.ResponseUri.Query);
                var responseCode = query.Get("code");


                string queryParameters = string.Format("client_id={0}&client_secret={1}&code={2}&grant_type={3}&redirect_uri={4}", clientId, clientSecret, responseCode, "authorization_code", "https://www.baymavi.com");
                string requestUriString = string.Format("{0}/api/token", serviceURL);
                JObject accessTokenObj = JsonConvert.DeserializeObject<JObject>(NW.Helper.HttpHelper.PostRequest(requestUriString, queryParameters, "application/x-www-form-urlencoded"));
                string accessToken = accessTokenObj["access_token"].Value<string>();


                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        PayminoRequest request = new PayminoRequest();
                        request.MemberId = memberId;
                        request.Firstname = name;
                        request.Lastname = surname;
                        request.StatusType = (int)StatusType.Passive;
                        request.BankId = bankId;
                        request.Amount = amount;
                        request.CreateDate = DateTime.UtcNow;

                        request = PayminoRequestRepository.Insert(request);

                        string qs = string.Format("userId={0}&amount={1}&name={2}&surname={3}&username={4}&successurl={5}&declinedurl={6}&paymentMethod={7}&bankName={8}", memberId, amount, name, surname, memberId, HttpUtility.UrlEncode("https://www.baymavi.com/tr/payment/successpaymino?requestId=" + request.Id), HttpUtility.UrlEncode("https://www.baymavi.com/tr/payment/declinedpaymino?requestId=" + request.Id), "banktransfer", bank.Name);
                        string orderId = NW.Helper.HttpHelper.PostRequest(serviceURL + "/api/orders/createdirectorder?" + qs, string.Empty, "application/x-www-form-urlencoded", new NameValueCollection() { { "Authorization", ("Bearer " + accessToken) } }).Trim('"');

                        int intOrderId;
                        if (int.TryParse(orderId, out intOrderId))
                        {
                            request.PaymentProviderTransactionId = intOrderId;
                            PayminoRequestRepository.Update(request);
                            result.Success = true;
                        }
                        else
                        {
                            result.ResponseDescription = orderId;
                        }
                        transaction.Commit();

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Fatal("Complete paymino callback failed: " + ex.Message, ex);
                return result;
            }

            return result;
        }
        public DepositResult CompletePayminoCallback(string domain, bool isProduction, int requestId)
        {
            DepositResult result = new DepositResult { Success = false };
            int providerId = 47;

            try
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        //CashlinkRequest lockedCashlinkRequest = Session.CreateSQLQuery("SELECT * FROM CashlinkRequest WITH(UPDLOCK, ROWLOCK) WHERE Id = " + id).AddEntity(typeof(CashlinkRequest)).SetCacheable(true).SetCacheMode(CacheMode.Refresh).List<CashlinkRequest>().FirstOrDefault();
                        PayminoRequest lockedPayminoRequest = Session.Load<PayminoRequest>(requestId, LockMode.Upgrade);

                        PayminoRequest pr = PayminoRequestRepository.GetAll().FirstOrDefault(w => w.Id == requestId && w.StatusType == 0);


                        Member member = MemberRepository.Get(pr.MemberId);
                        int? companyId = CompanyService.CompanyId(domain);
                        var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                        if (pr != null)
                        {
                            pr.UpdateDate = DateTime.UtcNow;

                            var partnerUrlToDepositMoneyToAccount = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                            var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                            var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                            //long? bonusAmount = GetBonusAmount(member.Id, pr.WithBonus, pr.Amount * 100);

                            var response2 =
                                            HttpServiceHelper.PostJsonRequest(partnerUrlToDepositMoneyToAccount + "deposit",
                                                JsonConvert.SerializeObject(new
                                                {
                                                    CompanyId = vCompanyId,
                                                    ApiUsername = apiUsername,
                                                    ApiPassword = apiPassword,
                                                    Username = member.Username,
                                                    TransactionId = pr.Id,
                                                    ProviderTransactionReference = pr.PaymentProviderTransactionId,
                                                    ProviderId = providerId,
                                                    Amount = pr.Amount * 100,
                                                    Checksum = ""
                                                })
                                                );
                            Logger.Debug("VResponse: " + JsonConvert.SerializeObject(response2));

                            bool balanceUpdateSuccessful = Convert.ToBoolean(response2.Success);
                            if (balanceUpdateSuccessful)
                            {
                                pr.StatusType = 1;
                                result.Success = true;
                                result.Amount = Convert.ToInt64(pr.Amount * 100);

                                ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (pr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (pr.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                                NWWebHook.PushBackOffice(domain, "g:bo", "", "Player " + member.Username + " made a deposit with bank transfer (Paymino) of " + pr.Amount + " TL");

                                MemberService.CheckLevelAfterDeposit(domain, member, isProduction);
                                MemberService.GiveBonusAfterDepositForEligibleUsers(companyId.Value, member.Username, isProduction, Convert.ToInt64(pr.Amount * 100), 11015, 47);
                                MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId.Value, member.Username, Convert.ToInt64(pr.Amount * 100));
                            }
                            else
                            {
                                result.ResponseDescription =
                                    "Bakiye güncellerken hata oluştu. Lütfen canlı chat'e bağlanın.";
                                Logger.Fatal("Bakiye güncellerken hata oluştu, sebep: " + response2.Message +
                                             ", obj: " + JsonConvert.SerializeObject(response2));



                                ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (pr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (pr.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = result.ResponseDescription }));
                                DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = Convert.ToInt64(pr.Amount) * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = 47, ProviderName = "Paymino", ErrorMessage = result.ResponseDescription };
                                Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                            }
                        }
                        else
                        {
                            pr.StatusType = -2;
                        }
                        PayminoRequestRepository.Update(pr);
                        transaction.Commit();

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Fatal("Complete bp callback failed: " + ex.Message, ex);
                return result;
            }

            return result;
        }
        public DepositResult DeclinePayminoCallback(string domain, bool isProduction, int requestId)
        {
            DepositResult result = new DepositResult { Success = false };
            int providerId = 57;

            try
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        PayminoRequest pr = PayminoRequestRepository.Get(requestId);
                        pr.StatusType = -1;
                        pr.UpdateDate = DateTime.UtcNow;
                        PayminoRequestRepository.Update(pr);
                        transaction.Commit();

                        Member member = MemberRepository.Get(pr.MemberId);
                        int? companyId = CompanyService.CompanyId(domain);
                        var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);

                        ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (pr.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (pr.Amount / 100m).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = "Declined" }));
                        DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = Convert.ToInt64(pr.Amount) * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = 47, ProviderName = "Paymino", ErrorMessage = "Desclined" };
                        Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Fatal("Complete bp callback failed: " + ex.Message, ex);
                return result;
            }

            return result;
        }

        public DepositResult StartDepositWithPaymino2(string domain, int memberId, bool isProduction, string name, string surname, long amount, bool withBonus, int? bonusId, string userIp)
        {
            DepositResult result = new DepositResult { Success = false };

            try
            {
                int? companyId = CompanyService.CompanyId(domain);
                var handlerURL = "https://api.clientpaymentz.com/handler";//CompanyService.GetValue(companyId.Value, "Paymino.ServiceURL", isProduction);
                var merchantId = CompanyService.GetValue(companyId.Value, "Paymino.MerchantId", isProduction);
                var clientSecret = CompanyService.GetValue(companyId.Value, "Paymino.SecretKey", isProduction);

                Member member = MemberService.GetMember(memberId);

                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        PayminoRequest2 request = new PayminoRequest2();
                        request.MemberId = memberId;
                        request.Firstname = name;
                        request.Lastname = surname;
                        request.StatusType = (int)StatusType.Passive;
                        request.RequestType = 1; //TODO
                        request.PaymentMethod = 1; //TODO
                        request.Amount = amount * 100;
                        request.RecognisedAmount = request.Amount;
                        request.CreateDate = DateTime.UtcNow;
                        request.WithBonus = withBonus;
                        request.BonusId = bonusId;

                        request = PayminoRequest2Repository.Insert(request);
                        string address = "address";
                        string city = "city";
                        string country = "TR";
                        string birthdate = "1980-02-01T00:00:00";
                        string phoneNumber = "+905555555555";
                        string zipCode = "34343";
                        string locale = "tr";
                        string notificationUrl = "https://www.maviappcontent.com/tr/payment/payminocallback";
                        userIp = "127.0.0.1";
                        string email = member.Id.ToString() + "@baymavi.com";
                        string firstName = member.FirstName;
                        string lastName = member.LastName;
                        string data = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}banktransferpay-in{17}", clientSecret, address, city, country, member.Id.ToString(), birthdate, email, firstName, lastName, phoneNumber, zipCode, locale, merchantId, notificationUrl, amount.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture), member.Currency.ToUpper(), request.Id.ToString(), userIp);
                        string signature = SecurityHelper.SHA256Encryption(data);

                        var requestObj = new
                        {
                            signature = signature,
                            data = new
                            {
                                request_type = "pay-in",
                                merchant_ref = merchantId,
                                pay = new
                                {
                                    eps_tran_ref = request.Id.ToString(),
                                    payment_method = "banktransfer",
                                    amount = amount.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture),
                                    currency = member.Currency.ToUpper(),
                                },
                                customer = new
                                {
                                    customer_ref = member.Id.ToString(),
                                    first_name = firstName,
                                    last_name = lastName,
                                    dob = birthdate,
                                    email = email,
                                    phone = phoneNumber,
                                    country = country,
                                    region = "",
                                    city = city,
                                    address = address,
                                    zip = zipCode,
                                },
                                notification_url = notificationUrl,
                                requestor_ip = userIp,
                                locale = locale

                            },
                        };


                        string payminoResult = NW.Helper.HttpHelper.PostRequest(handlerURL, JsonConvert.SerializeObject(requestObj), "application/json");


                        if (!string.IsNullOrEmpty(payminoResult))
                        {

                            //TODO response code ve signature check
                            try
                            {
                                JObject jObjectResult = (JObject)JsonConvert.DeserializeObject(payminoResult);
                                transaction.Commit();
                                result = new DepositResult
                                {
                                    Success = true,
                                    Amount = Convert.ToInt64(request.Amount),
                                    RedirectURL = ((JObject)jObjectResult["data"])["redirect_url"].Value<string>(),
                                    ResponseDescription = "Redirect"
                                };
                            }
                            catch (Exception ex)
                            {
                                Logger.Fatal("start deposit with paymino failed: " + ex.Message, ex);
                                return result;
                            }
                        }



                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Fatal("start deposit with paymino failed: " + ex.Message, ex);
                return result;
            }

            return result;
        }

        public DepositResult CompletePayminoCallback2(string domain, bool isProduction, int companyId, string jsonRequest)
        {
            DepositResult result = new DepositResult { Success = false };

            int providerId = 57;
            try
            {
                JObject request = (JObject)JsonConvert.DeserializeObject(jsonRequest);
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        string requestSignature = request["signature"].Value<string>();
                        JObject data = ((JObject)request["data"]);

                        var merchantId = CompanyService.GetValue(companyId, "Paymino.MerchantId", isProduction);
                        var clientSecret = CompanyService.GetValue(companyId, "Paymino.SecretKey", isProduction);

                        Dictionary<string, string> dataValues = new Dictionary<string, string>();
                        foreach (var x in data)
                        {
                            if (x.Key == "amount")
                            {
                                dataValues.Add(x.Key, x.Value.Value<float>().ToString("0.0", System.Globalization.CultureInfo.InvariantCulture));
                            }
                            else if (x.Key == "status_timestamp")
                            {
                                dataValues.Add(x.Key, x.Value.Value<DateTime>().ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                            }
                            else
                            {
                                dataValues.Add(x.Key, x.Value.Value<string>());
                            }
                        }
                        List<string> values = dataValues.OrderBy(v => v.Key).Select(v => v.Value).ToList();
                        string signature = SecurityHelper.SHA256Encryption(clientSecret + String.Join("", values));

                        if (requestSignature == signature)
                        {
                            string requestMerchantId = data["merchant_ref"].Value<string>();
                            if (requestMerchantId == merchantId)
                            {


                                string status = data["status"].Value<string>();

                                Member member = MemberRepository.Get(data["customer_acc_ref"].Value<int>());
                                PayminoRequest2 lockedPr = Session.Load<PayminoRequest2>(data["eps_tran_ref"].Value<int>(), LockMode.Upgrade);

                                var vCompanyId = CompanyService.GetValue(companyId, "Voltron.CompanyId", isProduction);
                                if (member.CompanyId == companyId)
                                {
                                    if (member != null)
                                    {
                                        PayminoRequest2 pr = PayminoRequest2Repository.GetAll().FirstOrDefault(p => p.Id == data["eps_tran_ref"].Value<int>() && p.StatusType == (int)PaymentStatusType.Pending);
                                        if (pr != null)
                                        {
                                            if (pr.MemberId == member.Id)
                                            {
                                                pr.UpdateDate = DateTime.UtcNow;
                                                if (status == "ACCEPTED")
                                                {
                                                    pr.PaymentProviderTransactionId = data["pspis_tran_ref"].Value<int>();
                                                    //pr.UpdateDate = data["status_timestamp"].Value<DateTime>();

                                                }
                                                else if (status == "CAPTURED")
                                                {

                                                    var partnerUrlToDepositMoneyToAccount = CompanyService.GetValue(companyId, "Voltron.ServiceURL", isProduction);
                                                    var apiUsername = CompanyService.GetValue(companyId, "Voltron.APIUsername", isProduction);
                                                    var apiPassword = CompanyService.GetValue(companyId, "Voltron.APIPassword", isProduction);

                                                    long? bonusAmount = GetBonusAmount(companyId, providerId, member.Id, pr.WithBonus, pr.Amount);
                                                    var response2 =
                                                                    HttpServiceHelper.PostJsonRequest(partnerUrlToDepositMoneyToAccount + "DepositWithBonus",
                                                                        JsonConvert.SerializeObject(new
                                                                        {
                                                                            CompanyId = vCompanyId,
                                                                            ApiUsername = apiUsername,
                                                                            ApiPassword = apiPassword,
                                                                            Username = member.Username,
                                                                            TransactionId = pr.Id,
                                                                            ProviderTransactionReference = pr.PaymentProviderTransactionId,
                                                                            ProviderId = providerId,
                                                                            Amount = pr.Amount,
                                                                            BonusAmount = bonusAmount,
                                                                            BonusId = pr.BonusId,
                                                                            Checksum = ""
                                                                        })
                                                                        );
                                                    Logger.Debug("VResponse: " + JsonConvert.SerializeObject(response2));

                                                    bool balanceUpdateSuccessful = Convert.ToBoolean(response2.Success);
                                                    if (balanceUpdateSuccessful)
                                                    {
                                                        pr.StatusType = 1;
                                                        result.Success = true;
                                                        result.Amount = pr.Amount;



                                                        ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (pr.Amount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (pr.Amount / 100m).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                                                        NWWebHook.PushBackOffice(domain, "g:bo", "", "Player " + member.Username + " made a deposit with bank transfer (Paymino) of " + (pr.Amount * 0.01) + " TL");
                                                        MemberService.CheckLevelAfterDeposit(domain, member, isProduction);
                                                        MemberService.GiveBonusAfterDepositForEligibleUsers(companyId, member.Username, isProduction, pr.Amount, 11015, providerId);
                                                        MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId, member.Username, pr.Amount);
                                                    }
                                                    else
                                                    {
                                                        result.ResponseDescription =
                                                            "Bakiye güncellerken hata oluştu. Lütfen canlı chat'e bağlanın.";
                                                        Logger.Fatal("Bakiye güncellerken hata oluştu, sebep: " + response2.Message +
                                                                     ", obj: " + JsonConvert.SerializeObject(response2));



                                                        ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (pr.Amount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (pr.Amount / 100m).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = result.ResponseDescription }));

                                                        DepositFailedKafkaModel depositFailedKafkaModel = new DepositFailedKafkaModel() { CompanyId = Convert.ToInt32(vCompanyId), Amount = pr.Amount * 100, UnformattedUsername = member.Username, MemberId = member.Id, Username = member.Username, ProviderId = providerId, ProviderName = "Paymino", ErrorMessage = result.ResponseDescription };
                                                        Task.Run(() => { KafkaHelper.InsertQueue(ConfigurationManager.AppSettings["KafkaConfig"], "DepositFailed", 0, JsonConvert.SerializeObject(depositFailedKafkaModel)); });
                                                    }

                                                    pr.StatusType = (int)PaymentStatusType.Approved;
                                                }
                                                else if (status == "DECLINED")
                                                {
                                                    pr.StatusType = (int)PaymentStatusType.Rejected;


                                                    ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (pr.Amount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (pr.Amount / 100m).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = status }));
                                                }
                                                pr.ResponseJson = jsonRequest;
                                                PayminoRequest2Repository.Update(pr);
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
            catch (Exception ex)
            {
                Logger.Fatal("Complete paymino callback failed: " + ex.Message, ex);
                return result;
            }

            return result;
        }


        public IList<PayminoRequest2> PendingPayminoRequest2List(int memberId)
        {

            return PayminoRequest2Repository.GetAll().Where(pr => pr.MemberId == memberId && pr.StatusType == (int)PaymentStatusType.Pending).ToList();
        }

        public DepositResult DepositMoneyWithAdjustmentTypeId(string domain, int memberId, bool isProduction, decimal bonusAmount, decimal amountAllocatedForBonus, int transactionTypeId, int transactionId)
        {

            DepositResult result = new DepositResult { Success = false };

            try
            {

                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        int? companyId = CompanyService.CompanyId(domain);
                        var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                        var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                        var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                        var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);
                        var methodName = "DepositWithTransactionTypeId";

                        Member member = MemberRepository.Get(memberId);

                        var response2 =
                            HttpServiceHelper.PostJsonRequest(partnerUrl + methodName,
                                JsonConvert.SerializeObject(new
                                {
                                    CompanyId = vCompanyId,
                                    ApiUsername = apiUsername,
                                    ApiPassword = apiPassword,
                                    Username = member.Username,
                                    TransactionId = transactionId, //transactionId,
                                    ProviderTransactionReference = transactionId,
                                    TransactionTypeId = transactionTypeId,
                                    ProviderId = 6, // bonus engine
                                    BonusAmount = Convert.ToInt64(bonusAmount * 100),
                                    AmountAllocatedForBonus = Convert.ToInt64(amountAllocatedForBonus * 100),
                                    Checksum = ""
                                })
                            );

                        Logger.Debug("VResponse: " + JsonConvert.SerializeObject(response2));

                        bool balanceUpdateSuccessful = Convert.ToBoolean(response2.Success);
                        if (balanceUpdateSuccessful)
                        {
                            result.Success = true;
                            result.Amount = Convert.ToInt64(bonusAmount * 100);
                        }
                        else
                        {
                            result.ResponseDescription = response2.Message;
                            Logger.Fatal("Bonus ekleme hata: " + response2.Message + ", obj: " + JsonConvert.SerializeObject(response2));
                        }
                        uniOfWork.Commit(transaction);

                        MemberService.CheckLevelAfterDeposit(domain, member, isProduction);
                        MemberService.GiveBonusAfterDepositForEligibleUsers(companyId.Value, member.Username, isProduction, result.Amount, transactionTypeId, 6);
                        MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId.Value, member.Username, result.Amount);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("Complete custom deposit callback failed: " + ex.Message, ex);
                return result;
            }

            return result;
        }

        public DepositResult WithdrawWithPapara(string domain, string username, bool isProduction, string paparaAccountNumber, decimal amount, string currency)
        {
            DepositResult result = new DepositResult { Success = false };

            long voltronAmount = (long)(amount * 100);

            PaparaAdditionalInfo additionalInfo = new PaparaAdditionalInfo() { PaparaAccountNumber = paparaAccountNumber, Amount = voltronAmount };
            int providerId = 49; //papara
            try
            {

                Member member = MemberService.GetActiveMember(domain, username);

                DepositResult depositResult = new DepositResult();


                int? companyId = CompanyService.CompanyId(domain);

                var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                var withdrawRequest = "WithdrawRequestWithProviderId";//int companyId, string apiUsername, string username, long withdrawAmount, long partnerReferenceId, string checksum

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                string addtionalInfoJSON = JsonConvert.SerializeObject(additionalInfo);

                var request = new RestRequest(withdrawRequest, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", username); // adds to POST or URL querystring based on Method
                request.AddParameter("providerId", providerId); // adds to POST or URL querystring based on Method // 49 papara
                request.AddParameter("withdrawAmount", voltronAmount); // adds to POST or URL querystring based on Method
                request.AddParameter("additionalInfo", addtionalInfoJSON); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username, providerId.ToString(), voltronAmount.ToString(), addtionalInfoJSON)); // adds to POST or URL querystring based on Method
                                                                                                                                                                                        //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                depositResult.Success = response.StatusCode == HttpStatusCode.OK && Convert.ToBoolean(response.Data["Success"]);
                if (depositResult.Success)
                {
                    ServiceBusHelper.InsertQueue(isProduction, "withdrawrequested", JsonConvert.SerializeObject(new { amount = (additionalInfo.Amount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (additionalInfo.Amount / 100m).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                    depositResult.TransactionId = response.Data["Message"];

                    //Task.Run(() => SendSMSAsync(member.Username, amount, Logger));
                }
                else
                {
                    Logger.Fatal("Failed updating voltron: " + response.Data["Message"]);
                    depositResult.ResponseDescription = response.Data["Message"];
                }


                result = new DepositResult
                {
                    Success = depositResult.Success,
                    Amount = Convert.ToInt64(amount),
                    //MerchantRef = ecoRequest.Id.ToString(),
                    TransactionId = depositResult.TransactionId,
                    RedirectURL = string.Empty,
                    ResponseDescription = string.Empty
                };

            }
            catch (Exception ex)
            {
                Logger.Fatal("Inserting failed papara request internally / withdraw." + ex.Message, ex);
                result.Amount = 0;
                result.ResponseDescription = "Err: Internal error while processing your Papara withdraw. Please contact live chat.";
            }

            return result;
        }

        public DepositResult WithdrawWithCMT(string domain, string username, bool isProduction, string cmtAccountNumber, decimal amount, string currency)
        {
            DepositResult result = new DepositResult { Success = false };

            long voltronAmount = (long)(amount * 100);

            CMTAdditionalInfo additionalInfo = new CMTAdditionalInfo() { CMTAccountNumber = cmtAccountNumber, Amount = voltronAmount };
            int providerId = 74; //papara
            try
            {

                Member member = MemberService.GetActiveMember(domain, username);

                DepositResult depositResult = new DepositResult();


                int? companyId = CompanyService.CompanyId(domain);

                var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                var withdrawRequest = "WithdrawRequestWithProviderId";//int companyId, string apiUsername, string username, long withdrawAmount, long partnerReferenceId, string checksum

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                string addtionalInfoJSON = JsonConvert.SerializeObject(additionalInfo);

                var request = new RestRequest(withdrawRequest, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", username); // adds to POST or URL querystring based on Method
                request.AddParameter("providerId", providerId); // adds to POST or URL querystring based on Method // 49 papara
                request.AddParameter("withdrawAmount", voltronAmount); // adds to POST or URL querystring based on Method
                request.AddParameter("additionalInfo", addtionalInfoJSON); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username, providerId.ToString(), voltronAmount.ToString(), addtionalInfoJSON)); // adds to POST or URL querystring based on Method
                                                                                                                                                                                        //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                depositResult.Success = response.StatusCode == HttpStatusCode.OK && Convert.ToBoolean(response.Data["Success"]);
                if (depositResult.Success)
                {
                    ServiceBusHelper.InsertQueue(isProduction, "withdrawrequested", JsonConvert.SerializeObject(new { amount = (voltronAmount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (voltronAmount / 100m).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                    depositResult.TransactionId = response.Data["Message"];

                    //Task.Run(() => SendSMSAsync(member.Username, amount, Logger));
                }
                else
                {
                    Logger.Fatal("Failed updating voltron: " + response.Data["Message"]);
                    depositResult.ResponseDescription = response.Data["Message"];
                }


                result = new DepositResult
                {
                    Success = depositResult.Success,
                    Amount = Convert.ToInt64(amount),
                    //MerchantRef = ecoRequest.Id.ToString(),
                    TransactionId = depositResult.TransactionId,
                    RedirectURL = string.Empty,
                    ResponseDescription = string.Empty
                };

            }
            catch (Exception ex)
            {
                Logger.Fatal("Inserting failed papara request internally / withdraw." + ex.Message, ex);
                result.Amount = 0;
                result.ResponseDescription = "Err: Internal error while processing your Papara withdraw. Please contact live chat.";
            }

            return result;
        }
        public DepositResult WithdrawWithMefete(string domain, string username, bool isProduction, string mefeteAccountNumber, decimal amount, string currency)
        {
            DepositResult result = new DepositResult { Success = false };

            long voltronAmount = (long)(amount * 100);

            MefeteAdditionalInfo additionalInfo = new MefeteAdditionalInfo() { MefeteAccountNumber = mefeteAccountNumber, Amount = voltronAmount };
            int providerId = 74; //papara
            try
            {

                Member member = MemberService.GetActiveMember(domain, username);

                DepositResult depositResult = new DepositResult();


                int? companyId = CompanyService.CompanyId(domain);

                var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                var withdrawRequest = "WithdrawRequestWithProviderId";//int companyId, string apiUsername, string username, long withdrawAmount, long partnerReferenceId, string checksum

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                string addtionalInfoJSON = JsonConvert.SerializeObject(additionalInfo);

                var request = new RestRequest(withdrawRequest, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", username); // adds to POST or URL querystring based on Method
                request.AddParameter("providerId", providerId); // adds to POST or URL querystring based on Method // 49 papara
                request.AddParameter("withdrawAmount", voltronAmount); // adds to POST or URL querystring based on Method
                request.AddParameter("additionalInfo", addtionalInfoJSON); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username, providerId.ToString(), voltronAmount.ToString(), addtionalInfoJSON)); // adds to POST or URL querystring based on Method
                                                                                                                                                                                        //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                depositResult.Success = response.StatusCode == HttpStatusCode.OK && Convert.ToBoolean(response.Data["Success"]);
                if (depositResult.Success)
                {
                    ServiceBusHelper.InsertQueue(isProduction, "withdrawrequested", JsonConvert.SerializeObject(new { amount = (voltronAmount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (voltronAmount / 100m).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                    depositResult.TransactionId = response.Data["Message"];

                    //Task.Run(() => SendSMSAsync(member.Username, amount, Logger));
                }
                else
                {
                    Logger.Fatal("Failed updating voltron: " + response.Data["Message"]);
                    depositResult.ResponseDescription = response.Data["Message"];
                }


                result = new DepositResult
                {
                    Success = depositResult.Success,
                    Amount = Convert.ToInt64(amount),
                    //MerchantRef = ecoRequest.Id.ToString(),
                    TransactionId = depositResult.TransactionId,
                    RedirectURL = string.Empty,
                    ResponseDescription = string.Empty
                };

            }
            catch (Exception ex)
            {
                Logger.Fatal("Inserting failed papara request internally / withdraw." + ex.Message, ex);
                result.Amount = 0;
                result.ResponseDescription = "Err: Internal error while processing your Papara withdraw. Please contact live chat.";
            }

            return result;
        }
        public DepositResult WithdrawWithParazula(string domain, string username, bool isProduction, string parazulaAccountNumber, decimal amount, string currency)
        {
            DepositResult result = new DepositResult { Success = false };

            long voltronAmount = (long)(amount * 100);

            ParazulaAdditionalInfo additionalInfo = new ParazulaAdditionalInfo() { ParazulaAccountNumber = parazulaAccountNumber, Amount = voltronAmount };
            int providerId = 74; //papara
            try
            {

                Member member = MemberService.GetActiveMember(domain, username);

                DepositResult depositResult = new DepositResult();


                int? companyId = CompanyService.CompanyId(domain);

                var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                var withdrawRequest = "WithdrawRequestWithProviderId";//int companyId, string apiUsername, string username, long withdrawAmount, long partnerReferenceId, string checksum

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                string addtionalInfoJSON = JsonConvert.SerializeObject(additionalInfo);

                var request = new RestRequest(withdrawRequest, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", username); // adds to POST or URL querystring based on Method
                request.AddParameter("providerId", providerId); // adds to POST or URL querystring based on Method // 49 papara
                request.AddParameter("withdrawAmount", voltronAmount); // adds to POST or URL querystring based on Method
                request.AddParameter("additionalInfo", addtionalInfoJSON); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username, providerId.ToString(), voltronAmount.ToString(), addtionalInfoJSON)); // adds to POST or URL querystring based on Method
                                                                                                                                                                                        //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                depositResult.Success = response.StatusCode == HttpStatusCode.OK && Convert.ToBoolean(response.Data["Success"]);
                if (depositResult.Success)
                {
                    depositResult.TransactionId = response.Data["Message"];

                    //Task.Run(() => SendSMSAsync(member.Username, amount, Logger));
                }
                else
                {
                    Logger.Fatal("Failed updating voltron: " + response.Data["Message"]);
                    depositResult.ResponseDescription = response.Data["Message"];
                }


                result = new DepositResult
                {
                    Success = depositResult.Success,
                    Amount = Convert.ToInt64(amount),
                    //MerchantRef = ecoRequest.Id.ToString(),
                    TransactionId = depositResult.TransactionId,
                    RedirectURL = string.Empty,
                    ResponseDescription = string.Empty
                };

            }
            catch (Exception ex)
            {
                Logger.Fatal("Inserting failed papara request internally / withdraw." + ex.Message, ex);
                result.Amount = 0;
                result.ResponseDescription = "Err: Internal error while processing your Papara withdraw. Please contact live chat.";
            }

            return result;
        }

        public DepositResult WithdrawWithPEP(string domain, string username, bool isProduction, string pepAccountNumber, decimal amount, string currency)
        {
            DepositResult result = new DepositResult { Success = false };

            long voltronAmount = (long)(amount * 100);

            PEPAdditionalInfo additionalInfo = new PEPAdditionalInfo() { PEPAccountNumber = pepAccountNumber, Amount = voltronAmount };
            int providerId = 90; //papara
            try
            {

                Member member = MemberService.GetActiveMember(domain, username);

                DepositResult depositResult = new DepositResult();


                int? companyId = CompanyService.CompanyId(domain);

                var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                var withdrawRequest = "WithdrawRequestWithProviderId";//int companyId, string apiUsername, string username, long withdrawAmount, long partnerReferenceId, string checksum

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                string addtionalInfoJSON = JsonConvert.SerializeObject(additionalInfo);

                var request = new RestRequest(withdrawRequest, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", username); // adds to POST or URL querystring based on Method
                request.AddParameter("providerId", providerId); // adds to POST or URL querystring based on Method // 49 papara
                request.AddParameter("withdrawAmount", voltronAmount); // adds to POST or URL querystring based on Method
                request.AddParameter("additionalInfo", addtionalInfoJSON); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username, providerId.ToString(), voltronAmount.ToString(), addtionalInfoJSON)); // adds to POST or URL querystring based on Method
                                                                                                                                                                                        //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                depositResult.Success = response.StatusCode == HttpStatusCode.OK && Convert.ToBoolean(response.Data["Success"]);
                if (depositResult.Success)
                {
                    ServiceBusHelper.InsertQueue(isProduction, "withdrawrequested", JsonConvert.SerializeObject(new { amount = (voltronAmount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (voltronAmount / 100m).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                    depositResult.TransactionId = response.Data["Message"];

                    //Task.Run(() => SendSMSAsync(member.Username, amount, Logger));
                }
                else
                {
                    Logger.Fatal("Failed updating voltron: " + response.Data["Message"]);
                    depositResult.ResponseDescription = response.Data["Message"];
                }


                result = new DepositResult
                {
                    Success = depositResult.Success,
                    Amount = Convert.ToInt64(amount),
                    //MerchantRef = ecoRequest.Id.ToString(),
                    TransactionId = depositResult.TransactionId,
                    RedirectURL = string.Empty,
                    ResponseDescription = string.Empty
                };

            }
            catch (Exception ex)
            {
                Logger.Fatal("Inserting failed papara request internally / withdraw." + ex.Message, ex);
                result.Amount = 0;
                result.ResponseDescription = "Err: Internal error while processing your Papara withdraw. Please contact live chat.";
            }

            return result;
        }
        public DepositResult WithdrawWithPayfix(string domain, string username, bool isProduction, string payfixAccountNumber, decimal amount, string currency)
        {
            DepositResult result = new DepositResult { Success = false };

            long voltronAmount = (long)(amount * 100);

            PayfixAdditionalInfo additionalInfo = new PayfixAdditionalInfo() { PayfixAccountNumber = payfixAccountNumber, Amount = voltronAmount };
            int providerId = 96; //papara
            try
            {

                Member member = MemberService.GetActiveMember(domain, username);

                DepositResult depositResult = new DepositResult();


                int? companyId = CompanyService.CompanyId(domain);

                var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                var withdrawRequest = "WithdrawRequestWithProviderId";//int companyId, string apiUsername, string username, long withdrawAmount, long partnerReferenceId, string checksum

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                string addtionalInfoJSON = JsonConvert.SerializeObject(additionalInfo);

                var request = new RestRequest(withdrawRequest, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", username); // adds to POST or URL querystring based on Method
                request.AddParameter("providerId", providerId); // adds to POST or URL querystring based on Method // 49 papara
                request.AddParameter("withdrawAmount", voltronAmount); // adds to POST or URL querystring based on Method
                request.AddParameter("additionalInfo", addtionalInfoJSON); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username, providerId.ToString(), voltronAmount.ToString(), addtionalInfoJSON)); // adds to POST or URL querystring based on Method
                                                                                                                                                                                        //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                depositResult.Success = response.StatusCode == HttpStatusCode.OK && Convert.ToBoolean(response.Data["Success"]);
                if (depositResult.Success)
                {
                    ServiceBusHelper.InsertQueue(isProduction, "withdrawrequested", JsonConvert.SerializeObject(new { amount = (voltronAmount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (voltronAmount / 100m).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                    depositResult.TransactionId = response.Data["Message"];

                    //Task.Run(() => SendSMSAsync(member.Username, amount, Logger));
                }
                else
                {
                    Logger.Fatal("Failed updating voltron: " + response.Data["Message"]);
                    depositResult.ResponseDescription = response.Data["Message"];
                }


                result = new DepositResult
                {
                    Success = depositResult.Success,
                    Amount = Convert.ToInt64(amount),
                    //MerchantRef = ecoRequest.Id.ToString(),
                    TransactionId = depositResult.TransactionId,
                    RedirectURL = string.Empty,
                    ResponseDescription = string.Empty
                };

            }
            catch (Exception ex)
            {
                Logger.Fatal("Inserting failed papara request internally / withdraw." + ex.Message, ex);
                result.Amount = 0;
                result.ResponseDescription = "Err: Internal error while processing your Papara withdraw. Please contact live chat.";
            }

            return result;
        }
        public WithdrawResult WithdrawWithJeton(string domain, string username, bool isProduction, string jetonAccountNumber, decimal amount, string currency)
        {
            WithdrawResult result = new WithdrawResult { Success = false };

            long voltronAmount = (long)(amount * 100);

            JetonAdditionalInfo additionalInfo = new JetonAdditionalInfo() { JetonAccountNumber = jetonAccountNumber, Amount = voltronAmount, Currency = currency };
            int providerId = 61; //jetmethod
            try
            {

                Member member = MemberService.GetActiveMember(domain, username);

                DepositResult depositResult = new DepositResult();


                int? companyId = CompanyService.CompanyId(domain);

                var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                var withdrawRequest = "WithdrawRequestWithProviderId";//int companyId, string apiUsername, string username, long withdrawAmount, long partnerReferenceId, string checksum

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                string addtionalInfoJSON = JsonConvert.SerializeObject(additionalInfo);

                var request = new RestRequest(withdrawRequest, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", username); // adds to POST or URL querystring based on Method
                request.AddParameter("providerId", providerId); // adds to POST or URL querystring based on Method // 45 ecopayz
                request.AddParameter("withdrawAmount", voltronAmount); // adds to POST or URL querystring based on Method
                request.AddParameter("additionalInfo", addtionalInfoJSON); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username, providerId.ToString(), voltronAmount.ToString(), addtionalInfoJSON)); // adds to POST or URL querystring based on Method
                                                                                                                                                                                        //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                depositResult.Success = response.StatusCode == HttpStatusCode.OK && Convert.ToBoolean(response.Data["Success"]);
                if (depositResult.Success)
                {
                    ServiceBusHelper.InsertQueue(isProduction, "withdrawrequested", JsonConvert.SerializeObject(new { amount = (voltronAmount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (voltronAmount / 100m).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                    depositResult.TransactionId = response.Data["Message"];

                    //Task.Run(() => SendSMSAsync(member.Username, amount, Logger));
                }
                else
                {
                    Logger.Fatal("Failed updating voltron: " + response.Data["Message"]);
                    depositResult.ResponseDescription = response.Data["Message"];
                }


                result = new WithdrawResult
                {
                    Success = true,
                    Amount = Convert.ToInt64(amount),
                    RedirectURL = string.Empty,
                    ResponseDescription = string.Empty
                };

            }
            catch (Exception ex)
            {
                Logger.Fatal("Inserting failed ecopayz request internally / withdraw." + ex.Message, ex);
                result.Amount = 0;
                result.ResponseDescription = "Err: Internal error while processing your EcoPayz withdraw. Please contact live chat.";
            }

            return result;
        }
        public DepositResult DepositWithPapara(string domain, string username, bool isProduction, string firstName, string lastName, string paparaAccountNumber, decimal amount, string currency)
        {
            DepositResult depositResult = new DepositResult { Success = false };


            return depositResult;
        }
        public DepositResult DepositWithTrustpay(string domain, string username, bool isProduction, string firstName, string lastName, string userId, string processId, decimal amount, string currency)
        {
            DepositResult result = new DepositResult { Success = false };
            try
            {
                using (var uniOfWork = UnitOfWork.Current)
                {

                    int? comapanyId = CompanyService.CompanyId(domain);
                    string appKey = CompanyService.GetValue(comapanyId.Value, "Trustpay.AppKey", isProduction);
                    string secretKey = CompanyService.GetValue(comapanyId.Value, "Trustpay.SecretKey", isProduction);
                    string url = CompanyService.GetValue(comapanyId.Value, "Trustpay.Url", isProduction);


                    string approvedUrl = "https://" + domain + "/tr/deposit/successfulpayfixtransfer"; // successful
                    string deniedUrl = "https://" + domain + "/tr/deposit/failedpayfixdeposit"; // failed

                    string sign = Convert.ToBase64String(SecurityHelper.GetSignHMACSHA256(secretKey, "amount=" + amount + "&userId=" + userId + "&name=" + firstName + "&surname=" + lastName + "&username=" + username + "&processId=" + processId + "&successRedirectUrl=" + approvedUrl + "&failRedirectUrl=" + deniedUrl));

                    JObject apiResult = (JObject)JsonConvert.DeserializeObject(Helper.HttpHelper.PostRequest(url + "/v1/integration/payfix/deposit", JsonConvert.SerializeObject(new { amount = amount, userId = userId, name = firstName, surname = lastName, username = username, processId = processId, successRedirectUrl = approvedUrl, failRedirectUrl = deniedUrl }), "application/json", new NameValueCollection() { { "appKey", appKey }, { "sign", sign } }));

                    result.Success = true;
                    result.RedirectURL = apiResult["redirect"].Value<string>();
                    result.MerchantRef = apiResult["transactionId"].Value<string>();
                }

            }
            catch (Exception ex)
            {

                result.Success = false;
                result.ResponseDescription = "İşlem sırasında bir hata oluştu, lütfen canlı yardım'a bağlanın.";
                Logger.Fatal("Error deposit with paykasa method " + ex.Message, ex);
            }

            return result;
        }
        public DepositResult StartDepositWithJetMethod(string domain, int memberId, bool isProduction, decimal amount, string currency, bool withBonus, int? bonusId)
        {
            DepositResult result = new DepositResult { Success = false };
            TransactionResponse response;
            string retVal = string.Empty;
            bool paymentProcessedSuccessfuly = false;
            string merchantRef = "0";
            JetRequest jetRequest = null;
            try
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                    {
                        // insert eco request here return the unique id and this will be merchantref
                        jetRequest = JetRequestRepository.Insert(new JetRequest
                        {
                            Currency = currency,
                            Amount = (amount),
                            RecognisedAmount = (long)(amount * 100),
                            MemberId = memberId,
                            StatusType = (int)StatusType.Passive,
                            CreateDate = DateTime.UtcNow,
                            WithBonus = withBonus,
                            BonusId = bonusId
                        });


                        string statusCheckUrl = "https://" + domain + "/tr/jet/status"; // status check

                        NameValueCollection headerList = new NameValueCollection();
                        headerList.Add("X-API-KEY", "da2bdb22b15d4a0baa2126d3be0507d3");

                        string jetResult = NW.Helper.HttpHelper.PostRequest("https://walletapi.jeton.com/api/v3/integration/merchants/payments/pay", JsonConvert.SerializeObject(new
                        {
                            amount = amount.ToString("#.##"),
                            currency = currency,
                            orderId = jetRequest.Id,
                            method = "CHECKOUT",
                            returnUrl = statusCheckUrl,
                        }), "application/json", headerList);

                        JObject jObjectResult = (JObject)JsonConvert.DeserializeObject(jetResult);

                        jetRequest.JetToken = HttpUtility.ParseQueryString(new Uri(jObjectResult["checkout"].Value<string>()).Query)["token"];
                        jetRequest.JetResult = jetResult;
                        jetRequest.JetPaymentId = jObjectResult["paymentId"].Value<long>();

                        JetRequestRepository.Update(jetRequest);

                        transaction.Commit();




                        result = new DepositResult
                        {
                            Success = true,
                            Amount = Convert.ToInt64(jetRequest.Amount),
                            MerchantRef = jetRequest.Id.ToString(),
                            TransactionId = retVal,
                            RedirectURL = jObjectResult["checkout"].Value<string>(),
                            ResponseDescription = "Redirect"
                        };

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Fatal("Inserting failed ecopayz request internally." + ex.Message, ex);
                result.Amount = 0;
                result.ResponseDescription = "Err: Internal error while processing your EcoPayz request. Please contact live chat.";
            }

            return result;
        }

        public DepositResult DepositWithJetMethod(string domain, string username, int? memberId, long paymentId, string customerId, bool isProduction)
        {
            DepositResult result = new DepositResult();

            int? companyId = CompanyService.CompanyId(domain);

            Member member = memberId.HasValue ? MemberService.GetMember(memberId.Value) : MemberService.GetActiveMember(companyId.Value, username);


            JetRequest request = JetRequestRepository.GetAll().FirstOrDefault(jr => jr.JetPaymentId == paymentId && jr.MemberId == member.Id && jr.StatusType == (int)StatusType.Passive);
            if (request != null)
            {
                using (var unitOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = unitOfWork.BeginTransaction(Session);
                    JetRequest lockedJetRequest = (JetRequest)Session.Load("JetRequest", request.Id, LockMode.Upgrade);
                    request = JetRequestRepository.GetAll().FirstOrDefault(jr => jr.JetPaymentId == paymentId && jr.MemberId == member.Id && jr.StatusType == (int)StatusType.Passive);

                    if (request != null)
                    {
                        var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                        var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                        var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                        var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                        var methodName = "DepositWithBonus";

                        int providerId = 61;//jetmethod
                        long? bonusAmount = GetBonusAmount(companyId.Value, providerId, member.Id, request.WithBonus, Convert.ToInt64(request.Amount * 100));

                        var response2 = HttpServiceHelper.PostJsonRequest(partnerUrl + methodName,
                                JsonConvert.SerializeObject(new
                                {
                                    CompanyId = vCompanyId,
                                    ApiUsername = apiUsername,
                                    ApiPassword = apiPassword,
                                    Username = member.Username,
                                    TransactionId = 0,//transactionId,
                                    ProviderTransactionReference = request.Id.ToString(),
                                    ProviderId = providerId,//jetmethod
                                    Amount = Convert.ToInt64(request.Amount * 100),
                                    BonusAmount = bonusAmount,
                                    BonusId = request.BonusId,
                                    Checksum = ""
                                }));


                        bool balanceUpdateSuccessful = Convert.ToBoolean(response2.Success);
                        if (balanceUpdateSuccessful)
                        {
                            request.StatusType = (int)StatusType.Active;
                            request.JetClientAccountNumber = customerId;
                            JetRequestRepository.Update(request);
                            transaction.Commit();



                            result.Success = true;
                            result.Amount = Convert.ToInt64(request.Amount * 100);

                            ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (request.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (request.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                            MemberService.CheckLevelAfterDeposit(domain, member, isProduction);
                            MemberService.GiveBonusAfterDepositForEligibleUsers(companyId.Value, member.Username, isProduction, Convert.ToInt64(request.Amount * 100), 11010, providerId);
                            MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId.Value, member.Username, Convert.ToInt64(request.Amount * 100));
                        }
                        else
                        {
                            result.ResponseDescription = "Bakiye güncellerken hata oluştu. Lütfen canlı chat'e bağlanın.";
                            Logger.Fatal("Bakiye güncellerken hata oluştu, sebep: " + response2.Message + ", obj: " + JsonConvert.SerializeObject(response2));
                            // error occured, we successfully took the money but failed to deposit it to Voltron (SW Engine) 
                            // either try again 2nd time or trigger an email/SMS to warn payment/cs department.

                            //DisplayMessage("Please contact customer service", true);

                            ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (request.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (request.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = result.ResponseDescription }));
                            transaction.Rollback();
                        }

                    }
                }

            }

            return result;
        }

        public WithdrawResult WithdrawWithCrypto(string domain, int memberId, bool isProduction, long amount, string userIp, string currency, string cryptoWalletAddress)
        {
            WithdrawResult result = new WithdrawResult { Success = false };

            CryptoAdditionalInfo additionalInfo = new CryptoAdditionalInfo() { Amount = amount };
            int providerId = 103; //crypto
            try
            {

                Member member = MemberService.GetMember(memberId);

                DepositResult depositResult = new DepositResult();


                int? companyId = CompanyService.CompanyId(domain);

                var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                var withdrawRequest = "WithdrawRequestWithProviderId";//int companyId, string apiUsername, string username, long withdrawAmount, long partnerReferenceId, string checksum

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                dynamic obj = HttpServiceHelper.GetJsonRequest("https://api.paybin.io/v1/exchangeprices?symbol=BTC");

                var amountToDebitToMember = 0d;
                amountToDebitToMember = Convert.ToDouble(amount);

                additionalInfo.Amount = Convert.ToInt64((amountToDebitToMember * 100));
                additionalInfo.Currency = currency;
                additionalInfo.CryptoWalletAddress = cryptoWalletAddress;
                additionalInfo.Rate = Convert.ToDecimal(obj["try"]["ask"]) * 1.01m;
                string addtionalInfoJSON = JsonConvert.SerializeObject(additionalInfo);

                var request = new RestRequest(withdrawRequest, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", member.Username); // adds to POST or URL querystring based on Method
                request.AddParameter("providerId", providerId); // adds to POST or URL querystring based on Method // 45 ecopayz
                request.AddParameter("withdrawAmount", additionalInfo.Amount); // adds to POST or URL querystring based on Method
                request.AddParameter("additionalInfo", addtionalInfoJSON); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, member.Username, providerId.ToString(), additionalInfo.Amount.ToString(), addtionalInfoJSON)); // adds to POST or URL querystring based on Method
                                                                                                                                                                                                       //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                depositResult.Success = response.StatusCode == HttpStatusCode.OK && Convert.ToBoolean(response.Data["Success"]);
                if (depositResult.Success)
                {
                    ServiceBusHelper.InsertQueue(isProduction, "withdrawrequested", JsonConvert.SerializeObject(new { amount = (additionalInfo.Amount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = currency, originalAmount = ((decimal)(additionalInfo.Amount / 100m) / additionalInfo.Rate).ToString("0.0000000", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));



                    result = new WithdrawResult
                    {
                        Success = true,
                        Amount = Convert.ToInt64(amount),
                        //MerchantRef = ecoRequest.Id.ToString(),
                        TransactionId = Convert.ToInt64(response.Data["Message"]),
                        RedirectURL = string.Empty,
                        ResponseDescription = string.Empty
                    };
                    //Task.Run(() => SendSMSAsync(member.Username, amount, Logger));
                }
                else
                {
                    Logger.Fatal("Failed updating voltron: " + response.Data["Message"]);
                    result.ResponseDescription = response.Data["Message"];
                }



            }
            catch (Exception ex)
            {
                Logger.Fatal("Inserting failed Crypto request internally / withdraw." + ex.Message, ex);
                result.Amount = 0;
                result.ResponseDescription = "Err: Internal error while processing your Crypto withdraw. Please contact live chat.";
            }

            return result;

        }

        public WithdrawResult WithdrawWithJetonVoucher(string domain, string username, bool isProduction, Int64 amount, string currency)
        {
            WithdrawResult result = new WithdrawResult { Success = false };

            JetonVoucherAdditinalInfo additionalInfo = new JetonVoucherAdditinalInfo() { Amount = amount, AmountJetonCurrency = amount, Currency = currency };
            int providerId = 68; //jetonvoucher
            try
            {

                DepositResult depositResult = new DepositResult();


                int? companyId = CompanyService.CompanyId(domain);
                Member member = MemberService.GetActiveMember(companyId.Value, username);

                var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                var withdrawRequest = "WithdrawRequestWithProviderId";//int companyId, string apiUsername, string username, long withdrawAmount, long partnerReferenceId, string checksum

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                additionalInfo.Amount = Convert.ToInt64((amount * 100));
                string addtionalInfoJSON = JsonConvert.SerializeObject(additionalInfo);

                var request = new RestRequest(withdrawRequest, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", member.Username); // adds to POST or URL querystring based on Method
                request.AddParameter("providerId", providerId); // adds to POST or URL querystring based on Method // 45 ecopayz
                request.AddParameter("withdrawAmount", additionalInfo.Amount); // adds to POST or URL querystring based on Method
                request.AddParameter("additionalInfo", addtionalInfoJSON); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum",
                    SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, member.Username, providerId.ToString(), additionalInfo.Amount.ToString(), addtionalInfoJSON)); // adds to POST or URL querystring based on Method
                                                                                                                                                                                                       //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                depositResult.Success = response.StatusCode == HttpStatusCode.OK && Convert.ToBoolean(response.Data["Success"]);
                if (depositResult.Success)
                {
                    ServiceBusHelper.InsertQueue(isProduction, "withdrawrequested", JsonConvert.SerializeObject(new { amount = (additionalInfo.Amount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (additionalInfo.Amount / 100m).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                    result = new WithdrawResult
                    {
                        Success = true,
                        Amount = Convert.ToInt64(amount),
                        //MerchantRef = ecoRequest.Id.ToString(),
                        TransactionId = Convert.ToInt64(response.Data["Message"]),
                        RedirectURL = string.Empty,
                        ResponseDescription = string.Empty
                    };
                    //Task.Run(() => SendSMSAsync(member.Username, amount, Logger));
                }
                else
                {
                    Logger.Fatal("Failed updating voltron: " + response.Data["Message"]);
                    result.ResponseDescription = response.Data["Message"];
                }



            }
            catch (Exception ex)
            {
                Logger.Fatal("Inserting failed paykasa request internally / withdraw." + ex.Message, ex);
                result.Amount = 0;
                result.ResponseDescription = "Err: Internal error while processing your Paykasa withdraw. Please contact live chat.";
            }

            return result;

        }
        private bool ReachedCreditCardLimit(int memberId, long amount)
        {

            return false;
        }

        public DepositResult DepositWithBPProvider(string domain, int memberId, bool isProduction, Int64 amount, string cardHolderName, string cardNo, int expiryYear,
            int expiryMonth, string cvv, string userIp, bool force3D)
        {
            dynamic response;
            BPRequest request;
            string retVal = string.Empty;
            bool paymentProcessedSuccessfuly = false;
            string validCardMessage = "";


            try
            {

                bool validCard = ValidateCardNumberAndExpiry(cardNo, expiryMonth, expiryYear, out validCardMessage);

                bool reachedLimit = ReachedCreditCardLimit(memberId, amount);

                if (validCard)
                {
                    using (var uniOfWork = UnitOfWork.Current)
                    {
                        string providerTxId;
                        Int64 transactionId = 0;
                        Member member;

                        using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                        {

                            // false is master mid, true at hirsizlari icin.
                            force3D = false; // this is actually not force3d , it should be true if you dont trust the player. see citigate IT definition of VIP member.

                            // initialize settings for payment 
                            int? comapanyId = CompanyService.CompanyId(domain);
                            string postUrl = CompanyService.GetValue(comapanyId.Value, "BP.PostUrl", isProduction);
                            //string mName = CompanyService.GetValue(comapanyId.Value, "BP.MerchantName" + (force3D ? "3d" : ""), isProduction);
                            string merchantKey = CompanyService.GetValue(comapanyId.Value, "BP.MerchantKey", isProduction);

                            // we did this check to avoid baymavivip and baymavili.com to fail on callback as thi
                            //string baseCallbackUrl = "http://stage.maviappcontent.com/tr";
                            string baseUrl = "http://" + domain + "/tr";

                            string sUrl = CompanyService.GetValue(comapanyId.Value, "BP.SuccessUrl", isProduction);
                            string fUrl = CompanyService.GetValue(comapanyId.Value, "BP.FailUrl", isProduction);
                            //string cbUrl = CompanyService.GetValue(comapanyId.Value, "BP.CallbackUrl", isProduction);

                            member = MemberRepository.Get(memberId);
                            Int32 cardId = 0;

                            string encCardNo = NW.Security.SecurityHelper.Encrypt(cardNo);

                            // check credit card exist, if not save it
                            var cardExist = CreditCardRepository.CreditCardExist(cardNo, expiryMonth, expiryYear);
                            if (!cardExist)
                            {
                                CreditCardRepository.Insert(new CreditCard
                                {
                                    CardNumber = cardNo,
                                    CreateDate = DateTime.UtcNow,
                                    CVV = cvv,
                                    ExpiryMonth = expiryMonth,
                                    ExpiryYear = expiryYear,
                                    MemberId = member.Id,
                                    NameOnCard = cardHolderName
                                });
                            }

                            cardId = CreditCardRepository.GetCard(cardNo, expiryMonth, expiryYear).Id;

                            CurrencyRate rate =
                                CurrencyRateRepository.GetAll().OrderByDescending(x => x.Id).FirstOrDefault(x => x.ToCurrency.Id == 2 && x.FromCurrency.Id == 3); // 1 = usd, 3 = eur
                            long eurAmount = Convert.ToInt64(amount * 100 / rate.Rate);

                            // create a credit card requet before processing it with provider. We need to send them reference number of that request.
                            CreditCardRequest crequest = CreditCardRequestRepository.Insert(
                                new CreditCardRequest
                                {
                                    Amount = eurAmount,
                                    OriginalAmount = amount,
                                    CCId = cardId,
                                    CreateDate = DateTime.UtcNow,
                                    IsComplete = false,
                                    MemberId = memberId,
                                    PaymentProviderId = 3 // bp
                                });

                            request = new BPRequest
                            {
                                PaymentMethod = "CREDIT_CARD",
                                MerchantKey = merchantKey,
                                MerchantRef = crequest.Id.ToString(),
                                Currency = "EUR", // base currency for Citigate
                                Amount = (eurAmount / 100),
                                // USD amount , currency exchange might be needed if member currency is different.
                                Language = "TR",
                                CardNo = cardNo,
                                ExpiryDate = expiryMonth.ToString().PadLeft(2, '0') + "-" + expiryYear.ToString().Substring(2),
                                CVV = cvv, //NW.Security.SecurityHelper.Decrypt(cvv),
                                Firstname = member.FirstName,
                                Lastname = member.LastName,
                                Address = "Street Line 1",
                                City = "Istanbul", //member.MemberDetails.Where(w=>w.Key=="City").FirstOrDefault().Value,
                                PostalCode = "34000",
                                //member.MemberDetails.Where(w => w.Key == "PostCode").FirstOrDefault().Value,
                                Country = "TR", //member.MemberDetails.Where(w => w.Key == "Country").FirstOrDefault().Value,
                                Email = (member.Id + "@bymv.eu"),
                                DateOfBirth = "1980-01-01", //member.MemberDetails.Where(w => w.Key == "Birthday").FirstOrDefault().Value,
                                SuccessURL = baseUrl + sUrl,
                                FailURL = baseUrl + fUrl,
                                Is3d = false,
                                Only3d = false
                            };

                            string data = JsonConvert.SerializeObject(request);

                            Logger.Fatal("Username: " + member.Username + ", request json:" + data);

                            // save the request first
                            uniOfWork.Commit(transaction);

                            // post this xml to payment provider and get the response.
                            response = HttpServiceHelper.PostJsonRequest(postUrl, data);

                            Logger.Fatal("Username: " + member.Username + ", response:" + JsonConvert.SerializeObject(response));

                            providerTxId = response.transactionId;


                            // we receive the response from provider, now complete the handshake.
                            using (ITransaction transaction2 = uniOfWork.BeginTransaction(Session))
                            {
                                crequest.Request = data;
                                crequest.IsComplete = true;
                                crequest.ProviderRefId = providerTxId;
                                CreditCardRequestRepository.Update(crequest);

                                var creditCardResponse = new CreditCardResponse
                                {
                                    Amount = Convert.ToInt64(request.Amount),
                                    CreateDate = DateTime.Now,
                                    BalanceUpdated = false,
                                    CCRequestId = crequest.Id
                                };
                                CreditCardResponseRepository.Insert(creditCardResponse);

                                uniOfWork.Commit(transaction2);
                            }

                        }


                        if (response.status == "APPROVED") // APPROVED means success
                        {
                            return new DepositResult
                            {
                                Success = paymentProcessedSuccessfuly,
                                Amount = Convert.ToInt64(request.Amount),
                                MerchantRef = request.MerchantRef,
                                TransactionId = retVal,
                                ResponseCode = 600,
                                RedirectURL = response.redirectUrl,
                            }; // if code is 600 required 3d redirect
                        }
                    }


                    return new DepositResult
                    {
                        Success = paymentProcessedSuccessfuly,
                        Amount = Convert.ToInt64(request.Amount),
                        MerchantRef = request.MerchantRef,
                        TransactionId = retVal,
                        ResponseCode = -1,
                        RedirectURL = string.Empty,
                    }; // if code is 600 required 3d redirect

                }
                return new DepositResult
                {
                    Success = false,
                    Amount = 0,
                    MerchantRef = "",
                    TransactionId = retVal,
                    ResponseCode = -1,
                    ResponseDescription = validCardMessage,
                    RedirectURL = ""
                }; // if code is 600 required 3d redirect
            }
            catch (Exception ex)
            {
                Logger.Fatal("Payment Service: Runtime error: " + ex.Message);
                return new DepositResult
                {
                    Success = false,
                    Amount = 0,
                    MerchantRef = "",
                    TransactionId = retVal,
                    ResponseCode = -1,
                    ResponseDescription = "Runtime error, please contact live help",
                    RedirectURL = ""
                }; // runtime error.
            }

        }
        public DepositResult DepositWithCreditCard(string domain, int memberId, bool isProduction, Int64 amount, string cardHolderName, string cardNo, int expiryYear,
            int expiryMonth, string cvv, string userIp, bool force3D, bool withBonus, int? bonusId)
        {
            TransactionResponse response;
            string retVal = string.Empty;
            bool paymentProcessedSuccessfuly = false;
            string validCardMessage = "";


            try
            {
                //if (amount > 5000)// max 1000
                //{
                //    return new DepositResult
                //    {
                //        Success = false,
                //        Amount = 0,
                //        MerchantRef = "",
                //        TransactionId = retVal,
                //        ResponseCode = -1,
                //        ResponseDescription = "En fazla 5000 TL yatırabilirsiniz.",
                //        RedirectURL = ""
                //    };
                //}

                //if (amount < 100) // min 80
                //{
                //    return new DepositResult
                //    {
                //        Success = false,
                //        Amount = 0,
                //        MerchantRef = "",
                //        TransactionId = retVal,
                //        ResponseCode = -1,
                //        ResponseDescription = "En az 100 TL yatırmanız gerekmektedir.",
                //        RedirectURL = ""
                //    };
                //}

                bool validCard = ValidateCardNumberAndExpiry(cardNo, expiryMonth, expiryYear, out validCardMessage);

                bool reachedLimit = ReachedCreditCardLimit(memberId, amount);

                if (validCard)
                {
                    using (var uniOfWork = UnitOfWork.Current)
                    {
                        string providerTxId = string.Empty;
                        Int64 transactionId = 0;
                        Member member;

                        bool isMobile = domain.Contains("m.");

                        using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                        {

                            // false is master mid, true at hirsizlari icin.
                            force3D = false; // this is actually not force3d , it should be true if you dont trust the player. see citigate IT definition of VIP member.

                            // initialize settings for payment 
                            int? comapanyId = CompanyService.CompanyId(domain);
                            string postUrl = CompanyService.GetValue(comapanyId.Value, "Citigate.PostUrl", isProduction);
                            string mName = CompanyService.GetValue(comapanyId.Value, "Citigate.MerchantName" + (force3D ? "3d" : ""), isProduction);
                            string mPassword = CompanyService.GetValue(comapanyId.Value, "Citigate.MerchantPassword" + (force3D ? "3d" : ""), isProduction);

                            // we did this check to avoid baymavivip and baymavili.com to fail on callback as thi
                            string baseCallbackUrl = "https://www.maviappcontent.com/tr";
                            string baseUrl = "https://" + domain + "/tr";

                            string sUrl = CompanyService.GetValue(comapanyId.Value, "Citigate.SuccessUrl", isProduction);
                            string fUrl = CompanyService.GetValue(comapanyId.Value, "Citigate.FailUrl", isProduction);
                            string cbUrl = CompanyService.GetValue(comapanyId.Value, "Citigate.CallbackUrl", isProduction);

                            member = MemberRepository.Get(memberId);
                            Int32 cardId = 0;

                            string encCardNo = NW.Security.SecurityHelper.Encrypt(cardNo);

                            // check credit card exist, if not save it
                            var cardExist = CreditCardRepository.CreditCardExist(cardNo, expiryMonth, expiryYear);
                            if (!cardExist)
                            {
                                CreditCardRepository.Insert(new CreditCard
                                {
                                    CardNumber = cardNo,
                                    CreateDate = DateTime.UtcNow,
                                    CVV = cvv,
                                    ExpiryMonth = expiryMonth,
                                    ExpiryYear = expiryYear,
                                    MemberId = member.Id,
                                    NameOnCard = cardHolderName
                                });
                            }

                            cardId = CreditCardRepository.GetCard(cardNo, expiryMonth, expiryYear).Id;

                            CurrencyRate rate =
                                CurrencyRateRepository.GetAll().OrderByDescending(x => x.Id).FirstOrDefault(x => x.ToCurrency.Id == 2 && x.FromCurrency.Id == 3); // 1 = usd, 3 = eur
                            long eurAmount = Convert.ToInt64(amount * 100 / rate.Rate);

                            // create a credit card requet before processing it with provider. We need to send them reference number of that request.
                            CreditCardRequest crequest = CreditCardRequestRepository.Insert(
                                new CreditCardRequest
                                {
                                    Amount = eurAmount,
                                    OriginalAmount = amount,
                                    CCId = cardId,
                                    CreateDate = DateTime.UtcNow,
                                    IsComplete = false,
                                    MemberId = memberId,
                                    PaymentProviderId = 1, // cc
                                    WithBonus = withBonus,
                                    BonusId = bonusId
                                });

                            TransactionRequest request = new TransactionRequest
                            {
                                MerchantName = mName,
                                MerchantPassword = mPassword,
                                MerchantRef = crequest.Id.ToString(),
                                Currency = "EUR", // base currency for Citigate
                                PaymentTypeId = 1, // 1 card payment
                                TransTypeId = 0, // 0 purchase
                                Amount = eurAmount,
                                // USD amount , currency exchange might be needed if member currency is different.
                                Brand = cardNo.StartsWith("4") ? "VISA" : "MASTERCARD",
                                CardholderName = cardHolderName,
                                CardNo = cardNo,
                                ExpiryMonth = expiryMonth,
                                ExpiryYear = expiryYear,
                                CVV = cvv, //NW.Security.SecurityHelper.Decrypt(cvv),
                                Firstname = member.FirstName,
                                Surname = member.LastName,
                                StreetLine1 = "Street Line 1",
                                StreetLine2 = "",
                                City = "Istanbul", //member.MemberDetails.Where(w=>w.Key=="City").FirstOrDefault().Value,
                                PostalCode = "34000",
                                //member.MemberDetails.Where(w => w.Key == "PostCode").FirstOrDefault().Value,
                                StateProvince = "",
                                Country = "TR", //member.MemberDetails.Where(w => w.Key == "Country").FirstOrDefault().Value,
                                Email = member.Email,
                                Telephone = "",
                                DateOfBirth = "", //member.MemberDetails.Where(w => w.Key == "Birthday").FirstOrDefault().Value,
                                UserIP = userIp,
                                SuccessURL = baseUrl + sUrl,
                                FailURL = baseUrl + fUrl,
                                CallbackURL = baseCallbackUrl + cbUrl
                            };

                            if (domain == "casinonavy")
                            {
                                //request.FailURL = "https://payments.casinonavy.com/en/deposit/cfailed";
                            }

                            XmlSerializer xsSubmit = new XmlSerializer(typeof(TransactionRequest));

                            StringWriter sww = new StringWriter();
                            XmlWriter writer = XmlWriter.Create(sww);
                            xsSubmit.Serialize(writer, request);
                            var xml = sww.ToString(); // Your XML

                            Logger.Fatal("Username: " + member.Username + ", request xml:" + xml);

                            // save the request first
                            uniOfWork.Commit(transaction);

                            // post this xml to payment provider and get the response.
                            response = HttpServiceHelper.PostXMLRequest<TransactionResponse>(postUrl, xml);

                            Logger.Fatal("Username: " + member.Username + ", response:" + JsonConvert.SerializeObject(response));

                            providerTxId = response.TransactionID.ToString();


                            // we receive the response from provider, now complete the handshake.
                            using (ITransaction transaction2 = uniOfWork.BeginTransaction(Session))
                            {
                                crequest.Request = xml;
                                crequest.IsComplete = true;
                                crequest.ProviderRefId = providerTxId;
                                CreditCardRequestRepository.Update(crequest);

                                var creditCardResponse = new CreditCardResponse
                                {
                                    Amount = Convert.ToInt64(response.Amount),
                                    CreateDate = DateTime.Now,
                                    BalanceUpdated = false,
                                    CCRequestId = crequest.Id
                                };
                                CreditCardResponseRepository.Insert(creditCardResponse);

                                uniOfWork.Commit(transaction2);
                            }

                        }


                        if (response.ResponseCode == 0) // 0 means success
                        {
                            retVal = providerTxId;
                            //

                            //paymentProcessedSuccessfuly = true;
                            //creditCardResponse.BalanceUpdated = paymentProcessedSuccessfuly;
                            transactionId = Convert.ToInt64(response.MerchantRef);

                            CreditCardRequest cr =
                                        CreditCardRequestRepository.GetAll().SingleOrDefault(w => w.Id == transactionId);

                            int? companyId = CompanyService.CompanyId(domain);
                            var partnerUrlToDepositMoneyToAccount = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                            var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                            var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);
                            var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);

                            int providerId = 30;
                            long? bonusAmount = GetBonusAmount(companyId.Value, providerId, member.Id, cr.WithBonus, cr.OriginalAmount * 100);
                            var response2 = HttpServiceHelper.PostJsonRequest(partnerUrlToDepositMoneyToAccount + "DepositWithBonus",
                                                JsonConvert.SerializeObject(new
                                                {
                                                    CompanyId = vCompanyId,
                                                    ApiUsername = apiUsername,
                                                    ApiPassword = apiPassword,
                                                    Username = member.Username,
                                                    TransactionId = transactionId,
                                                    ProviderTransactionReference = providerTxId,
                                                    ProviderId = providerId,
                                                    Amount = cr.OriginalAmount * 100,
                                                    BonusAmount = bonusAmount,
                                                    BonusId = bonusId,
                                                    Checksum = ""
                                                }));


                            ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (cr.OriginalAmount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (cr.OriginalAmount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));
                            NWWebHook.PushBackOffice(domain, "g:bo", "", "Player " + member.Username + " made a deposit with credit card of " + cr.OriginalAmount + " TL");

                            MemberService.CheckLevelAfterDeposit(domain, memberId, isProduction);
                            MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId.Value, member.Username, cr.OriginalAmount * 100);
                            MemberService.GiveBonusAfterDepositForEligibleUsers(companyId.Value, member.Username, isProduction, cr.OriginalAmount * 100, 11002, providerId);
                            // add the amount to the customer wallet
                            //BalanceResult balanceResult = MemberService.UpdateBalance(domain, member.Username, amount, 0, isProduction);
                            //if (balanceResult.IsSuccess)
                            //{
                            //    // success

                            //}
                            //Logger.Fatal("Transaction successful from citigate without 3d. txId:" + providerTxId);
                            //var partnerUrl = CompanyService.GetValue(comapanyId.Value, "WalletEndpoint.ServiceURL", isProduction);
                            //var voltronCompanyId = CompanyService.GetValue(comapanyId.Value, "Voltron.CompanyId", isProduction);


                            //var response = HttpServiceHelper.PostJsonRequest(partnerUrl + "Deposit/?username=" + username + "&companyId=" + voltronCompanyId + "&language=" + language + "&gameId=" + game.VoltronGameId + "&providerId=" + game.VoltronProviderId));

                            //if (Convert.ToBoolean(response.success))
                            //{
                            //    URL = response.GameURL;
                            //}

                            //uniOfWork.Commit();
                        }
                    }

                    return new DepositResult
                    {
                        Success = paymentProcessedSuccessfuly,
                        Amount = response.Amount,
                        MerchantRef = response.MerchantRef,
                        TransactionId = retVal,
                        ResponseCode = response.ResponseCode,
                        RedirectURL = response.RedirectURL,
                        ResponseDescription = response.ResponseDescription
                    }; // if code is 600 required 3d redirect


                }
                return new DepositResult
                {
                    Success = false,
                    Amount = 0,
                    MerchantRef = "",
                    TransactionId = retVal,
                    ResponseCode = -1,
                    ResponseDescription = validCardMessage,
                    RedirectURL = ""
                }; // if code is 600 required 3d redirect
            }
            catch (Exception ex)
            {
                Logger.Fatal("Payment Service: Runtime error: " + ex.Message);
                return new DepositResult
                {
                    Success = false,
                    Amount = 0,
                    MerchantRef = "",
                    TransactionId = retVal,
                    ResponseCode = -1,
                    ResponseDescription = "Runtime error, please contact live help",
                    RedirectURL = ""
                }; // runtime error.
            }

        }

        public DepositResult DepositWithCreditCardReyPay(string domain, int memberId, bool isProduction, Int64 amount, string cardHolderName, string cardNo, int expiryYear,
            int expiryMonth, string cvv, string userIp, bool force3D, bool withBonus, int? bonusId)
        {
            var rpService = new NW.Payment.Wrappers.ReyPayWs.IWSPINservice();
            ReyPayResult<ReyPayGetPaymentResponse> response;
            string retVal = string.Empty;
            string validCardMessage = "";

            try
            {
                bool validCard = ValidateCardNumberAndExpiry(cardNo, expiryMonth, expiryYear, out validCardMessage);

                bool reachedLimit = ReachedCreditCardLimit(memberId, amount);

                if (validCard)
                {
                    using (var uniOfWork = UnitOfWork.Current)
                    {
                        string providerTxId = string.Empty;
                        Int64 transactionId = 0;
                        Member member;

                        using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                        {

                            // initialize settings for payment 
                            int? companyId = CompanyService.CompanyId(domain);
                            var wsUser = CompanyService.GetValue(companyId.Value, "ReyPay.WsUser", isProduction);
                            var wsPass = CompanyService.GetValue(companyId.Value, "ReyPay.WsPass", isProduction);
                            var apiKey = CompanyService.GetValue(companyId.Value, "ReyPay.ApiKey", isProduction);
                            var username = CompanyService.GetValue(companyId.Value, "ReyPay.Username", isProduction);
                            var password = CompanyService.GetValue(companyId.Value, "ReyPay.Password", isProduction);


                            member = MemberRepository.Get(memberId);
                            Int32 cardId = 0;


                            // check credit card exist, if not save it
                            var cardExist = CreditCardRepository.CreditCardExist(cardNo, expiryMonth, expiryYear);
                            if (!cardExist)
                            {
                                CreditCardRepository.Insert(new CreditCard
                                {
                                    CardNumber = cardNo,
                                    CreateDate = DateTime.UtcNow,
                                    CVV = cvv,
                                    ExpiryMonth = expiryMonth,
                                    ExpiryYear = expiryYear,
                                    MemberId = member.Id,
                                    NameOnCard = cardHolderName
                                });
                            }

                            cardId = CreditCardRepository.GetCard(cardNo, expiryMonth, expiryYear).Id;

                            long longAmount = Convert.ToInt64(amount * 100);

                            // create a credit card requet before processing it with provider. We need to send them reference number of that request.
                            CreditCardRequest crequest = CreditCardRequestRepository.Insert(
                                new CreditCardRequest
                                {
                                    Amount = longAmount,
                                    OriginalAmount = amount,
                                    CCId = cardId,
                                    CreateDate = DateTime.UtcNow,
                                    IsComplete = false,
                                    MemberId = memberId,
                                    PaymentProviderId = 4, // ReyPay
                                    WithBonus = withBonus,
                                    BonusId = bonusId
                                });

                            // We were told to create a new customer on ReyPay system for each member
                            username = "member_" + member.Id.ToString().PadLeft(8, '0');

                            var request = new ReyPayGetPaymentRequest
                            {
                                Wsuser = wsUser,
                                Wspass = wsPass,
                                Apikey = apiKey,

                                Username = username,
                                Password = password,

                                Card_Name = cardHolderName,
                                Card_Month = expiryMonth.ToString().PadLeft(2, '0'),
                                Card_Year = expiryYear.ToString(),
                                Card_Number1 = cardNo.Substring(0, 4),
                                Card_Number2 = cardNo.Substring(4, 4),
                                Card_Number3 = cardNo.Substring(8, 4),
                                Card_Number4 = cardNo.Substring(12, 4),
                                Card_Security = cvv,
                                Card_Type = cardNo.StartsWith("4") ? "20" : "10", //20: VISA, 10: MASTERCARD, 30: AMEX
                                Ip = userIp,
                                Price = Convert.ToDouble(amount)
                            };

                            XmlSerializer xsSubmit = new XmlSerializer(typeof(ReyPayGetPaymentRequest));

                            StringWriter sww = new StringWriter();
                            XmlWriter writer = XmlWriter.Create(sww);
                            xsSubmit.Serialize(writer, request);
                            var xml = sww.ToString(); // Your XML

                            Logger.Fatal("Username: " + member.Username + ", request xml:" + xml);

                            // save the request first
                            uniOfWork.Commit(transaction);

                            // Check payment statuses for a given date. Uncomment and run this to get the results
                            //var ccc = rpService.PaymentList(request.Wsuser, request.Wspass, request.Apikey, "2018-04-10");

                            // Create a user on payment provider system. It will either succeed, or return errorcode 140 or 141 for existing customer, which is fine...
                            var newCustomerResponse = rpService.NewCustomer(request.Wsuser, request.Wspass, request.Apikey, "Member",
                                member.Id.ToString().PadLeft(8, '0'), request.Username + "@hvaryemez.net", request.Username, request.Password, "01", "01", "1940", "",
                                "", "", "", "1112223344", "11111111111");

                            var newCustomerResponseModel = JsonConvert.DeserializeObject<ReyPayResult<ReyPayNewCustomerResponse>>(newCustomerResponse);

                            // Expected ResultCode --> [100: Success || 140: Mukerrer Kayit (email) || 141: Mukerrer Kayit (username)]
                            if (newCustomerResponseModel.ResultCode != 100 && newCustomerResponseModel.ResultCode != 140 && newCustomerResponseModel.ResultCode != 141)
                            {
                                Logger.Fatal(JsonConvert.SerializeObject(newCustomerResponseModel));

                                return new DepositResult
                                {
                                    Success = false,
                                    Amount = 0,
                                    MerchantRef = "",
                                    TransactionId = retVal,
                                    ResponseCode = -1,
                                    ResponseDescription = newCustomerResponseModel.ResultMessage,
                                    RedirectURL = ""
                                };
                            }

                            // post this xml to payment provider and get the response.
                            var paymentResponse = rpService.GetPayment(request.Wsuser, request.Wspass, request.Apikey,
                                request.Username, request.Password, request.Card_Number1, request.Card_Number2,
                                request.Card_Number3, request.Card_Number4, request.Card_Month, request.Card_Year,
                                request.Card_Security, request.Card_Name, request.Card_Type, request.Ip, request.Price);

                            response = JsonConvert.DeserializeObject<ReyPayResult<ReyPayGetPaymentResponse>>(paymentResponse);

                            Logger.Fatal("Username: " + member.Username + ", response:" + JsonConvert.SerializeObject(response));


                            if (response.ResultCode != 100) // 100 means success
                            {
                                Logger.Fatal(response.ResultMessage);

                                return new DepositResult
                                {
                                    Success = false,
                                    Amount = 0,
                                    MerchantRef = "",
                                    TransactionId = retVal,
                                    ResponseCode = -1,
                                    ResponseDescription = response.ResultCode == 169 ? "Uygun kart değildir" : response.ResultMessage,//blacklist
                                    RedirectURL = ""
                                };
                            }

                            retVal = response.ResultObject.PaymentId;
                            providerTxId = response.ResultObject.Reference_Code;

                            if (response.ResultObject.SmsStatus != "1") // "1" means 3D SMS has been sent to customer's phone
                            {
                                Logger.Fatal(JsonConvert.SerializeObject(response));

                                return new DepositResult
                                {
                                    Success = false,
                                    ResponseCode = -1,
                                    Amount = 0,
                                    ResponseDescription = response.ResultObject.SmsContext,
                                    MerchantRef = providerTxId
                                };
                            }

                            using (ITransaction transaction2 = uniOfWork.BeginTransaction(Session))
                            {
                                crequest.Request = xml;
                                crequest.ProviderRefId = providerTxId;
                                CreditCardRequestRepository.Update(crequest);

                                uniOfWork.Commit(transaction2);
                            }

                            // Redirect to SMS flow
                            return new DepositResult { Success = true, ResponseCode = 5555, MerchantRef = providerTxId, Amount = amount };
                        }
                    }
                }
                return new DepositResult
                {
                    Success = false,
                    Amount = 0,
                    MerchantRef = "",
                    TransactionId = retVal,
                    ResponseCode = -1,
                    ResponseDescription = validCardMessage,
                    RedirectURL = ""
                };
            }
            catch (Exception ex)
            {
                Logger.Fatal("Payment Service: Runtime error: " + ex.Message);
                return new DepositResult
                {
                    Success = false,
                    Amount = 0,
                    MerchantRef = "",
                    TransactionId = retVal,
                    ResponseCode = -1,
                    ResponseDescription = "Runtime error, please contact live help",
                    RedirectURL = ""
                }; // runtime error.
            }

        }
        public DepositResult DepositWithCreditCardPayIn(string domain, int memberId, bool isProduction, Int64 amount, string cardHolderName, string cardNo, int expiryYear,
            int expiryMonth, string cvv, string userIp, bool force3D, bool withBonus, int? bonusId)
        {
            var rpService = new NW.Payment.Wrappers.PayInWs.IWSPINservice();
            ReyPayResult<ReyPayGetPaymentResponse> response;
            string retVal = string.Empty;
            string validCardMessage = "";

            try
            {
                bool validCard = ValidateCardNumberAndExpiry(cardNo, expiryMonth, expiryYear, out validCardMessage);

                bool reachedLimit = ReachedCreditCardLimit(memberId, amount);

                if (validCard)
                {
                    using (var uniOfWork = UnitOfWork.Current)
                    {
                        string providerTxId = string.Empty;
                        Int64 transactionId = 0;
                        Member member;

                        using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                        {

                            // initialize settings for payment 
                            int? companyId = CompanyService.CompanyId(domain);
                            var wsUser = CompanyService.GetValue(companyId.Value, "PayIn.WsUser", isProduction);
                            var wsPass = CompanyService.GetValue(companyId.Value, "PayIn.WsPass", isProduction);
                            var apiKey = CompanyService.GetValue(companyId.Value, "PayIn.ApiKey", isProduction);
                            var username = CompanyService.GetValue(companyId.Value, "PayIn.Username", isProduction);
                            var password = CompanyService.GetValue(companyId.Value, "PayIn.Password", isProduction);


                            member = MemberRepository.Get(memberId);
                            Int32 cardId = 0;


                            // check credit card exist, if not save it
                            var cardExist = CreditCardRepository.CreditCardExist(cardNo, expiryMonth, expiryYear);
                            if (!cardExist)
                            {
                                CreditCardRepository.Insert(new CreditCard
                                {
                                    CardNumber = cardNo,
                                    CreateDate = DateTime.UtcNow,
                                    CVV = cvv,
                                    ExpiryMonth = expiryMonth,
                                    ExpiryYear = expiryYear,
                                    MemberId = member.Id,
                                    NameOnCard = cardHolderName
                                });
                            }

                            cardId = CreditCardRepository.GetCard(cardNo, expiryMonth, expiryYear).Id;

                            long longAmount = Convert.ToInt64(amount * 100);

                            // create a credit card requet before processing it with provider. We need to send them reference number of that request.
                            CreditCardRequest crequest = CreditCardRequestRepository.Insert(
                                new CreditCardRequest
                                {
                                    Amount = longAmount,
                                    OriginalAmount = amount,
                                    CCId = cardId,
                                    CreateDate = DateTime.UtcNow,
                                    IsComplete = false,
                                    MemberId = memberId,
                                    PaymentProviderId = 5, // PayIn
                                    WithBonus = withBonus,
                                    BonusId = bonusId
                                });

                            // We were told to create a new customer on ReyPay system for each member
                            username = "member_" + member.Id.ToString().PadLeft(8, '0');

                            var request = new ReyPayGetPaymentRequest
                            {
                                Wsuser = wsUser,
                                Wspass = wsPass,
                                Apikey = apiKey,

                                Username = username,
                                Password = password,

                                Card_Name = cardHolderName,
                                Card_Month = expiryMonth.ToString().PadLeft(2, '0'),
                                Card_Year = expiryYear.ToString(),
                                Card_Number1 = cardNo.Substring(0, 4),
                                Card_Number2 = cardNo.Substring(4, 4),
                                Card_Number3 = cardNo.Substring(8, 4),
                                Card_Number4 = cardNo.Substring(12, 4),
                                Card_Security = cvv,
                                Card_Type = cardNo.StartsWith("4") ? "20" : "10", //20: VISA, 10: MASTERCARD, 30: AMEX
                                Ip = userIp,
                                Price = Convert.ToDouble(amount)
                            };

                            XmlSerializer xsSubmit = new XmlSerializer(typeof(ReyPayGetPaymentRequest));

                            StringWriter sww = new StringWriter();
                            XmlWriter writer = XmlWriter.Create(sww);
                            xsSubmit.Serialize(writer, request);
                            var xml = sww.ToString(); // Your XML

                            Logger.Fatal("Username: " + member.Username + ", request xml:" + xml);

                            // save the request first
                            uniOfWork.Commit(transaction);

                            // Check payment statuses for a given date. Uncomment and run this to get the results
                            //var ccc = rpService.PaymentList(request.Wsuser, request.Wspass, request.Apikey, "2018-04-10");

                            // Create a user on payment provider system. It will either succeed, or return errorcode 140 or 141 for existing customer, which is fine...
                            var newCustomerResponse = rpService.NewCustomer(request.Wsuser, request.Wspass, request.Apikey, "Member",
                                member.Id.ToString().PadLeft(8, '0'), request.Username + "@baymavi.com", request.Username, request.Password, "01", "01", "1940", "",
                                "", "", "", "1112223344", "11111111111");

                            var newCustomerResponseModel = JsonConvert.DeserializeObject<ReyPayResult<ReyPayNewCustomerResponse>>(newCustomerResponse);

                            // Expected ResultCode --> [100: Success || 140: Mukerrer Kayit (email) || 141: Mukerrer Kayit (username)]
                            if (newCustomerResponseModel.ResultCode != 100 && newCustomerResponseModel.ResultCode != 140 && newCustomerResponseModel.ResultCode != 141)
                            {
                                Logger.Fatal(JsonConvert.SerializeObject(newCustomerResponseModel));

                                return new DepositResult
                                {
                                    Success = false,
                                    Amount = 0,
                                    MerchantRef = "",
                                    TransactionId = retVal,
                                    ResponseCode = -1,
                                    ResponseDescription = newCustomerResponseModel.ResultMessage,
                                    RedirectURL = ""
                                };
                            }

                            // post this xml to payment provider and get the response.
                            var paymentResponse = rpService.GetPayment(request.Wsuser, request.Wspass, request.Apikey,
                                request.Username, request.Password, request.Card_Number1, request.Card_Number2,
                                request.Card_Number3, request.Card_Number4, request.Card_Month, request.Card_Year,
                                request.Card_Security, request.Card_Name, request.Card_Type, request.Ip, request.Price);

                            response = JsonConvert.DeserializeObject<ReyPayResult<ReyPayGetPaymentResponse>>(paymentResponse);

                            Logger.Fatal("Username: " + member.Username + ", response:" + JsonConvert.SerializeObject(response));


                            if (response.ResultCode != 100) // 100 means success
                            {
                                Logger.Fatal(response.ResultMessage);

                                return new DepositResult
                                {
                                    Success = false,
                                    Amount = 0,
                                    MerchantRef = "",
                                    TransactionId = retVal,
                                    ResponseCode = -1,
                                    ResponseDescription = response.ResultCode == 169 ? "Uygun kart değildir" : response.ResultMessage,//blacklist
                                    RedirectURL = ""
                                };
                            }

                            retVal = response.ResultObject.PaymentId;
                            providerTxId = response.ResultObject.Reference_Code;

                            if (response.ResultObject.SmsStatus != "1") // "1" means 3D SMS has been sent to customer's phone
                            {
                                Logger.Fatal(JsonConvert.SerializeObject(response));

                                return new DepositResult
                                {
                                    Success = false,
                                    ResponseCode = -1,
                                    Amount = 0,
                                    ResponseDescription = response.ResultObject.SmsContext,
                                    MerchantRef = providerTxId
                                };
                            }

                            using (ITransaction transaction2 = uniOfWork.BeginTransaction(Session))
                            {
                                crequest.Request = xml;
                                crequest.ProviderRefId = providerTxId;
                                CreditCardRequestRepository.Update(crequest);

                                uniOfWork.Commit(transaction2);
                            }

                            // Redirect to SMS flow
                            return new DepositResult { Success = true, ResponseCode = 5555, MerchantRef = providerTxId, Amount = amount };
                        }
                    }
                }
                return new DepositResult
                {
                    Success = false,
                    Amount = 0,
                    MerchantRef = "",
                    TransactionId = retVal,
                    ResponseCode = -1,
                    ResponseDescription = validCardMessage,
                    RedirectURL = ""
                };
            }
            catch (Exception ex)
            {
                Logger.Fatal("Payment Service: Runtime error: " + ex.Message);
                return new DepositResult
                {
                    Success = false,
                    Amount = 0,
                    MerchantRef = "",
                    TransactionId = retVal,
                    ResponseCode = -1,
                    ResponseDescription = "Runtime error, please contact live help",
                    RedirectURL = ""
                }; // runtime error.
            }

        }

        public int SelectCCProvider(int memberId, int companyId, bool isProduction)
        {
            string[] btSystemNames = { "creditcardreypay", "quickbit" };
            List<Provider> btProviders = ProviderService.GetAllProviders(2).Where(p => btSystemNames.Contains(p.SystemName)).ToList();

            List<int> disabledProviders = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).Select(m => m.ProviderId).ToList();

            //btProviders = btProviders.Where(p => p.ProviderSettings.FirstOrDefault(s => s.Name == "IsOpen").Value == "True").ToList(); //TODO
            List<int> tempProviderIds = new List<int>();
            foreach (Provider p in btProviders)
            {
                if (!disabledProviders.Any(dp => dp == p.VoltronProviderId))
                {
                    string weightString = ProviderService.GetValue(p.Id, companyId, "Weight", isProduction);
                    int weight;
                    bool isNumeric = int.TryParse(weightString, out weight);
                    if (isNumeric)
                    {
                        for (int i = weight; i > 0; i--)
                        {
                            tempProviderIds.Add(p.VoltronProviderId);
                        }
                    }
                }
            }
            if (tempProviderIds.Count > 0)
            {
                Random rnd = new Random();
                int r = rnd.Next(tempProviderIds.Count);
                return tempProviderIds[r];
            }
            else
            {
                return -1; //TODO
            }
        }
        public DepositResult SendSmsReyPay(string domain, int memberId, bool isProduction, string reference, string smsCode)
        {
            var rpService = new NW.Payment.Wrappers.ReyPayWs.IWSPINservice();

            try
            {
                using (var unitOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                    {
                        // Check if this ccRequest is valid
                        var pendingReyPayRequest = CreditCardRequestRepository.GetPendingReyPayRequest(4, memberId, false);
                        if (pendingReyPayRequest == null || !pendingReyPayRequest.ProviderRefId.Equals(reference))
                            return new DepositResult
                            {
                                Success = false,
                                ResponseDescription = "Gecersiz referans numarasi",
                                ResponseCode = -1
                            };

                        // Check if ccResponse has been created before
                        var reyPayResponse = CreditCardResponseRepository.GetByCcRequestId(pendingReyPayRequest.Id);
                        if (reyPayResponse != null)
                            return new DepositResult
                            {
                                Success = false,
                                ResponseDescription = "Bu islem daha once tamamlanmis",
                                ResponseCode = -1
                            };

                        // initialize settings for payment 
                        int? companyId = CompanyService.CompanyId(domain);
                        var wsUser = CompanyService.GetValue(companyId.Value, "ReyPay.WsUser", isProduction);
                        var wsPass = CompanyService.GetValue(companyId.Value, "ReyPay.WsPass", isProduction);
                        var apiKey = CompanyService.GetValue(companyId.Value, "ReyPay.ApiKey", isProduction);
                        var username = CompanyService.GetValue(companyId.Value, "ReyPay.Username", isProduction);
                        var password = CompanyService.GetValue(companyId.Value, "ReyPay.Password", isProduction);

                        username = "member_" + memberId.ToString().PadLeft(8, '0');

                        Logger.Fatal(string.Format("ReyPay GetSms Request - Ref: {0} - SmsCode: {1}", reference, smsCode));
                        var getSmsResponse = rpService.GetSms(wsUser, wsPass, apiKey, username, password, reference, smsCode);
                        Logger.Fatal(string.Format("ReyPay GetSms Response: {0}", getSmsResponse));

                        var response = JsonConvert.DeserializeObject<ReyPayResult<ReyPayGetSmsResponse>>(getSmsResponse);

                        var smsWasSentButResponseNotProcessed = false;

                        if (response.ResultCode != 100) // 100 means success
                        {
                            // Check for a special case where the SMS was actually sent in a previous request but there was no response from Provider or it was not processed.
                            if (response.ResultCode == 112 && !pendingReyPayRequest.IsComplete && response.ResultObject != null && response.ResultObject.Status == "2")
                            {
                                Logger.Fatal(string.Format("ReyPay GetSms - Duplicate request's response will be processed correctly now. Ref: {0} - SmsCode: {1}", reference, smsCode));
                                smsWasSentButResponseNotProcessed = true;
                            }
                            else
                            {
                                return new DepositResult
                                {
                                    Success = false,
                                    MerchantRef = reference,
                                    ResponseCode = -1,
                                    ResponseDescription = response.ResultMessage,
                                };
                            }
                        }

                        if (smsWasSentButResponseNotProcessed || response.ResultObject.Status == "2") // "2" means SMS has been sent successfully
                        {
                            pendingReyPayRequest.IsComplete = true;
                            CreditCardRequestRepository.Update(pendingReyPayRequest);

                            unitOfWork.Commit(transaction);

                            using (ITransaction transaction2 = unitOfWork.BeginTransaction(Session))
                            {
                                var ccResponse = new CreditCardResponse
                                {
                                    CCRequestId = pendingReyPayRequest.Id,
                                    BalanceUpdated = false,
                                    CreateDate = DateTime.UtcNow,
                                    Response = getSmsResponse,
                                    Amount = pendingReyPayRequest.Amount
                                };
                                CreditCardResponseRepository.Insert(ccResponse);

                                unitOfWork.Commit(transaction2);
                            }

                            return new DepositResult
                            {
                                Success = true
                            };
                        }
                        else if (response.ResultObject.Status == "1")
                        {
                            // Mark this step as complete since the SMS code has been sent even it's still pending on provider.
                            pendingReyPayRequest.IsComplete = true;
                            CreditCardRequestRepository.Update(pendingReyPayRequest);

                            unitOfWork.Commit(transaction);

                            // Recheck the SMS status later
                            return new DepositResult
                            {
                                Success = false,
                                ResponseCode = 1
                            };
                        }

                        // SMS code not sent so user may try again
                        return new DepositResult
                        {
                            Success = false,
                            ResponseCode = 0
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new DepositResult
                {
                    Success = false,
                    ResponseCode = -1
                };
            }
        }

        public DepositResult CheckSmsStatusReyPay(string domain, int memberId, bool isProduction, string reference)
        {
            var rpService = new NW.Payment.Wrappers.ReyPayWs.IWSPINservice();

            try
            {
                using (var unitOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                    {
                        // Check if this ccRequest is valid
                        var pendingReyPayRequest = CreditCardRequestRepository.GetPendingReyPayRequest(4, memberId, true);
                        if (pendingReyPayRequest == null || !pendingReyPayRequest.ProviderRefId.Equals(reference))
                            return new DepositResult
                            {
                                Success = false,
                                ResponseDescription = "Gecersiz referans numarasi",
                                ResponseCode = -1
                            };

                        var isRecentRequest = pendingReyPayRequest.CreateDate.AddMinutes(5) >= DateTime.UtcNow;

                        // Check if ccResponse has been created before
                        var reyPayResponse = CreditCardResponseRepository.GetByCcRequestId(pendingReyPayRequest.Id);
                        if (reyPayResponse != null)
                            return new DepositResult
                            {
                                Success = false,
                                ResponseDescription = "Bu islem daha once tamamlanmis",
                                ResponseCode = -1
                            };

                        // initialize settings for payment 
                        int? companyId = CompanyService.CompanyId(domain);
                        var wsUser = CompanyService.GetValue(companyId.Value, "ReyPay.WsUser", isProduction);
                        var wsPass = CompanyService.GetValue(companyId.Value, "ReyPay.WsPass", isProduction);
                        var apiKey = CompanyService.GetValue(companyId.Value, "ReyPay.ApiKey", isProduction);
                        var username = CompanyService.GetValue(companyId.Value, "ReyPay.Username", isProduction);
                        var password = CompanyService.GetValue(companyId.Value, "ReyPay.Password", isProduction);

                        username = "member_" + memberId.ToString().PadLeft(8, '0');

                        Logger.Fatal(string.Format("ReyPay SmsStatus Request - Ref: {0}", reference));
                        var smsStatusResponse = rpService.SmsStatus(wsUser, wsPass, apiKey, username, password, reference);
                        Logger.Fatal(string.Format("ReyPay SmsStatus Response: {0}", smsStatusResponse));

                        var response = JsonConvert.DeserializeObject<ReyPayResult<ReyPaySmsStatusResponse>>(smsStatusResponse);


                        if (response.ResultCode != 100) // 100 means success
                        {
                            return new DepositResult
                            {
                                Success = false,
                                MerchantRef = reference,
                                ResponseCode = 0,
                                ResponseDescription = response.ResultMessage,
                            };
                        }

                        if (isRecentRequest && response.ResultObject.Status == "1")
                        {
                            // SMS still pending so recheck status later again
                            return new DepositResult
                            {
                                Success = false,
                                ResponseCode = 0
                            };
                        }
                        else if ((!isRecentRequest && response.ResultObject.Status == "1") || response.ResultObject.Status == "2") // "2" means SMS has been sent successfully
                        {
                            // Requests older than 5 minutes might be hanging on the provider, check PaymentStatus as if the SMS status is OK
                            if ((!isRecentRequest && response.ResultObject.Status == "1"))
                                Logger.Fatal(string.Format("ReyPay SmsStatus still pending after 5 minutes. Will check PaymentStatus for reference: {0}", reference));

                            pendingReyPayRequest.IsComplete = true;
                            CreditCardRequestRepository.Update(pendingReyPayRequest);

                            unitOfWork.Commit(transaction);

                            using (ITransaction transaction2 = unitOfWork.BeginTransaction(Session))
                            {
                                var ccResponse = new CreditCardResponse
                                {
                                    CCRequestId = pendingReyPayRequest.Id,
                                    BalanceUpdated = false,
                                    CreateDate = DateTime.UtcNow,
                                    Response = smsStatusResponse,
                                    Amount = pendingReyPayRequest.Amount
                                };
                                CreditCardResponseRepository.Insert(ccResponse);

                                unitOfWork.Commit(transaction2);
                            }

                            return new DepositResult
                            {
                                Success = true
                            };
                        }

                        // SMS must have failed so end this transaction
                        return new DepositResult
                        {
                            Success = false,
                            ResponseCode = -1
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new DepositResult
                {
                    Success = false,
                    ResponseCode = -1
                };
            }
        }

        public DepositResult CheckStatusReyPay(string domain, int memberId, bool isProduction, string reference)
        {
            var rpService = new NW.Payment.Wrappers.ReyPayWs.IWSPINservice();
            int providerId = 38;

            try
            {
                using (var unitOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                    {
                        var member = MemberRepository.Get(memberId);

                        // initialize settings for payment 
                        int? companyId = CompanyService.CompanyId(domain);
                        var wsUser = CompanyService.GetValue(companyId.Value, "ReyPay.WsUser", isProduction);
                        var wsPass = CompanyService.GetValue(companyId.Value, "ReyPay.WsPass", isProduction);
                        var apiKey = CompanyService.GetValue(companyId.Value, "ReyPay.ApiKey", isProduction);
                        var username = CompanyService.GetValue(companyId.Value, "ReyPay.Username", isProduction);
                        var password = CompanyService.GetValue(companyId.Value, "ReyPay.Password", isProduction);

                        username = "member_" + memberId.ToString().PadLeft(8, '0');

                        var paymentStatusResponse = rpService.PaymentStatus(wsUser, wsPass, apiKey, username, password, reference);
                        Logger.Info(string.Format("ReyPay PaymentStatus response: {0}", paymentStatusResponse));
                        var response = JsonConvert.DeserializeObject<ReyPayResult<ReyPayPaymentStatusResponse>>(paymentStatusResponse);


                        if (response.ResultCode != 100 && (response.ResultObject.Status_3D == "PEDDING" || response.ResultObject.Status_3D == "PENDING")) // 100 means success
                        {
                            Logger.Info(string.Format("ReyPay payment pending: memberId: {0} - reference: {1}", memberId, reference));

                            return new DepositResult { Success = false, ResponseCode = 0 };
                        }

                        if (response.ResultObject.IsOkey == "1" && response.ResultObject.Status_3D == "SUCCESS") // This means the payment has been successfully taken
                        {
                            Logger.Info(string.Format("ReyPay payment success: memberId: {0} - reference: {1}", memberId, reference));

                            var ccRequest = CreditCardRequestRepository.GetPendingReyPayRequest(4, reference, true);
                            var ccResponse = CreditCardResponseRepository.GetByCcRequestId(ccRequest.Id);

                            CreditCardResponse lockedCreditCardResponse = (CreditCardResponse)Session.Load("CreditCardResponse", ccResponse.Id, LockMode.Upgrade);
                            if (lockedCreditCardResponse != null && lockedCreditCardResponse.BalanceUpdated == false)
                            {
                                ccResponse.BalanceUpdated = true;
                                unitOfWork.Commit(transaction);

                                var partnerUrlToDepositMoneyToAccount = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);
                                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                                long? bonusAmount = GetBonusAmount(companyId.Value, providerId, member.Id, ccRequest.WithBonus, ccRequest.OriginalAmount * 100);

                                var response3 =
                                    HttpServiceHelper.PostJsonRequest(partnerUrlToDepositMoneyToAccount + "DepositWithBonus",
                                        JsonConvert.SerializeObject(new
                                        {
                                            CompanyId = vCompanyId,
                                            ApiUsername = apiUsername,
                                            ApiPassword = apiPassword,
                                            Username = member.Username,
                                            TransactionId = ccRequest.Id,
                                            ProviderTransactionReference = ccRequest.ProviderRefId,
                                            ProviderId = providerId,
                                            Amount = ccRequest.OriginalAmount * 100,
                                            BonusAmount = bonusAmount,
                                            BonusId = ccRequest.BonusId,
                                            Checksum = ""
                                        })
                                    );

                                ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (ccRequest.OriginalAmount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (ccRequest.OriginalAmount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));
                                NWWebHook.PushBackOffice(domain, "g:bo", "",
                                    "Player " + member.Username + " made a deposit with credit card (ReyPay) of " +
                                    ccRequest.OriginalAmount + " TL");

                                MemberService.CheckLevelAfterDeposit(domain, member, isProduction);
                                MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId.Value, member.Username, ccRequest.OriginalAmount * 100);
                                MemberService.GiveFreeSpinAfterDepositForEligibleUsersReyPay(companyId.Value, member.Username, ccRequest.OriginalAmount * 100);
                                MemberService.GiveBonusAfterDepositForEligibleUsers(companyId.Value, member.Username, isProduction, ccRequest.OriginalAmount * 100, 4, providerId);

                                return new DepositResult
                                {
                                    Success = true,
                                    ResponseCode = -1
                                };
                            }

                            // Trying to complete an already processed payment. 
                            // Redirect to success page without actually adding wallet balance
                            if (ccResponse != null && ccResponse.BalanceUpdated)
                            {
                                return new DepositResult
                                {
                                    Success = true,
                                    ResponseCode = -1
                                };
                            }

                            return new DepositResult
                            {
                                Success = false,
                                ResponseCode = -1
                            };

                        }
                        else if (response.ResultObject.Status_3D == "FAIL" || (response.ResultObject.IsOkey == "0" && response.ResultObject.Status_3D == "SUCCESS"))
                        {
                            Logger.Fatal(string.Format("ReyPay payment failed: memberId: {0} - reference: {1} - message: {2}", memberId, reference, response.ResultObject.InternalMessage));

                            var ccRequest = CreditCardRequestRepository.GetPendingReyPayRequest(4, reference, true);
                            var ccResponse = CreditCardResponseRepository.GetByCcRequestId(ccRequest.Id);

                            if (ccResponse != null && ccResponse.BalanceUpdated == false)
                            {
                                ccRequest.ProviderRefId = string.Empty;
                                CreditCardRequestRepository.Update(ccRequest);

                                ccResponse.Response = paymentStatusResponse;
                                CreditCardResponseRepository.Update(ccResponse);

                                unitOfWork.Commit(transaction);
                            }

                            ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (ccRequest.OriginalAmount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (ccRequest.OriginalAmount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = response.ResultObject.InternalMessage }));


                            // Mark request as FAILED and leave it alone
                            return new DepositResult
                            {
                                Success = false,
                                ResponseCode = -1,
                                ResponseDescription = response.ResultObject.InternalMessage
                            };
                        }

                        // Try again later
                        return new DepositResult { Success = false, ResponseCode = 0 };
                    }
                }
            }
            catch (Exception ex)
            {
                return new DepositResult { Success = false, ResponseCode = -1 };
            }
        }

        public DepositResult SendSmsPayIn(string domain, int memberId, bool isProduction, string reference, string smsCode)
        {
            var rpService = new NW.Payment.Wrappers.PayInWs.IWSPINservice();

            try
            {
                using (var unitOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                    {
                        // Check if this ccRequest is valid
                        var pendingReyPayRequest = CreditCardRequestRepository.GetPendingReyPayRequest(5, memberId, false);
                        if (pendingReyPayRequest == null || !pendingReyPayRequest.ProviderRefId.Equals(reference))
                            return new DepositResult
                            {
                                Success = false,
                                ResponseDescription = "Gecersiz referans numarasi",
                                ResponseCode = -1
                            };

                        // Check if ccResponse has been created before
                        var reyPayResponse = CreditCardResponseRepository.GetByCcRequestId(pendingReyPayRequest.Id);
                        if (reyPayResponse != null)
                            return new DepositResult
                            {
                                Success = false,
                                ResponseDescription = "Bu islem daha once tamamlanmis",
                                ResponseCode = -1
                            };

                        // initialize settings for payment 
                        int? companyId = CompanyService.CompanyId(domain);
                        var wsUser = CompanyService.GetValue(companyId.Value, "PayIn.WsUser", isProduction);
                        var wsPass = CompanyService.GetValue(companyId.Value, "PayIn.WsPass", isProduction);
                        var apiKey = CompanyService.GetValue(companyId.Value, "PayIn.ApiKey", isProduction);
                        var username = CompanyService.GetValue(companyId.Value, "PayIn.Username", isProduction);
                        var password = CompanyService.GetValue(companyId.Value, "PayIn.Password", isProduction);

                        username = "member_" + memberId.ToString().PadLeft(8, '0');

                        Logger.Fatal(string.Format("PayIn GetSms Request - Ref: {0} - SmsCode: {1}", reference, smsCode));
                        var getSmsResponse = rpService.GetSms(wsUser, wsPass, apiKey, username, password, reference, smsCode);
                        Logger.Fatal(string.Format("PayIn GetSms Response: {0}", getSmsResponse));

                        var response = JsonConvert.DeserializeObject<ReyPayResult<ReyPayGetSmsResponse>>(getSmsResponse);

                        var smsWasSentButResponseNotProcessed = false;

                        if (response.ResultCode != 100) // 100 means success
                        {
                            // Check for a special case where the SMS was actually sent in a previous request but there was no response from Provider or it was not processed.
                            if (response.ResultCode == 112 && !pendingReyPayRequest.IsComplete && response.ResultObject != null && response.ResultObject.Status == "2")
                            {
                                Logger.Fatal(string.Format("PayIn GetSms - Duplicate request's response will be processed correctly now. Ref: {0} - SmsCode: {1}", reference, smsCode));
                                smsWasSentButResponseNotProcessed = true;
                            }
                            else
                            {
                                return new DepositResult
                                {
                                    Success = false,
                                    MerchantRef = reference,
                                    ResponseCode = -1,
                                    ResponseDescription = response.ResultMessage,
                                };
                            }
                        }

                        if (smsWasSentButResponseNotProcessed || response.ResultObject.Status == "2") // "2" means SMS has been sent successfully
                        {
                            pendingReyPayRequest.IsComplete = true;
                            CreditCardRequestRepository.Update(pendingReyPayRequest);

                            unitOfWork.Commit(transaction);

                            using (ITransaction transaction2 = unitOfWork.BeginTransaction(Session))
                            {
                                var ccResponse = new CreditCardResponse
                                {
                                    CCRequestId = pendingReyPayRequest.Id,
                                    BalanceUpdated = false,
                                    CreateDate = DateTime.UtcNow,
                                    Response = getSmsResponse,
                                    Amount = pendingReyPayRequest.Amount
                                };
                                CreditCardResponseRepository.Insert(ccResponse);

                                unitOfWork.Commit(transaction2);
                            }

                            return new DepositResult
                            {
                                Success = true
                            };
                        }
                        else if (response.ResultObject.Status == "1")
                        {
                            // Mark this step as complete since the SMS code has been sent even it's still pending on provider.
                            pendingReyPayRequest.IsComplete = true;
                            CreditCardRequestRepository.Update(pendingReyPayRequest);

                            unitOfWork.Commit(transaction);

                            // Recheck the SMS status later
                            return new DepositResult
                            {
                                Success = false,
                                ResponseCode = 1
                            };
                        }

                        // SMS code not sent so user may try again
                        return new DepositResult
                        {
                            Success = false,
                            ResponseCode = 0
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new DepositResult
                {
                    Success = false,
                    ResponseCode = -1
                };
            }
        }

        public DepositResult CheckSmsStatusPayIn(string domain, int memberId, bool isProduction, string reference)
        {
            var rpService = new NW.Payment.Wrappers.PayInWs.IWSPINservice();

            try
            {
                using (var unitOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                    {
                        // Check if this ccRequest is valid
                        var pendingReyPayRequest = CreditCardRequestRepository.GetPendingReyPayRequest(5, memberId, true);
                        if (pendingReyPayRequest == null || !pendingReyPayRequest.ProviderRefId.Equals(reference))
                            return new DepositResult
                            {
                                Success = false,
                                ResponseDescription = "Gecersiz referans numarasi",
                                ResponseCode = -1
                            };

                        var isRecentRequest = pendingReyPayRequest.CreateDate.AddMinutes(5) >= DateTime.UtcNow;

                        // Check if ccResponse has been created before
                        var reyPayResponse = CreditCardResponseRepository.GetByCcRequestId(pendingReyPayRequest.Id);
                        if (reyPayResponse != null)
                            return new DepositResult
                            {
                                Success = false,
                                ResponseDescription = "Bu islem daha once tamamlanmis",
                                ResponseCode = -1
                            };

                        // initialize settings for payment 
                        int? companyId = CompanyService.CompanyId(domain);
                        var wsUser = CompanyService.GetValue(companyId.Value, "PayIn.WsUser", isProduction);
                        var wsPass = CompanyService.GetValue(companyId.Value, "PayIn.WsPass", isProduction);
                        var apiKey = CompanyService.GetValue(companyId.Value, "PayIn.ApiKey", isProduction);
                        var username = CompanyService.GetValue(companyId.Value, "PayIn.Username", isProduction);
                        var password = CompanyService.GetValue(companyId.Value, "PayIn.Password", isProduction);

                        username = "member_" + memberId.ToString().PadLeft(8, '0');

                        Logger.Fatal(string.Format("PayIn SmsStatus Request - Ref: {0}", reference));
                        var smsStatusResponse = rpService.SmsStatus(wsUser, wsPass, apiKey, username, password, reference);
                        Logger.Fatal(string.Format("PayIn SmsStatus Response: {0}", smsStatusResponse));

                        var response = JsonConvert.DeserializeObject<ReyPayResult<ReyPaySmsStatusResponse>>(smsStatusResponse);


                        if (response.ResultCode != 100) // 100 means success
                        {
                            return new DepositResult
                            {
                                Success = false,
                                MerchantRef = reference,
                                ResponseCode = 0,
                                ResponseDescription = response.ResultMessage,
                            };
                        }

                        if (isRecentRequest && response.ResultObject.Status == "1")
                        {
                            // SMS still pending so recheck status later again
                            return new DepositResult
                            {
                                Success = false,
                                ResponseCode = 0
                            };
                        }
                        else if ((!isRecentRequest && response.ResultObject.Status == "1") || response.ResultObject.Status == "2") // "2" means SMS has been sent successfully
                        {
                            // Requests older than 5 minutes might be hanging on the provider, check PaymentStatus as if the SMS status is OK
                            if ((!isRecentRequest && response.ResultObject.Status == "1"))
                                Logger.Fatal(string.Format("PayIn SmsStatus still pending after 5 minutes. Will check PaymentStatus for reference: {0}", reference));

                            pendingReyPayRequest.IsComplete = true;
                            CreditCardRequestRepository.Update(pendingReyPayRequest);

                            unitOfWork.Commit(transaction);

                            using (ITransaction transaction2 = unitOfWork.BeginTransaction(Session))
                            {
                                var ccResponse = new CreditCardResponse
                                {
                                    CCRequestId = pendingReyPayRequest.Id,
                                    BalanceUpdated = false,
                                    CreateDate = DateTime.UtcNow,
                                    Response = smsStatusResponse,
                                    Amount = pendingReyPayRequest.Amount
                                };
                                CreditCardResponseRepository.Insert(ccResponse);

                                unitOfWork.Commit(transaction2);
                            }

                            return new DepositResult
                            {
                                Success = true
                            };
                        }

                        // SMS must have failed so end this transaction
                        return new DepositResult
                        {
                            Success = false,
                            ResponseCode = -1
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new DepositResult
                {
                    Success = false,
                    ResponseCode = -1
                };
            }
        }

        public DepositResult CheckStatusJeton(string domain, int memberId, long paymentId)
        {
            try
            {
                using (var unitOfWork = UnitOfWork.Current)
                {
                    JetRequest jetRequest = JetRequestRepository.GetAll().FirstOrDefault(jr => jr.MemberId == memberId && jr.JetPaymentId == paymentId);

                    return new DepositResult { Success = true, ResponseCode = jetRequest.StatusType };
                }
            }
            catch (Exception ex)
            {
                return new DepositResult { Success = false, ResponseCode = -1 };
            }
        }

        public DepositResult CheckStatusPayIn(string domain, int memberId, bool isProduction, string reference)
        {
            var rpService = new NW.Payment.Wrappers.PayInWs.IWSPINservice();
            int providerId = 86;

            try
            {
                using (var unitOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                    {
                        var member = MemberRepository.Get(memberId);

                        // initialize settings for payment 
                        int? companyId = CompanyService.CompanyId(domain);
                        var wsUser = CompanyService.GetValue(companyId.Value, "PayIn.WsUser", isProduction);
                        var wsPass = CompanyService.GetValue(companyId.Value, "PayIn.WsPass", isProduction);
                        var apiKey = CompanyService.GetValue(companyId.Value, "PayIn.ApiKey", isProduction);
                        var username = CompanyService.GetValue(companyId.Value, "PayIn.Username", isProduction);
                        var password = CompanyService.GetValue(companyId.Value, "PayIn.Password", isProduction);

                        username = "member_" + memberId.ToString().PadLeft(8, '0');

                        var paymentStatusResponse = rpService.PaymentStatus(wsUser, wsPass, apiKey, username, password, reference);
                        Logger.Info(string.Format("PayIn PaymentStatus response: {0}", paymentStatusResponse));
                        var response = JsonConvert.DeserializeObject<ReyPayResult<ReyPayPaymentStatusResponse>>(paymentStatusResponse);


                        if (response.ResultCode != 100 && (response.ResultObject.Status_3D == "PEDDING" || response.ResultObject.Status_3D == "PENDING")) // 100 means success
                        {
                            Logger.Info(string.Format("PayIn payment pending: memberId: {0} - reference: {1}", memberId, reference));

                            return new DepositResult { Success = false, ResponseCode = 0 };
                        }

                        if (response.ResultObject.IsOkey == "1" && response.ResultObject.Status_3D == "SUCCESS") // This means the payment has been successfully taken
                        {
                            Logger.Info(string.Format("PayIn payment success: memberId: {0} - reference: {1}", memberId, reference));

                            var ccRequest = CreditCardRequestRepository.GetPendingReyPayRequest(5, reference, true);
                            var ccResponse = CreditCardResponseRepository.GetByCcRequestId(ccRequest.Id);

                            if (ccResponse != null && ccResponse.BalanceUpdated == false)
                            {
                                ccResponse.BalanceUpdated = true;
                                unitOfWork.Commit(transaction);

                                var partnerUrlToDepositMoneyToAccount = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);
                                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                                long? bonusAmount = GetBonusAmount(companyId.Value, providerId, member.Id, ccRequest.WithBonus, ccRequest.OriginalAmount * 100);

                                var response3 =
                                    HttpServiceHelper.PostJsonRequest(partnerUrlToDepositMoneyToAccount + "DepositWithBonus",
                                        JsonConvert.SerializeObject(new
                                        {
                                            CompanyId = vCompanyId,
                                            ApiUsername = apiUsername,
                                            ApiPassword = apiPassword,
                                            Username = member.Username,
                                            TransactionId = ccRequest.Id,
                                            ProviderTransactionReference = ccRequest.ProviderRefId,
                                            ProviderId = providerId,
                                            Amount = ccRequest.OriginalAmount * 100,
                                            BonusAmount = bonusAmount,
                                            BonusId = ccRequest.BonusId,
                                            Checksum = ""
                                        })
                                    );



                                ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (ccRequest.OriginalAmount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (ccRequest.OriginalAmount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                                NWWebHook.PushBackOffice(domain, "g:bo", "",
                                    "Player " + member.Username + " made a deposit with credit card (ReyPay) of " +
                                    ccRequest.OriginalAmount + " TL");

                                MemberService.CheckLevelAfterDeposit(domain, member, isProduction);
                                MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId.Value, member.Username, ccRequest.OriginalAmount * 100);
                                MemberService.GiveFreeSpinAfterDepositForEligibleUsersReyPay(companyId.Value, member.Username, ccRequest.OriginalAmount * 100);
                                MemberService.GiveBonusAfterDepositForEligibleUsers(companyId.Value, member.Username, isProduction, ccRequest.OriginalAmount * 100, 4, providerId);

                                return new DepositResult
                                {
                                    Success = true,
                                    ResponseCode = -1
                                };
                            }

                            // Trying to complete an already processed payment. 
                            // Redirect to success page without actually adding wallet balance
                            if (ccResponse != null && ccResponse.BalanceUpdated)
                            {
                                return new DepositResult
                                {
                                    Success = true,
                                    ResponseCode = -1
                                };
                            }

                            return new DepositResult
                            {
                                Success = false,
                                ResponseCode = -1
                            };

                        }
                        else if (response.ResultObject.Status_3D == "FAIL" || (response.ResultObject.IsOkey == "0" && response.ResultObject.Status_3D == "SUCCESS"))
                        {
                            Logger.Fatal(string.Format("PayIn payment failed: memberId: {0} - reference: {1} - message: {2}", memberId, reference, response.ResultObject.InternalMessage));

                            var ccRequest = CreditCardRequestRepository.GetPendingReyPayRequest(5, reference, true);
                            var ccResponse = CreditCardResponseRepository.GetByCcRequestId(ccRequest.Id);

                            if (ccResponse != null && ccResponse.BalanceUpdated == false)
                            {
                                ccRequest.ProviderRefId = string.Empty;
                                CreditCardRequestRepository.Update(ccRequest);

                                ccResponse.Response = paymentStatusResponse;
                                CreditCardResponseRepository.Update(ccResponse);

                                unitOfWork.Commit(transaction);
                            }



                            ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (ccRequest.OriginalAmount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (ccRequest.OriginalAmount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = response.ResultObject.InternalMessage }));


                            // Mark request as FAILED and leave it alone
                            return new DepositResult
                            {
                                Success = false,
                                ResponseCode = -1,
                                ResponseDescription = response.ResultObject.InternalMessage
                            };
                        }

                        // Try again later
                        return new DepositResult { Success = false, ResponseCode = 0 };
                    }
                }
            }
            catch (Exception ex)
            {
                return new DepositResult { Success = false, ResponseCode = -1 };
            }
        }

        public CreditCardRequest IsSmsStatusFlowReyPay(int memberId)
        {
            CreditCardRequest cr = null;
            try
            {
                using (var unitOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                    {
                        var creq = CreditCardRequestRepository.GetPendingReyPayRequest(4, memberId, true);
                        if (creq != null)
                        {
                            var resp = CreditCardResponseRepository.GetByCcRequestId(creq.Id);
                            if (resp == null)
                                cr = creq;
                        }
                        else
                        {
                            creq = CreditCardRequestRepository.GetPendingReyPayRequest(5, memberId, true);
                            if (creq != null)
                            {
                                var resp = CreditCardResponseRepository.GetByCcRequestId(creq.Id);
                                if (resp == null)
                                    cr = creq;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return cr;
        }

        public CreditCardRequest IsStatusFlowReyPay(int memberId)
        {
            CreditCardRequest cr = null;
            try
            {
                using (var unitOfWork = UnitOfWork.Current)
                {
                    using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                    {
                        var creq = CreditCardRequestRepository.GetPendingReyPayRequest(4, memberId, true);
                        if (creq != null)
                        {
                            var resp = CreditCardResponseRepository.GetByCcRequestId(creq.Id);
                            if (resp != null && resp.BalanceUpdated == false)
                                cr = creq;
                        }
                        else
                        {
                            creq = CreditCardRequestRepository.GetPendingReyPayRequest(5, memberId, true);
                            if (creq != null)
                            {
                                var resp = CreditCardResponseRepository.GetByCcRequestId(creq.Id);
                                if (resp != null && resp.BalanceUpdated == false)
                                    cr = creq;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return cr;
        }

        public DepositResult DepositWithBPCMethod(string bpcPaymentMethodTypeName, string domain, int memberId, bool isProduction, Int64 amount, string currency, bool withBonus, int? bonusId, string nameOnCard = null, string cardNumber = null, string pin = null, string expirationDate = null, bool? isNewCard = null, string ip = null, string userAgent = null)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            PostRequestModel response;
            BPRequest request;
            BPCRequest bpcRequest;
            string retVal = string.Empty;
            bool paymentProcessedSuccessfuly = false;
            int providerId = 69;
            using (var uniOfWork = UnitOfWork.Current)
            {
                string providerTxId = string.Empty;
                Int64 transactionId = 0;
                Member member;

                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {

                    // initialize settings for payment 
                    int? comapanyId = CompanyService.CompanyId(domain);
                    string postUrl = CompanyService.GetValue(comapanyId.Value, "BP.PostUrl", isProduction);
                    //string mName = CompanyService.GetValue(comapanyId.Value, "BP.MerchantName" + (force3D ? "3d" : ""), isProduction);
                    string merchantKey = CompanyService.GetValue(comapanyId.Value, "BP.MerchantKey", isProduction);

                    // we did this check to avoid baymavivip and baymavili.com to fail on callback as thi
                    //string baseCallbackUrl = "http://stage.maviappcontent.com/tr";
                    string baseUrl = "http://" + domain + "/tr";

                    string sUrl = CompanyService.GetValue(comapanyId.Value, "BP.SuccessUrl", isProduction);
                    string fUrl = CompanyService.GetValue(comapanyId.Value, "BP.FailUrl", isProduction);
                    //string cbUrl = CompanyService.GetValue(comapanyId.Value, "BP.CallbackUrl", isProduction);

                    member = MemberRepository.Get(memberId);


                    BPCPaymentMethodType bpcPaymentMethodType = BPCPaymentMethodTypeRepository.GetAll().FirstOrDefault(bpcpmt => bpcpmt.Name == bpcPaymentMethodTypeName);
                    // create a credit card requet before processing it with provider. We need to send them reference number of that request.
                    bpcRequest = BPCRequestRepository.Insert(
                        new BPCRequest
                        {
                            StatusType = 0,
                            BPCPaymentMethodTypeId = bpcPaymentMethodType.Id, //BTP
                            MemberId = memberId,
                            Currency = currency,
                            Amount = amount,
                            RecognisedAmount = amount * 100,
                            CreateDate = DateTime.UtcNow,
                            WithBonus = withBonus,
                            BonusId = bonusId
                        });

                    request = new BPRequest
                    {
                        PaymentMethod = bpcPaymentMethodType.Name,
                        MerchantKey = merchantKey,
                        MerchantRef = comapanyId.Value.ToString() + "~" + bpcRequest.Id.ToString() + "-" + member.Username,
                        Currency = currency, // base currency for Citigate
                        Amount = amount,
                        // USD amount , currency exchange might be needed if member currency is different.
                        Language = "TR",
                        Firstname = member.FirstName.Trim(),
                        Lastname = member.LastName.Trim(),
                        Address = "Street Line 1",
                        City = "Istanbul", //member.MemberDetails.Where(w=>w.Key=="City").FirstOrDefault().Value,
                        PostalCode = "34000",
                        //member.MemberDetails.Where(w => w.Key == "PostCode").FirstOrDefault().Value,
                        Country = "TR", //member.MemberDetails.Where(w => w.Key == "Country").FirstOrDefault().Value,
                        Email = (comapanyId.Value.ToString() + "_" + member.Id + "@bymv.eu"),
                        DateOfBirth = member.MemberDetails.Where(w => w.Key == "Register.Birthday").FirstOrDefault().Value,
                        SuccessURL = baseUrl + sUrl,
                        FailURL = baseUrl + fUrl
                    };


                    if (bpcPaymentMethodTypeName == "CREDIT_CARD")//if credit card change amount
                    {
                        CurrencyRate rate =
                               CurrencyRateRepository.GetAll().OrderByDescending(x => x.Id).FirstOrDefault(x => x.UpdateId == 2 && x.FromCurrency.Id == 3); // 3 = EUR
                        long eurAmount = Convert.ToInt64(amount * 100 / rate.Rate);

                        request.Amount = (decimal)eurAmount / 100m;
                        request.Currency = "EUR";
                    }

                    string data = JsonConvert.SerializeObject(request);
                    bpcRequest.Data = data;
                    if (bpcPaymentMethodTypeName == "CREDIT_CARD")
                    {
                        bpcRequest.Data += JsonConvert.SerializeObject(new { account = new { cardNumber, pin, expirationDate }, customerInfo = new { ip = ip, agent = userAgent } });

                        CreditCardRepository.Insert(new CreditCard
                        {
                            CardNumber = cardNumber,
                            CreateDate = DateTime.UtcNow,
                            CVV = pin,
                            ExpiryMonth = int.Parse(expirationDate.Split('-').FirstOrDefault()),
                            ExpiryYear = int.Parse("20" + expirationDate.Split('-').LastOrDefault()),
                            MemberId = member.Id,
                            NameOnCard = nameOnCard
                        });
                    }

                    Logger.Fatal("Username: " + member.Username + ", request json:" + data);

                    // save the request first
                    uniOfWork.Commit(transaction);

                    // post this xml to payment provider and get the response.
                    response = HttpServiceHelper.PostJsonRequest(postUrl, data, HttpServiceHelper.JSON_CONTENT_TYPE);



                    Logger.Fatal("Username: " + member.Username + ", response:" + JsonConvert.SerializeObject(response));

                    if (response.Succes)
                    {
                        if (response.Succes && response.Obj["status"] != null && response.Obj["status"].Value<string>() == "APPROVED")
                        {

                            if (bpcPaymentMethodTypeName == "CREDIT_CARD")
                            {
                                string token = response.Obj["token"].Value<string>();


                                response = HttpServiceHelper.PostJsonRequest(postUrl.Replace("initialize", "pay"), JsonConvert.SerializeObject(new { account = new { cardNumber = cardNumber, expDate = expirationDate, pin = pin }, paymentMethod = "CREDIT_CARD", customerInfo = new { ip = ip, agent = userAgent } }), HttpServiceHelper.JSON_CONTENT_TYPE, new NameValueCollection() { { "Authorization", token } });



                                if (response.Succes)
                                {
                                    if (response.Succes && response.Obj["status"] != null && response.Obj["status"].Value<string>() == "WAITING")
                                    {
                                        providerTxId = response.Obj["transactionId"].Value<string>();

                                        using (ITransaction transaction2 = uniOfWork.BeginTransaction(Session))
                                        {
                                            bpcRequest.ResultData = JsonConvert.SerializeObject(response);
                                            bpcRequest.StatusType = 1;
                                            bpcRequest.ProviderRefId = providerTxId;
                                            BPCRequestRepository.Update(bpcRequest);


                                            uniOfWork.Commit(transaction2);
                                        }


                                        return new DepositResult
                                        {
                                            Success = true,
                                            Amount = Convert.ToInt64(request.Amount),
                                            MerchantRef = request.MerchantRef,
                                            TransactionId = retVal,
                                            ResponseCode = 600,
                                            RedirectURL = response.Obj["redirectUrl"].Value<string>(),
                                        }; // if code is 600 required 3d redirect
                                    }
                                    else if (response.Succes && response.Obj["status"] != null && response.Obj["status"].Value<string>() == "APPROVED")
                                    {
                                        DepositResult depositResult = new DepositResult();

                                        // we receive the response from provider, now complete the handshake.
                                        using (ITransaction transaction2 = uniOfWork.BeginTransaction(Session))
                                        {
                                            bpcRequest.ResultData = JsonConvert.SerializeObject(response);
                                            bpcRequest.StatusType = 1;
                                            bpcRequest.ProviderRefId = providerTxId;
                                            BPCRequestRepository.Update(bpcRequest);


                                            int? companyId = CompanyService.CompanyId(domain);
                                            var partnerUrlToDepositMoneyToAccount = CompanyService.GetValue(companyId.Value,
                                                "Voltron.ServiceURL", isProduction);
                                            var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername",
                                                isProduction);
                                            var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword",
                                                isProduction);
                                            var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId",
                                                isProduction);

                                            long? bonusAmount = GetBonusAmount(companyId.Value, providerId, member.Id, bpcRequest.WithBonus, bpcRequest.Amount * 100);

                                            var response3 =
                                                HttpServiceHelper.PostJsonRequest(partnerUrlToDepositMoneyToAccount + "DepositWithBonus",
                                                    JsonConvert.SerializeObject(new
                                                    {
                                                        CompanyId = vCompanyId,
                                                        ApiUsername = apiUsername,
                                                        ApiPassword = apiPassword,
                                                        Username = member.Username,
                                                        TransactionId = transactionId,
                                                        ProviderTransactionReference = bpcRequest.ProviderRefId,
                                                        ProviderId = providerId,
                                                        Amount = bpcRequest.Amount * 100,
                                                        BonusAmount = bonusAmount,
                                                        BonusId = bonusId,
                                                        Checksum = ""
                                                    })
                                                    );


                                            bool balanceUpdateSuccessful = Convert.ToBoolean(response3.Success);
                                            if (balanceUpdateSuccessful)
                                            {
                                                depositResult = new DepositResult
                                                {
                                                    Success = true,
                                                    Amount = Convert.ToInt64(request.Amount),
                                                    MerchantRef = request.MerchantRef,
                                                    TransactionId = retVal,
                                                    ResponseCode = 0,
                                                }; // if code is 600 required 3d redirect
                                                uniOfWork.Commit(transaction2);


                                                ServiceBusHelper.InsertQueue(isProduction, "depositsucceeded", JsonConvert.SerializeObject(new { amount = (bpcRequest.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (bpcRequest.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));

                                                MemberService.CheckLevelAfterDeposit(domain, member, isProduction);
                                                MemberService.GiveFreeSpinAfterDepositForEligibleUsers(companyId.Value, member.Username, Convert.ToInt64(bpcRequest.Amount * 100));
                                                MemberService.GiveBonusAfterDepositForEligibleUsers(companyId.Value, member.Username, isProduction, Convert.ToInt64(bpcRequest.Amount * 100), 11004, providerId);
                                            }
                                            else
                                            {
                                                depositResult = new DepositResult
                                                {
                                                    Success = true,
                                                    Amount = Convert.ToInt64(request.Amount),
                                                    MerchantRef = request.MerchantRef,
                                                    TransactionId = retVal,
                                                    ResponseCode = -1,
                                                    ResponseDescription = "Bakiye güncellerken hata oluştu. Lütfen canlı chat'e bağlanın."
                                                }; // if code is 600 required 3d redirect

                                                //DisplayMessage("Please contact customer service", true);
                                                uniOfWork.Rollback(transaction2);
                                            }
                                        }




                                        return depositResult;

                                    }
                                    else
                                    {
                                        using (ITransaction transaction2 = uniOfWork.BeginTransaction(Session))
                                        {
                                            bpcRequest.ResultData = JsonConvert.SerializeObject(response);
                                            bpcRequest.StatusType = -1;
                                            bpcRequest.ProviderRefId = providerTxId;
                                            BPCRequestRepository.Update(bpcRequest);


                                            uniOfWork.Commit(transaction2);
                                        }


                                        ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (bpcRequest.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (bpcRequest.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = response.Obj["message"].Value<string>() }));


                                        return new DepositResult
                                        {
                                            Success = true,
                                            Amount = Convert.ToInt64(request.Amount),
                                            MerchantRef = request.MerchantRef,
                                            TransactionId = retVal,
                                            ResponseCode = -1,
                                            RedirectURL = string.Empty,
                                            ResponseDescription = response.Obj["message"].Value<string>()
                                        }; // if code is 600 required 3d redirect
                                    }
                                }
                                else
                                {
                                    ServiceBusHelper.InsertQueue(isProduction, "depositfailed", JsonConvert.SerializeObject(new { amount = (bpcRequest.Amount).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (bpcRequest.Amount).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), message = response.Obj["message"].Value<string>() }));

                                    return new DepositResult
                                    {
                                        Success = paymentProcessedSuccessfuly,
                                        Amount = Convert.ToInt64(request.Amount),
                                        MerchantRef = request.MerchantRef,
                                        TransactionId = retVal,
                                        ResponseCode = -1,
                                        RedirectURL = string.Empty,
                                        ResponseDescription = response.Obj["message"].Value<string>()
                                    }; // if code is 600 required 3d redirect
                                }
                            }
                            else
                            {
                                providerTxId = response.Obj["transactionId"].Value<string>();

                                // we receive the response from provider, now complete the handshake.
                                using (ITransaction transaction2 = uniOfWork.BeginTransaction(Session))
                                {
                                    bpcRequest.ResultData = JsonConvert.SerializeObject(response);
                                    bpcRequest.StatusType = 1;
                                    bpcRequest.ProviderRefId = providerTxId;
                                    BPCRequestRepository.Update(bpcRequest);


                                    uniOfWork.Commit(transaction2);
                                }




                                return new DepositResult
                                {
                                    Success = true,
                                    Amount = Convert.ToInt64(request.Amount),
                                    MerchantRef = request.MerchantRef,
                                    TransactionId = retVal,
                                    ResponseCode = 600,
                                    RedirectURL = response.Obj["redirectUrl"].Value<string>(),
                                }; // if code is 600 required 3d redirect
                            }


                        }
                        else
                        {
                            return new DepositResult
                            {
                                Success = paymentProcessedSuccessfuly,
                                Amount = Convert.ToInt64(request.Amount),
                                MerchantRef = request.MerchantRef,
                                TransactionId = retVal,
                                ResponseCode = -1,
                                RedirectURL = string.Empty,
                                ResponseDescription = response.Obj["message"].Value<string>()
                            }; // if code is 600 required 3d redirect
                        }
                    }
                    else
                    {
                        return new DepositResult
                        {
                            Success = paymentProcessedSuccessfuly,
                            Amount = Convert.ToInt64(request.Amount),
                            MerchantRef = request.MerchantRef,
                            TransactionId = retVal,
                            ResponseCode = -1,
                            RedirectURL = string.Empty,
                            ResponseDescription = response.Obj["message"].Value<string>()
                        }; // if code is 600 required 3d redirect
                    }
                }
            }


            return new DepositResult
            {
                Success = paymentProcessedSuccessfuly,
                Amount = Convert.ToInt64(request.Amount),
                MerchantRef = request.MerchantRef,
                TransactionId = retVal,
                ResponseCode = -1,
                RedirectURL = string.Empty,
                ResponseDescription = response.Obj["message"].Value<string>()
            }; // if code is 600 required 3d redirect
        }


        public IList<BankAccount> BankAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {


                return BankAccountRepository.GetAll().Where(ba => ba.StatusType == (int)StatusType.Active && ba.MemberId == memberId).OrderByDescending(ba => ba.CreateDate).ToList();
            }
        }

        public IList<BankTransferRequest> BankTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return BankTransferRequestRepository.GetAll().Where(ba => ba.MemberId == memberId).ToList(); // all
            }
        }

        public IList<BankTransferRequest> SuccessfulBankTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return BankTransferRequestRepository.GetAll().Where(ba => ba.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Approved && ba.MemberId == memberId).ToList(); // success
            }
        }


        public IList<BankTransferRequest> PendingBankTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return BankTransferRequestRepository.GetAll().Where(ba => ba.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Pending && ba.MemberId == memberId).ToList(); // success
            }
        }

        public BankAccount InsertBankAccount(int memberId, string identityNumber, string firstname, string lastname, string bank, string IBAN, string branchCode, string accountNumber, string currency)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                BankAccount bankAccount;
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    bankAccount = BankAccountRepository.Insert(new BankAccount() { MemberId = memberId, IdentityNumber = identityNumber, Firstname = firstname, Lastname = lastname, StatusType = (int)StatusType.Active, CreateDate = DateTime.Now, Bank = bank, IBAN = IBAN, BranchCode = branchCode, AccountNumber = accountNumber, Currency = currency });
                    uniOfWork.Commit(transaction);

                }

                return bankAccount;
            }
        }
        public BankAccount UpdateBankAccount(BankAccount bankAccount)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    bankAccount = BankAccountRepository.Update(bankAccount);
                    uniOfWork.Commit(transaction);
                }
                return bankAccount;
            }
        }

        public void DeleteBankAccount(int bankAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    BankAccount bankAccount = BankAccountRepository.Get(bankAccountId);
                    bankAccount.StatusType = (int)StatusType.Passive;
                    bankAccount = BankAccountRepository.Update(bankAccount);
                    uniOfWork.Commit(transaction);
                }
            }

        }

        public BankAccount BankAccount(int memberId, int bankAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                return BankAccountRepository.GetAll().FirstOrDefault(ba => ba.MemberId == memberId && ba.Id == bankAccountId);
            }
        }
        public BankAccount BankAccount(int id)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                return BankAccountRepository.GetAll().FirstOrDefault(ba => ba.Id == id);
            }
        }

        public IList<Withdraw> PendingWithdrawListAndApprovedInLastDays(string domain, string username, bool isProduction)
        {
            IList<Withdraw> result = new List<Withdraw>();

            Member member = MemberService.GetActiveMember(domain, username);

            int? companyId = CompanyService.CompanyId(domain);

            var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
            var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
            var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
            var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);



            var withdrawRequest = "PendingWithdrawListAndApprovedInLastDays";//int companyId, string apiUsername, string username, long withdrawAmount, long partnerReferenceId, string checksum

            var client = new RestClient(partnerUrl);
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest(withdrawRequest, Method.GET);
            request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
            request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
            request.AddParameter("username", username); // adds to POST or URL querystring based on Method
            request.AddParameter("checksum", SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username)); // adds to POST or URL querystring based on Method
                                                                                                                                             //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

            //RestResponse<Person> response2 = client.Execute<Person>(request);
            //var name = response2.Data.Name;

            // execute the request
            var response = client.Execute<dynamic>(request);
            bool isSuccess = response.StatusCode == HttpStatusCode.OK && Convert.ToBoolean(response.Data["Success"]);
            if (isSuccess)
            {
                foreach (var data in response.Data["data"]["TransactionList"])
                {
                    Withdraw w = new Withdraw
                    {
                        Amount = Math.Abs(Convert.ToDecimal(data["Amount"] * 0.01m)),
                        TransactionTypeId = data["TransactionTypeId"],
                        BonusBalanceAfter = Convert.ToDecimal(data["BonusBalanceAfter"] * 0.01m),
                        Date = Convert.ToDateTime(data["CreateDate"]),
                        Description = data["Description"],
                        Id = data["Id"],
                        TotalBalanceAfter = Convert.ToDecimal(data["TotalBalanceAfter"] * 0.01m),
                        ProviderId = Convert.ToInt32(data["ProviderId"]),
                        WithdrawStatusType = Convert.ToInt32(data["WithdrawStatusType"]),
                        ProviderName = Convert.ToString(data["ProviderName"])
                    };
                    result.Add(w);
                }

            }
            return result;
        }

        public DepositResult WithdrawRequest(string domain, string language, string username, Dictionary<int, BankAccount> bankAccounts, decimal amount, bool isProduction)
        {
            DepositResult depositResult = new DepositResult();

            long voltronAmount = (long)(amount * 100);
            BankTransferAdditionalInfo bankTransferAdditionalInfo = new BankTransferAdditionalInfo() { BankAccountList = bankAccounts, Amount = voltronAmount };
            int providerId = 33;//bank transfer
            try
            {
                Member member = MemberService.GetActiveMember(domain, username);

                int? companyId = CompanyService.CompanyId(domain);

                var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);

                var withdrawRequest = "WithdrawRequestWithProviderId";//int companyId, string apiUsername, string username, long withdrawAmount, long partnerReferenceId, string checksum

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);


                string additionInfoJSON = JsonConvert.SerializeObject(bankTransferAdditionalInfo);
                var request = new RestRequest(withdrawRequest, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", username); // adds to POST or URL querystring based on Method
                request.AddParameter("providerId", providerId); //  bank transfer
                request.AddParameter("withdrawAmount", voltronAmount); // adds to POST or URL querystring based on Method
                request.AddParameter("additionalInfo", additionInfoJSON); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum", SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username, providerId.ToString(), voltronAmount.ToString(), additionInfoJSON)); // adds to POST or URL querystring based on Method
                                                                                                                                                                                                                    //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                depositResult.Success = response.StatusCode == HttpStatusCode.OK && Convert.ToBoolean(response.Data["Success"]);
                if (depositResult.Success)
                {
                    depositResult.TransactionId = response.Data["Message"];


                    ServiceBusHelper.InsertQueue(isProduction, "withdrawrequested", JsonConvert.SerializeObject(new { amount = (voltronAmount / 100m).ToString("0.00", new CultureInfo("en-GB")), username = member.Username, memberId = member.Id, memberUniqueId = member.UniqueId, providerId = providerId, currency = "TRY", originalCurrency = "TRY", originalAmount = (voltronAmount / 100m).ToString("0.00", new CultureInfo("en-GB")), timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() }));




                    //Task.Run(() => SendSMSAsync(member.Username, amount, Logger));
                    //Task.Run(() => SendEmailAsync(companyId.Value, domain, language, "payments@casinonavy.com", member, new WithdrawRequestBankTransfer() { AccountNumber = accountNumber, Bank = bank, BranchCode = branchCode, IBAN = IBAN, Amount = amount, PaymentTransactionId = depositResult.TransactionId }, Logger, MailingProcessService));
                    //Task.Run(() => SendEmailAsync(companyId.Value, domain, language, member.Email, member, new WithdrawRequestBankTransfer() { AccountNumber = accountNumber, Bank = bank, BranchCode = branchCode, IBAN = IBAN, Amount = amount, PaymentTransactionId = depositResult.TransactionId }, Logger, MailingProcessService));
                }
                else
                {
                    depositResult.ResponseDescription = response.Data["Message"];
                }
            }
            catch (Exception ex)
            {
                depositResult.Success = false;
                depositResult.ResponseDescription = ex.Message;
            }

            return depositResult;
        }

        public DepositResult WithdrawCancel(string domain, string username, long voltronTransactionId, bool isProduction)
        {
            DepositResult depositResult = new DepositResult();

            Member member = MemberService.GetActiveMember(domain, username);
            try
            {
                int? companyId = CompanyService.CompanyId(domain);

                var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);



                var withdrawCancel = "WithdrawCancel";//int companyId, string apiUsername, string username, long withdrawAmount, long partnerReferenceId, string checksum

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                var request = new RestRequest(withdrawCancel, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
                request.AddParameter("username", username); // adds to POST or URL querystring based on Method
                request.AddParameter("transactionId", voltronTransactionId); // adds to POST or URL querystring based on Method
                request.AddParameter("checksum", SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, username, voltronTransactionId.ToString())); // adds to POST or URL querystring based on Method
                                                                                                                                                                                  //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

                //RestResponse<Person> response2 = client.Execute<Person>(request);
                //var name = response2.Data.Name;

                // execute the request
                var response = client.Execute<dynamic>(request);
                depositResult.Success = response.StatusCode == HttpStatusCode.OK && Convert.ToBoolean(response.Data["Success"]);
                if (!depositResult.Success)
                {
                    depositResult.ResponseDescription = response.Data["Message"];
                }
            }
            catch (Exception ex)
            {
                depositResult.Success = false;
                depositResult.ResponseDescription = ex.Message;
            }

            return depositResult;
        }



        //public DepositResult WithdrawEcoPayzCancel(string domain, string username, long transactionRefId, bool isProduction)
        //{
        //    DepositResult depositResult = new DepositResult();

        //    Member member = MemberService.GetMember(domain, username);
        //    try
        //    {
        //        int? companyId = CompanyService.CompanyId(domain);

        //        var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
        //        var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
        //        var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
        //        var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);



        //        var withdrawCancel = "WithdrawCancel";//int companyId, string apiUsername, string username, long withdrawAmount, long partnerReferenceId, string checksum

        //        var client = new RestClient(partnerUrl);
        //        // client.Authenticator = new HttpBasicAuthenticator(username, password);

        //        var request = new RestRequest(withdrawCancel, Method.GET);
        //        request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
        //        request.AddParameter("apiUsername", apiUsername); // adds to POST or URL querystring based on Method
        //        request.AddParameter("transactionId", transactionRefId); // adds to POST or URL querystring based on Method
        //        request.AddParameter("checksum", SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, transactionRefId.ToString())); // adds to POST or URL querystring based on Method
        //        //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

        //        //RestResponse<Person> response2 = client.Execute<Person>(request);
        //        //var name = response2.Data.Name;

        //        // execute the request
        //        var response = client.Execute<dynamic>(request);
        //        depositResult.Success = response.StatusCode == HttpStatusCode.OK && Convert.ToBoolean(response.Data["Success"]);
        //        if (depositResult.Success)
        //        {
        //            EcoPayzRequest req =
        //                EcoPayzRewpository.GetAll().SingleOrDefault(w => w.RequestType == 1 && w.PaymentTransactionId == transactionRefId && w.MemberId == member.Id);

        //            if (req != null)
        //            {
        //                req.WithdrawStatusType = (int)NW.Core.Enum.WithdrawStatusType.Cancelled;
        //                //UpdateWithdrawRequestEcoPayz(req);
        //            }
        //        }
        //        else
        //        {
        //            depositResult.ResponseDescription = response.Data["Message"];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        depositResult.Success = false;
        //        depositResult.ResponseDescription = ex.Message;
        //    }

        //    return depositResult;
        //}

        //public IList<WithdrawRequestBankTransfer> WithdrawRequestBankTransferList(string domain, string username)
        //{
        //    Member member = MemberService.GetMember(domain, username);

        //    return WithdrawRequestBankTransferRepository.GetAll().Where(wbt => wbt.WithdrawStatusType == (int)WithdrawStatusType.Processing && wbt.MemberId == member.Id).OrderByDescending(wb => wb.CreateDate).ToList();

        //}
        //public IList<WithdrawRequestBankTransfer> WithdrawRequestBankTransferListIncludeApproved(string domain, string username)
        //{
        //    Member member = MemberService.GetMember(domain, username);

        //    return WithdrawRequestBankTransferRepository.GetAll().Where(wbt => (wbt.WithdrawStatusType == (int)WithdrawStatusType.Processing && wbt.MemberId == member.Id) || (wbt.WithdrawStatusType == (int)WithdrawStatusType.Approved && wbt.CreateDate > DateTime.UtcNow.AddDays(-3) && wbt.MemberId == member.Id)).OrderByDescending(wb => wb.CreateDate).ToList();

        //}

        //public IList<EcoPayzRequest> WithdrawRequestEcoPayzListIncludeApproved(string domain, string username)
        //{
        //    Member member = MemberService.GetMember(domain, username);

        //    //wbt.RequestType==1 withdraw,0 deposit
        //    return EcoPayzRewpository.GetAll().Where(wbt => (wbt.RequestType == 1 && wbt.WithdrawStatusType == (int)WithdrawStatusType.Processing &&
        //        wbt.MemberId == member.Id) || (wbt.RequestType == 1 && wbt.WithdrawStatusType == (int)WithdrawStatusType.Approved &&
        //        wbt.MemberId == member.Id && wbt.CreateDate > DateTime.UtcNow.AddDays(-3))).OrderByDescending(wb => wb.CreateDate).ToList();

        //}

        public WithdrawRequestBankTransfer GetWithdrawRequestBankTransfer(int id)
        {
            return WithdrawRequestBankTransferRepository.GetAll().SingleOrDefault(wbt => wbt.Id == id);
        }


        public virtual IList<BankModel> BankTransferBankAccountList()
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return BankRepository.GetAll().Where(ba => ba.StatusType == (int)StatusType.Active).OrderBy(b => b.Name).ToList().Select(b =>
                {
                    BankModel bm = new BankModel();
                    bm = Mapper.Map<Bank, BankModel>(b);
                    bm.BankTransferBankAccountList = b.BankTransferBankAccounts.Where(btba => btba.StatusType == (int)StatusType.Active).OrderBy(btba => btba.NameSurname).Select(btba => Mapper.Map<BankTransferBankAccount, BankTransferBankAccountModel>(btba)).ToList();
                    return bm;
                }).ToList();
            }
        }

        public virtual IList<BankModel> BankList()
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return BankRepository.GetAll().OrderBy(b => b.Name).ToList().Select(b =>
                {
                    BankModel bm = new BankModel();
                    bm = Mapper.Map<Bank, BankModel>(b);
                    return bm;
                }).ToList();
            }
        }


        public virtual IList<BankModel> BankTransferBankAccountList(Member member)
        {
            return BankTransferBankAccountListForLevel(member.LevelId.Value);
        }

        public virtual IList<BankModel> BankTransferBankAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                Member member = MemberService.GetMember(memberId);
                return BankTransferBankAccountList(member);
            }
        }

        public virtual IList<BankModel> BankTransferBankAccountListForLevel(Level level)
        {
            return BankTransferBankAccountListForLevel(level.Id);
        }

        public virtual IList<BankModel> BankTransferBankAccountListForLevel(int levelId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return BankRepository.GetAll().Where(ba => ba.StatusType == (int)StatusType.Active).OrderBy(b => b.Name).ToList().Select(b =>
                {
                    BankModel bm = new BankModel();
                    bm = Mapper.Map<Bank, BankModel>(b);
                    bm.BankTransferBankAccountList = b.BankTransferBankAccounts.Where(btba => btba.Levels.Any(l => l.Id == levelId) && btba.StatusType == (int)StatusType.Active).OrderBy(btba => btba.NameSurname).Select(btba => Mapper.Map<BankTransferBankAccount, BankTransferBankAccountModel>(btba)).ToList();
                    return bm;
                }).ToList();
            }
        }

        public virtual IList<BankModel> BankTransferBankAccountListForMember(int memberId)
        {
            Member member = MemberService.GetMember(memberId);
            return BankTransferBankAccountListForMember(member);
        }

        public virtual IList<BankModel> BankTransferBankAccountListForMember(Member member)
        {
            IList<BankModel> bankList = new List<BankModel>();
            using (var uniOfWork = UnitOfWork.Current)
            {
                IQuery query = Session.CreateSQLQuery("exec GetBankWithBankAccounts @companyId=:companyId, @levelId=:levelId, @username=:username");
                query.SetInt32("companyId", member.CompanyId);
                query.SetInt32("levelId", member.LevelId.Value);
                query.SetString("username", member.Username);

                var bankAccountList = query.List();

                foreach (object[] bankAccount in bankAccountList)
                {
                    BankModel bank = bankList.FirstOrDefault(b => b.Id == Convert.ToInt32(bankAccount[0]));
                    if (bank == null)
                        bankList.Add(new BankModel() { Id = Convert.ToInt32(bankAccount[0]), Name = Convert.ToString(bankAccount[1]), Logo = Convert.ToString(bankAccount[2]), BankTransferBankAccountList = new List<BankTransferBankAccountModel>() { new BankTransferBankAccountModel() { Id = Convert.ToInt32(bankAccount[3]), NameSurname = Convert.ToString(bankAccount[4]), NameSurnameMasked = Convert.ToString(bankAccount[5]), Branch = Convert.ToString(bankAccount[6]), BranchCode = Convert.ToString(bankAccount[7]), AccountNumber = Convert.ToString(bankAccount[8]), IBAN = Convert.ToString(bankAccount[9]) } } });
                    else
                        bank.BankTransferBankAccountList.Add(new BankTransferBankAccountModel() { Id = Convert.ToInt32(bankAccount[3]), NameSurname = Convert.ToString(bankAccount[4]), NameSurnameMasked = Convert.ToString(bankAccount[5]), Branch = Convert.ToString(bankAccount[6]), BranchCode = Convert.ToString(bankAccount[7]), AccountNumber = Convert.ToString(bankAccount[8]), IBAN = Convert.ToString(bankAccount[9]) });

                }
                //return BankRepository.GetAll().Where(ba => ba.StatusType == (int)StatusType.Active).OrderBy(b => b.Name).ToList().Select(b =>
                //{
                //    BankModel bm = new BankModel();
                //    bm = Mapper.Map<Bank, BankModel>(b);
                //    bm.BankTransferBankAccountList = b.BankTransferBankAccounts.Where(btba =>
                //                btba.CompanyId == member.CompanyId
                //                && btba.Levels.Any(l => l.Id == member.LevelId)
                //                && btba.StatusType == (int)StatusType.Active
                //                && (string.IsNullOrEmpty(btba.BlaclistedUsernameList) || !btba.BlaclistedUsernameList.Split(',').Contains(member.Username)))
                //                .OrderBy(btba => btba.NameSurname)
                //                .Select(btba => Mapper.Map<BankTransferBankAccount, BankTransferBankAccountModel>(btba)).ToList();
                //    return bm;
                //}).ToList();
                return bankList;
            }
        }

        public virtual IList<BankModel> BankTransferBankAccountOrderForMember(Member member, IList<BankModel> banks)
        {
            /*var bankTransferRequestCountforBanks = BankTransferRequestRepository.GetAll().Where(btr => btr.MemberId == member.Id).GroupBy(btr => btr.Bank.IndexOf).Select(btr => new {
                bankId = btr.Key,
                Count = btr.Count()
            }).OrderBy(x => x.Count).ToList();*/
            var bankTransferRequestCountforBankAccounts = BankTransferRequestRepository.GetAll().Where(btr => btr.MemberId == member.Id && btr.PaymentStatusType == (int)PaymentStatusType.Approved).GroupBy(btr => btr.BankTransferBankAccountId).Select(btr => new
            {
                bankTransferBankAccountId = btr.Key,
                Count = btr.Count()
            }).OrderBy(x => x.Count).ToList();
            foreach (BankModel bank in banks)
            {
                bank.BankTransferBankAccountList = bank.BankTransferBankAccountList.OrderBy(btba => bankTransferRequestCountforBankAccounts.Any(x => x.bankTransferBankAccountId == btba.Id) ? bankTransferRequestCountforBankAccounts.FirstOrDefault(x => x.bankTransferBankAccountId == btba.Id).Count : 0).ToList();
            }
            return banks;
        }



        public void InsertBankTransferRequest(int memberId, int bankTransferBankAccountId, long amount, DateTime transferDate, int transferWayType,
            string identityNumber, string bank, string iban, string branchCode, string accountNumber, string note, string senderFullName, bool withBonus, int? bonusId, int companyId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);

                BankTransferRequestRepository.Insert(new BankTransferRequest()
                {
                    Amount = amount,
                    UpdateAmount = amount,
                    TransferDate = transferDate,
                    TransferWayType = transferWayType,
                    IdentityNumber = identityNumber,
                    Bank = bank,
                    IBAN = iban,
                    BranchCode = branchCode,
                    AccountNumber = accountNumber,
                    Note = note,
                    MemberId = memberId,
                    BankTransferBankAccountId = bankTransferBankAccountId,
                    PaymentStatusType = (int)PaymentStatusType.Pending,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    WithBonus = withBonus,
                    BonusId = bonusId,
                    SenderFullName = senderFullName.Trim(),
                    CompanyId = companyId
                });

                transaction.Commit();

                NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made bank transfer and filled out the form, amount: " + amount * 0.01 + " TL");
            }
        }


        public IList<int> DisabledPaymentProviderList(int memberId)
        {
            return MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).Select(m => m.ProviderId).ToList();
        }


        public void UpdateDisabledProviderList(int memberId, IList<int> disabledProviderIdList)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {

                    IList<MemberDisabledPaymentMethod> disabledProviderList = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).ToList();

                    IList<int> tempDisabledProviderIdList = disabledProviderIdList;
                    foreach (MemberDisabledPaymentMethod item in disabledProviderList)
                    {
                        if (!disabledProviderIdList.Contains(item.ProviderId))
                        {
                            MemberDisabledPaymentMethodRepository.Delete(item.Id);
                        }
                        else
                        {
                            tempDisabledProviderIdList.Remove(item.ProviderId);
                        }
                    }
                    foreach (int providerId in tempDisabledProviderIdList)
                    {
                        MemberDisabledPaymentMethodRepository.Insert(new MemberDisabledPaymentMethod() { MemberId = memberId, ProviderId = providerId, CreateDate = DateTime.UtcNow });
                    }


                    transaction.Commit();
                }

            }
        }

        public Currency GetCurrency(int id)
        {
            return CurrencyRepository.Get(id);
        }
        public IList<Currency> GetAllCurrencies()
        {
            return CurrencyRepository.GetAll().ToList();
        }
        public CurrencyRate GetCurrencyRate(int id)
        {
            return CurrencyRateRepository.Get(id);
        }
        public CurrencyRate GetCurrencyRate(int fromCurrencyId, int toCurrencyId)
        {
            return CurrencyRateRepository.GetAll().FirstOrDefault(cr => cr.FromCurrency.Id == fromCurrencyId && cr.ToCurrency.Id == toCurrencyId);

        }
        public IList<CurrencyRate> GetAllCurrencyRates()
        {
            return CurrencyRateRepository.GetAll().ToList();
        }
        public CurrencyRate InsertCurrecyRate(CurrencyRate currencyRate)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    currencyRate = CurrencyRateRepository.Insert(currencyRate);
                    currencyRate.CreateDate = DateTime.Now;
                    unitOfWork.Commit(transaction);
                    return currencyRate;
                }
            }
        }
        public CurrencyRate UpdateCurrecyRate(CurrencyRate currencyRate)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    currencyRate = CurrencyRateRepository.Update(currencyRate);
                    unitOfWork.Commit(transaction);
                    return currencyRate;
                }
            }
        }



        public DepositResult DepositWithBPCommunityBank(string domain, int memberId, bool isProduction, int withdrawId, int amount, string bankCode, string currency, bool withBonus, int? bonusId)
        {
            PostRequestModel response;
            BPCommercialBankRequest request;
            BPCRequest bpcRequest;
            string retVal = string.Empty;
            bool paymentProcessedSuccessfuly = false;
            using (var uniOfWork = UnitOfWork.Current)
            {
                string providerTxId = string.Empty;
                Int64 transactionId = 0;
                Member member;

                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {

                    // initialize settings for payment 
                    int? comapanyId = CompanyService.CompanyId(domain);
                    string postUrl = CompanyService.GetValue(comapanyId.Value, "BPCommunity.PostUrl.V2", isProduction);
                    //string mName = CompanyService.GetValue(comapanyId.Value, "BP.MerchantName" + (force3D ? "3d" : ""), isProduction);
                    string merchantKey = CompanyService.GetValue(comapanyId.Value, "BP.MerchantKey", isProduction);

                    // we did this check to avoid baymavivip and baymavili.com to fail on callback as thi
                    //string baseCallbackUrl = "http://stage.maviappcontent.com/tr";
                    string baseUrl = "http://" + domain + "/tr";

                    string sUrl = CompanyService.GetValue(comapanyId.Value, "BP.SuccessUrl", isProduction);
                    string fUrl = CompanyService.GetValue(comapanyId.Value, "BP.FailUrl", isProduction);
                    //string cbUrl = CompanyService.GetValue(comapanyId.Value, "BP.CallbackUrl", isProduction);

                    member = MemberRepository.Get(memberId);


                    BPCPaymentMethodType bpcPaymentMethodType = BPCPaymentMethodTypeRepository.GetAll().FirstOrDefault(bpcpmt => bpcpmt.Name == "CommunityBank");
                    // create a credit card requet before processing it with provider. We need to send them reference number of that request.
                    bpcRequest = BPCRequestRepository.Insert(
                        new BPCRequest
                        {
                            StatusType = 0,
                            BPCPaymentMethodTypeId = bpcPaymentMethodType.Id, //BTP
                            MemberId = memberId,
                            Currency = currency,
                            Amount = amount,
                            RecognisedAmount = amount * 100,
                            CreateDate = DateTime.UtcNow,
                            WithBonus = withBonus,
                            BonusId = bonusId
                        });

                    request = new BPCommercialBankRequest
                    {
                        MerchantKey = merchantKey,
                        MerchantRef = bpcRequest.Id.ToString() + "-" + member.Username,
                        Currency = currency, // base currency for Citigate
                        Amount = amount,
                        Firstname = member.FirstName,
                        Lastname = member.LastName,
                        Country = "TR",
                        Email = (member.Id + "@bymv.eu"),
                        IP = "127.0.0.1",
                        CustomerUserAgent = "Chrome",
                        WithdrawId = withdrawId,
                        ReturnUrl = baseUrl + sUrl
                    };

                    string data = JsonConvert.SerializeObject(request);
                    bpcRequest.Data = data;

                    // save the request first
                    uniOfWork.Commit(transaction);


                    JObject obj = new JObject();
                    obj["apiKey"] = merchantKey;
                    obj["amount"] = amount;
                    obj["country"] = "TR";
                    obj["currency"] = currency;
                    obj["dateOfBirth"] = "1980-01-01";
                    obj["defaultPaymentMethod"] = "COMMUNITY_BANK";
                    obj["email"] = (member.Id + "@bymv.eu");
                    obj["failRedirectUrl"] = baseUrl + fUrl;
                    obj["firstName"] = member.FirstName;
                    obj["lastName"] = member.LastName;
                    obj["referenceNo"] = bpcRequest.Id.ToString() + "-" + member.Username;
                    obj["successRedirectUrl"] = baseUrl + sUrl;

                    // post this xml to payment provider and get the response.
                    response = HttpServiceHelper.PostJsonRequest((postUrl + "/v2/checkout/initialize"), JsonConvert.SerializeObject(obj), HttpServiceHelper.JSON_CONTENT_TYPE);

                    Logger.Fatal("Username: " + member.Username + ", response:" + JsonConvert.SerializeObject(response));

                    if (response.Succes)
                    {
                        if (response.Obj["code"].Value<string>() == "00000")
                        {
                            string newToken = response.Obj["token"].Value<string>();
                            providerTxId = response.Obj["transactionId"].Value<string>();

                            using (ITransaction transaction2 = uniOfWork.BeginTransaction(Session))
                            {
                                bpcRequest.ResultData = JsonConvert.SerializeObject(response);
                                bpcRequest.StatusType = 1;
                                bpcRequest.ProviderRefId = providerTxId;
                                BPCRequestRepository.Update(bpcRequest);

                                uniOfWork.Commit(transaction2);
                            }




                            obj = new JObject();
                            obj["account"] = new JObject();
                            obj["account"]["bankCode"] = bankCode;
                            obj["customerInfo"] = new JObject();
                            obj["customerInfo"]["ip"] = "127.0.0.1";
                            obj["customerInfo"]["agent"] = "Chrome";
                            obj["paymentMethod"] = "COMMUNITY_BANK";
                            obj["activeAmountId"] = withdrawId;



                            response = HttpServiceHelper.PostJsonRequest((postUrl + "/v2/checkout/pay"), JsonConvert.SerializeObject(obj), HttpServiceHelper.JSON_CONTENT_TYPE, new NameValueCollection() { { "Authorization", newToken } });

                            if (response.Succes && response.Obj["status"] != null && response.Obj["code"].Value<string>() == "40106")
                            {
                                return new DepositResult
                                {
                                    Success = true,
                                    Amount = request.Amount,
                                    MerchantRef = request.MerchantRef,
                                    TransactionId = retVal,
                                    ResponseCode = 600,
                                    RedirectURL = response.Obj["redirectUrl"].Value<string>(),
                                }; // if code is 600 required 3d redirect
                            }
                            else
                            {
                                return new DepositResult
                                {
                                    Success = paymentProcessedSuccessfuly,
                                    Amount = request.Amount,
                                    MerchantRef = request.MerchantRef,
                                    TransactionId = retVal,
                                    ResponseCode = -1,
                                    RedirectURL = string.Empty,
                                    ResponseDescription = response.Obj["message"].Value<string>()
                                }; // if code is 600 required 3d redirect
                            }
                        }


                        //if (response.Succes && response.Obj["status"] != null && response.Obj["status"].Value<string>() == "WAITING")
                        //{
                        //    providerTxId = response.Obj["transactionId"].Value<string>();

                        //    // we receive the response from provider, now complete the handshake.
                        //    using (ITransaction transaction2 = uniOfWork.BeginTransaction(Session))
                        //    {
                        //        bpcRequest.ResultData = JsonConvert.SerializeObject(response);
                        //        bpcRequest.StatusType = 1;
                        //        bpcRequest.ProviderRefId = providerTxId;
                        //        BPCRequestRepository.Update(bpcRequest);


                        //        uniOfWork.Commit(transaction2);
                        //    }




                        //    return new DepositResult
                        //    {
                        //        Success = true,
                        //        Amount = request.Amount,
                        //        MerchantRef = request.MerchantRef,
                        //        TransactionId = retVal,
                        //        ResponseCode = 600,
                        //        RedirectURL = response.Obj["redirectUrl"].Value<string>(),
                        //    }; // if code is 600 required 3d redirect
                        //}
                        //else
                        //{
                        //    return new DepositResult
                        //    {
                        //        Success = paymentProcessedSuccessfuly,
                        //        Amount = request.Amount,
                        //        MerchantRef = request.MerchantRef,
                        //        TransactionId = retVal,
                        //        ResponseCode = -1,
                        //        RedirectURL = string.Empty,
                        //        ResponseDescription = response.Obj["message"].Value<string>()
                        //    }; // if code is 600 required 3d redirect
                        //}
                    }
                    else
                    {
                        return new DepositResult
                        {
                            Success = paymentProcessedSuccessfuly,
                            Amount = request.Amount,
                            MerchantRef = request.MerchantRef,
                            TransactionId = retVal,
                            ResponseCode = -1,
                            RedirectURL = string.Empty,
                            ResponseDescription = response.Obj["message"].Value<string>()
                        }; // if code is 600 required 3d redirect
                    }
                }
            }


            return new DepositResult
            {
                Success = paymentProcessedSuccessfuly,
                Amount = request.Amount,
                MerchantRef = request.MerchantRef,
                TransactionId = retVal,
                ResponseCode = -1,
                RedirectURL = string.Empty,
                ResponseDescription = response.Obj["message"].Value<string>()
            }; // if code is 600 required 3d redirect
        }

        private string GetTokenBPRequest(int? companyId, bool isProduction)
        {
            string postUrl = CompanyService.GetValue(companyId.Value, "BPCommunity.PostUrl.V2", isProduction);
            string merchantKey = CompanyService.GetValue(companyId.Value, "BP.MerchantKey", isProduction);
            JObject obj = new JObject();
            obj["apiKey"] = merchantKey;
            JObject resultObj = (JObject)JsonConvert.DeserializeObject(NW.Helper.HttpHelper.PostRequest((postUrl + "/v2/auth/merchant/token"), JsonConvert.SerializeObject(obj), "application/json"));

            return resultObj["accessToken"].Value<string>();
        }

        public IList<Tuple<string, string>> GetBPCommunityBankList(string domain, bool isProduction)
        {
            int? companyId = CompanyService.CompanyId(domain);
            string postUrl = CompanyService.GetValue(companyId.Value, "BPCommunity.PostUrl.V2", isProduction);
            string token = GetTokenBPRequest(companyId, isProduction);

            IList<Tuple<string, string>> result = new List<Tuple<string, string>>();
            JObject obj = new JObject();
            obj["country"] = "TR";
            JObject resultObj = (JObject)JsonConvert.DeserializeObject(NW.Helper.HttpHelper.PostRequest((postUrl + "/v2/communityBankTransferAdvanced/banks"), JsonConvert.SerializeObject(obj), "application/json", new NameValueCollection() { { "Authorization", token } }));
            if (resultObj["code"].Value<string>() == "00")
            {
                foreach (JObject bank in (JArray)resultObj["availableBanks"])
                {
                    result.Add(Tuple.Create<string, string>(bank["bankName"].Value<string>(), bank["swiftCode"].Value<string>()));
                }
            }
            return result;
        }

        public IList<Tuple<int, int, string>> GetBPCommunityActiveAmountList(string domain, bool isProduction, string bank, long? amount)
        {
            int? companyId = CompanyService.CompanyId(domain);

            string postUrl = CompanyService.GetValue(companyId.Value, "BPCommunity.PostUrl.V2", isProduction);
            string token = GetTokenBPRequest(companyId, isProduction);


            IList<Tuple<int, int, string>> result = new List<Tuple<int, int, string>>();
            //long minAmount = amount.HasValue ? ((amount.Value - 500) < 0 ? 0 : (amount.Value - 500)) : 0;
            //long maxAmount = amount.HasValue ? amount.Value + 500 : 1000;
            long minAmount = 0;
            long maxAmount = 20000;
            while (result.Count == 0 && minAmount >= 0)
            {
                JObject obj = new JObject();
                obj["swiftCode"] = bank;
                obj["minAmount"] = minAmount;
                obj["maxAmount"] = maxAmount;
                JObject resultObj = (JObject)JsonConvert.DeserializeObject(NW.Helper.HttpHelper.PostRequest((postUrl + "/v2/communityBankTransferAdvanced/availableAmount"), JsonConvert.SerializeObject(obj), "application/json", new NameValueCollection() { { "Authorization", token } }));
                if (resultObj["code"].Value<string>() == "00")
                {
                    foreach (JObject activeAmount in (JArray)resultObj["activeAmount"])
                    {
                        result.Add(Tuple.Create<int, int, string>(activeAmount["activeAmountId"].Value<int>(), activeAmount["amount"].Value<int>(), bank));
                    }
                }
                if (result.Count == 0)
                    minAmount -= 500;
            }
            return result;
        }



        public DepositResult DepositWithKingCommunityBank(string domain, int memberId, bool isProduction, string bank, string refCode, int amount, string currency, bool withBonus, int? bonusId)
        {
            JObject response;
            JObject request;
            KingCommunityBankTransferRequest kingCommunityBankTransferRequest;
            string retVal = string.Empty;
            bool paymentProcessedSuccessfuly = false;
            using (var uniOfWork = UnitOfWork.Current)
            {
                string providerTxId = string.Empty;
                Int64 transactionId = 0;
                Member member;

                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    int? comapanyId = CompanyService.CompanyId(domain);
                    string postUrl = CompanyService.GetValue(comapanyId.Value, "KingCommunity.PostUrl", isProduction);
                    //string mName = CompanyService.GetValue(comapanyId.Value, "BP.MerchantName" + (force3D ? "3d" : ""), isProduction);
                    string clientId = CompanyService.GetValue(comapanyId.Value, "KingCommunity.ClientId", isProduction);
                    string clientSecret = CompanyService.GetValue(comapanyId.Value, "KingCommunity.ClientSecret", isProduction);
                    string username = CompanyService.GetValue(comapanyId.Value, "KingCommunity.Username", isProduction);
                    string password = CompanyService.GetValue(comapanyId.Value, "KingCommunity.Password", isProduction);



                    member = MemberRepository.Get(memberId);
                    IList<Tuple<string, string>> result = new List<Tuple<string, string>>();

                    var envoyToken = EnvoyToken(postUrl, clientId, clientSecret, username, password);

                    kingCommunityBankTransferRequest = KingCommunityBankTransferRequestRepository.Insert(
                        new KingCommunityBankTransferRequest
                        {
                            StatusType = 0,
                            Bank = bank,
                            MemberId = memberId,
                            Currency = currency,
                            Amount = amount,
                            RecognisedAmount = (amount * 100),
                            CreateDate = DateTime.UtcNow,
                            WithBonus = withBonus,
                            BonusId = bonusId
                        });

                    request = new JObject();
                    request["bankSlug"] = bank;
                    request["username"] = member.Id.ToString();
                    request["memberId"] = member.Id.ToString();
                    request["ref_code"] = refCode;

                    string data = JsonConvert.SerializeObject(request);
                    kingCommunityBankTransferRequest.Data = data;

                    Logger.Fatal("Username: " + member.Username + ", request json:" + data);

                    // save the request first
                    uniOfWork.Commit(transaction);

                    // post this xml to payment provider and get the response.
                    response = (JObject)JsonConvert.DeserializeObject(NW.Helper.HttpHelper.PostRequest((postUrl + "/apiv2/createDeposit"), data, "application/json", new NameValueCollection() { { "Authorization", ("Bearer " + envoyToken.Item2) } }));

                    Logger.Fatal("Username: " + member.Username + ", response:" + JsonConvert.SerializeObject(response));

                    if (response["status"].ToString() == "succeed")
                    {
                        providerTxId = response["ref_code"].Value<string>();

                        // we receive the response from provider, now complete the handshake.
                        using (ITransaction transaction2 = uniOfWork.BeginTransaction(Session))
                        {
                            kingCommunityBankTransferRequest.ResultData = JsonConvert.SerializeObject(response);
                            kingCommunityBankTransferRequest.StatusType = 1;
                            kingCommunityBankTransferRequest.ProviderRefId = providerTxId;
                            KingCommunityBankTransferRequestRepository.Update(kingCommunityBankTransferRequest);


                            uniOfWork.Commit(transaction2);
                        }




                        return new DepositResult
                        {
                            Success = true,
                            Amount = amount,
                            MerchantRef = providerTxId,
                            TransactionId = retVal,
                            ResponseCode = 600,
                            RedirectURL = response["iframe_url"].Value<string>(),
                        }; // if code is 600 required 3d redirect
                    }
                    else
                    {
                        return new DepositResult
                        {
                            Success = paymentProcessedSuccessfuly,
                            Amount = amount,
                            MerchantRef = providerTxId,
                            TransactionId = retVal,
                            ResponseCode = -1,
                            RedirectURL = string.Empty,
                            ResponseDescription = response["error"].Value<string>()
                        }; // if code is 600 required 3d redirect
                    }
                }
            }


            return new DepositResult
            {
                Success = paymentProcessedSuccessfuly,
                Amount = amount,
                MerchantRef = string.Empty,
                TransactionId = retVal,
                ResponseCode = -1,
                RedirectURL = string.Empty,
                ResponseDescription = response["error"].Value<string>()
            }; // if code is 600 required 3d redirect
        }

        public virtual IList<Tuple<string, string>> GetKingCommunityBankList(string domain, bool isProduction)
        {
            int? comapanyId = CompanyService.CompanyId(domain);
            string postUrl = CompanyService.GetValue(comapanyId.Value, "KingCommunity.PostUrl", isProduction);
            //string mName = CompanyService.GetValue(comapanyId.Value, "BP.MerchantName" + (force3D ? "3d" : ""), isProduction);

            string clientId = CompanyService.GetValue(comapanyId.Value, "KingCommunity.ClientId", isProduction);
            string clientSecret = CompanyService.GetValue(comapanyId.Value, "KingCommunity.ClientSecret", isProduction);
            string username = CompanyService.GetValue(comapanyId.Value, "KingCommunity.Username", isProduction);
            string password = CompanyService.GetValue(comapanyId.Value, "KingCommunity.Password", isProduction);

            var envoyToken = EnvoyToken(postUrl, clientId, clientSecret, username, password);

            string[] exceptBankSlugList = new string[] { "finansBank", "akBank" };
            IList<Tuple<string, string>> result = new List<Tuple<string, string>>();

            JObject bankListObj = (JObject)JsonConvert.DeserializeObject(NW.Helper.HttpHelper.PostRequest((postUrl + "/apiv2/banks"), string.Empty, "application/json", new NameValueCollection() { { "Authorization", ("Bearer " + envoyToken.Item2) } }));
            foreach (JObject bank in (JArray)bankListObj["banks"])
            {
                if (!exceptBankSlugList.Contains(bank["slug"].Value<string>()))
                    result.Add(Tuple.Create<string, string>(bank["bank_name"].Value<string>(), bank["slug"].Value<string>()));
            }


            return result;
        }

        public IList<Tuple<string, int>> GetKingCommunityActiveAmountList(string domain, bool isProduction, string bank, long? amount)
        {
            int? comapanyId = CompanyService.CompanyId(domain);
            string postUrl = CompanyService.GetValue(comapanyId.Value, "KingCommunity.PostUrl", isProduction);
            //string mName = CompanyService.GetValue(comapanyId.Value, "BP.MerchantName" + (force3D ? "3d" : ""), isProduction);
            string clientId = CompanyService.GetValue(comapanyId.Value, "KingCommunity.ClientId", isProduction);
            string clientSecret = CompanyService.GetValue(comapanyId.Value, "KingCommunity.ClientSecret", isProduction);
            string username = CompanyService.GetValue(comapanyId.Value, "KingCommunity.Username", isProduction);
            string password = CompanyService.GetValue(comapanyId.Value, "KingCommunity.Password", isProduction);



            IList<Tuple<string, int>> result = new List<Tuple<string, int>>();

            var envoyToken = EnvoyToken(postUrl, clientId, clientSecret, username, password);


            JObject obj = new JObject();
            obj["bank"] = bank;
            JObject amountListObj = (JObject)JsonConvert.DeserializeObject(NW.Helper.HttpHelper.PostRequest((postUrl + "/apiv2/availableAmounts"), JsonConvert.SerializeObject(obj), "application/json", new NameValueCollection() { { "Authorization", ("Bearer " + envoyToken.Item2) } }));
            foreach (JObject activeAmount in (JArray)amountListObj["draws"])
            {
                result.Add(Tuple.Create<string, int>(activeAmount["ref_code"].Value<string>(), (int)activeAmount["amount"].Value<decimal>()));
            }

            return result;

        }

        public IList<DepositSummaryModel> GetLastDaysDepositList(int memberId)
        {
            List<DepositSummaryModel> allDepositList = new List<DepositSummaryModel>();
            int[] bpcMethodIdList = new int[] { 10, 5, 4, 2, 1 };
            allDepositList.AddRange(CepBankRequestRepository.GetAll().Where(cbr => cbr.CreateDate >= DateTime.UtcNow.AddDays(-2) && cbr.MemberId == memberId).Select(cbr => new DepositSummaryModel() { Amount = cbr.Amount.ToString(), StatusType = cbr.PaymentStatusType, CreateDate = cbr.CreateDate, ProviderName = "Cepbank" }));
            allDepositList.AddRange(BPCRequestRepository.GetAll().Where(bp => bp.CreateDate >= DateTime.UtcNow.AddDays(-2) && bp.MemberId == memberId && bpcMethodIdList.Contains(bp.BPCPaymentMethodTypeId)).Select(bp => new DepositSummaryModel() { Amount = bp.Amount.ToString(), StatusType = ChangeIntegrationStatusType(bp.StatusType), CreateDate = bp.CreateDate, ProviderName = "Cepbank" }));
            allDepositList.AddRange(BankTransferRequestRepository.GetAll().Where(bt => bt.CreateDate >= DateTime.UtcNow.AddDays(-2) && bt.MemberId == memberId).Select(bt => new DepositSummaryModel() { Amount = (bt.Amount * 0.01).ToString(), StatusType = bt.PaymentStatusType, CreateDate = bt.CreateDate, ProviderName = "BankTransfer" }));
            allDepositList.AddRange(PaparaTransferRequestRepository.GetAll().Where(pt => pt.PaymentStatusType != -99 && pt.CreateDate >= DateTime.UtcNow.AddDays(-2) && pt.MemberId == memberId).Select(pt => new DepositSummaryModel() { Amount = (pt.Amount * 0.01).ToString(), StatusType = pt.PaymentStatusType, CreateDate = pt.CreateDate, ProviderName = "Papara" }));
            allDepositList.AddRange(PayminoRequest2Repository.GetAll().Where(pr => pr.CreateDate >= DateTime.UtcNow.AddDays(-2) && pr.MemberId == memberId && pr.UpdateDate != null).Select(pr => new DepositSummaryModel() { Amount = (pr.Amount * 0.01).ToString(), StatusType = pr.StatusType, CreateDate = pr.CreateDate, ProviderName = "BankTransfer" }));


            return allDepositList;
        }




        #region papara

        public IList<PaparaTransferRequest> PaparaTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PaparaTransferRequestRepository.GetAll().Where(p => p.MemberId == memberId).ToList();
            }
        }
        public IList<PaparaTransferRequest> PendingPaparaTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PaparaTransferRequestRepository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Pending && p.MemberId == memberId).ToList(); // success
            }
        }
        public IList<PaparaTransferRequest> SuccessfulPaparaTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PaparaTransferRequestRepository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Approved && p.MemberId == memberId).ToList(); // success
            }
        }
        public IList<PaparaAccount> PaparaAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {


                return PaparaAccountRepository.GetAll().Where(pa => pa.StatusType == (int)StatusType.Active && pa.MemberId == memberId).OrderByDescending(pa => pa.CreateDate).ToList();
            }
        }
        public PaparaAccount InsertPaparaAccount(int memberId, string firstname, string lastname, string accountNumber, string currency)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                PaparaAccount paparaAccount;
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    paparaAccount = PaparaAccountRepository.Insert(new PaparaAccount() { MemberId = memberId, Firstname = firstname, Lastname = lastname, StatusType = (int)StatusType.Active, CreateDate = DateTime.UtcNow, AccountNumber = accountNumber, Currency = currency });
                    uniOfWork.Commit(transaction);

                }

                return paparaAccount;
            }
        }
        public PaparaAccount UpdatePaparaAccount(PaparaAccount paparaAccount)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    paparaAccount = PaparaAccountRepository.Update(paparaAccount);
                    uniOfWork.Commit(transaction);
                }
                return paparaAccount;
            }
        }
        public void DeletePaparaAccount(int paparaAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    PaparaAccount paparaAccount = PaparaAccountRepository.Get(paparaAccountId);
                    paparaAccount.StatusType = (int)StatusType.Passive;
                    paparaAccount = PaparaAccountRepository.Update(paparaAccount);
                    uniOfWork.Commit(transaction);
                }
            }

        }
        public PaparaAccount PaparaAccount(int memberId, int paparaAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                return PaparaAccountRepository.GetAll().FirstOrDefault(pa => pa.MemberId == memberId && pa.Id == paparaAccountId);
            }
        }
        public PaparaAccount PaparaAccount(int paparaAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                return PaparaAccountRepository.GetAll().FirstOrDefault(pa => pa.Id == paparaAccountId);
            }
        }

        public IList<PaparaTransferPaparaAccount> PaparaTransferPaparaAccountList()
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PaparaTransferPaparaAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public IList<PaparaTransferPaparaAccount> PaparaTransferPaparaAccountList(Member member)
        {
            return PaparaTransferPaparaAccountListForLevel(member.LevelId.Value);
        }
        public IList<PaparaTransferPaparaAccount> PaparaTransferPaparaAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                Member member = MemberService.GetMember(memberId);
                return PaparaTransferPaparaAccountList(member);
            }
        }
        public IList<PaparaTransferPaparaAccount> PaparaTransferPaparaAccountList(int memberId, int providerId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                Provider provider = ProviderService.GetProviderByVoltronProviderId(providerId);
                return PaparaTransferPaparaAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active && ptpa.ProviderId == provider.Id).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public IList<PaparaTransferPaparaAccount> PaparaTransferPaparaAccountListForLevel(Level level)
        {
            return PaparaTransferPaparaAccountListForLevel(level.Id);
        }
        public IList<PaparaTransferPaparaAccount> PaparaTransferPaparaAccountListForLevel(int levelId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PaparaTransferPaparaAccountRepository.GetAll().Where(ptpa => ptpa.Levels.Any(l => l.Id == levelId) && ptpa.StatusType == (int)StatusType.Active).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public void InsertPaparaTransferRequest(int memberId, int paparaTransferPaparaAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);

                PaparaTransferRequestRepository.Insert(new PaparaTransferRequest()
                {
                    Amount = amount,
                    UpdateAmount = amount,
                    RecognisedAmount = amount,
                    AccountNumber = accountNumber,
                    MemberId = memberId,
                    PaparaTransferPaparaAccountId = paparaTransferPaparaAccountId,
                    PaymentStatusType = (int)PaymentStatusType.Pending,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    SenderFullName = String.Format("{0} {1}", firstname, lastname),
                    WithBonus = withBonus,
                    BonusId = bonusId,
                    Note = note,
                    CompanyId = companyId,
                    ProviderId = 49 // TODO
                });

                transaction.Commit();

                NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made papara transfer and filled out the form, amount: " + amount * 0.01 + " TL");
            }
        }
        public void UpdatePaparaTransferRequest(int paparaTransferRequestId, int paparaTransferPaparaAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);

                PaparaTransferRequest paparaTransferRequest = PaparaTransferRequestRepository.Get(paparaTransferRequestId);


                PaparaTransferPaparaAccount paparaTransferPaparaAccount = PaparaTransferPaparaAccountRepository.Get(paparaTransferPaparaAccountId);
                paparaTransferRequest.ProviderId = paparaTransferPaparaAccount.Provider.VoltronProviderId;

                paparaTransferRequest.Amount = amount;
                paparaTransferRequest.UpdateAmount = amount;
                paparaTransferRequest.RecognisedAmount = amount;
                paparaTransferRequest.AccountNumber = accountNumber;
                paparaTransferRequest.PaparaTransferPaparaAccountId = paparaTransferPaparaAccountId;
                paparaTransferRequest.PaymentStatusType = (int)PaymentStatusType.Pending;
                paparaTransferRequest.UpdateDate = DateTime.UtcNow;
                paparaTransferRequest.SenderFullName = String.Format("{0} {1}", firstname, lastname);
                paparaTransferRequest.WithBonus = withBonus;
                paparaTransferRequest.BonusId = bonusId;
                paparaTransferRequest.Note = note;
                paparaTransferRequest.CompanyId = companyId;

                PaparaTransferRequestRepository.Update(paparaTransferRequest);
                transaction.Commit();

                NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made papara transfer and filled out the form, amount: " + amount * 0.01 + " TL");
            }
        }
        public int SelectPaparaProvider(int memberId, int companyId, bool isProduction, long? amount = null)
        {

            List<NewPaymentProvider> paparaProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 5).ToList();

            //commented out part enables provider management for papara providers
            List<int> disabledProviders = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).Select(m => m.ProviderId).ToList();

            List<int> tempProviderIds = new List<int>();
            foreach (NewPaymentProvider p in paparaProviders)
            {
                if (!disabledProviders.Any(dp => dp == p.VoltronProviderId))
                {
                    int weight = p.Weight.HasValue ? p.Weight.Value : 0;
                    for (int i = weight; i > 0; i--)
                    {
                        if (amount.HasValue && p.MinAmount <= amount.Value && p.MaxAmount > amount.Value)
                        {
                            tempProviderIds.Add(p.VoltronProviderId.Value);
                        }
                    }
                }
            }
            Random rnd = new Random();
            int r = rnd.Next(tempProviderIds.Count);
            return tempProviderIds[r];
        }

        public NewPaymentProvider GetPaparaPaymentProvider(int memberId, int companyId, long amount, bool isProduction)
        {
            PaparaTransferRequest request = new PaparaTransferRequest();
            // check if a provider is assigned to user when s/he visits papara form page
            // check for 15 mins to make sure if scheduled job failed
            string log = string.Empty;
            int paparaProviderId = 0;
            List<NewPaymentProvider> paparaProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 5).ToList();
            if (PaparaTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.Amount == amount && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15)))
            {

                request = PaparaTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.Amount == amount && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15));
                paparaProviderId = request.ProviderId.Value;
                log += ">Member has -99 request #" + request.Id + " for provider #" + request.ProviderId.Value;

                // change if the assigned provider is closed
                if (!paparaProviders.Any(p => p.VoltronProviderId == request.ProviderId))
                {
                    log += ">But provider is turned off";
                    using (var uniOfWork = UnitOfWork.Current)
                    {
                        ITransaction transaction = uniOfWork.BeginTransaction(Session);

                        // assign new provider id & reset time
                        paparaProviderId = SelectPaparaProvider(memberId, companyId, isProduction, amount);

                        request.ProviderId = paparaProviderId;
                        request.CreateDate = DateTime.UtcNow;
                        request.UpdateDate = DateTime.UtcNow;


                        request = PaparaTransferRequestRepository.Update(request);

                        log += ">New provider set #" + paparaProviderId + ". Now provider is(checking) #" + request.ProviderId;
                        transaction.Commit();
                    }
                }
            }
            // check if user has any rejected request within last 15 minutes
            else if (PaparaTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.Amount == amount && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)))
            {
                log += ">Member has rejected request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    paparaProviderId = PaparaTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)).ProviderId.Value;
                    log += ">for provider #" + paparaProviderId;

                    // change if the assigned provider is closed. if so assign another provider
                    if (!paparaProviders.Any(p => p.VoltronProviderId == request.ProviderId))
                    {
                        log += ">But provider is turned off";
                        paparaProviderId = SelectPaparaProvider(memberId, companyId, isProduction, amount);
                        log += ">New provider set #" + paparaProviderId;
                    }

                    request = PaparaTransferRequestRepository.Insert(new PaparaTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = paparaProviderId,
                        Amount = amount,
                        RecognisedAmount = amount,
                        UpdateAmount = amount
                    });
                    log += ">-99 request created #" + request.Id;

                    transaction.Commit();
                }
            }
            // assign random provider
            else
            {
                log += ">Member has no request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    paparaProviderId = SelectPaparaProvider(memberId, companyId, isProduction, amount);

                    log += ">New provider set #" + paparaProviderId;

                    request = PaparaTransferRequestRepository.Insert(new PaparaTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = paparaProviderId,
                        Amount = amount,
                        RecognisedAmount = amount,
                        UpdateAmount = amount
                    });

                    log += ">-99 request created #" + request.Id;
                    transaction.Commit();
                }
            }
            Logger.Fatal("CheckPaparaPageLoad ==>Member: " + memberId + " STORY: " + log);

            NewPaymentProvider paymentProvider = paparaProviders.FirstOrDefault(pp => pp.VoltronProviderId == paparaProviderId);

            return paymentProvider;
        }
        public PaparaTransferRequest CheckPaparaRequest(int memberId, int companyId, bool isProduction)
        {
            PaparaTransferRequest request = new PaparaTransferRequest();
            // check if a provider is assigned to user when s/he visits papara form page
            // check for 15 mins to make sure if scheduled job failed
            string log = string.Empty;
            if (PaparaTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15)))
            {

                request = PaparaTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15));
                log += ">Member has -99 request #" + request.Id + " for provider #" + request.ProviderId.Value;


                List<NewPaymentProvider> paparaProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 5).ToList();
                // change if the assigned provider is closed
                if (!paparaProviders.Any(p => p.VoltronProviderId == request.ProviderId))
                {
                    log += ">But provider is turned off";
                    using (var uniOfWork = UnitOfWork.Current)
                    {
                        ITransaction transaction = uniOfWork.BeginTransaction(Session);

                        // assign new provider id & reset time
                        int paparaProviderId = SelectPaparaProvider(memberId, companyId, isProduction);

                        request.ProviderId = paparaProviderId;
                        request.CreateDate = DateTime.UtcNow;
                        request.UpdateDate = DateTime.UtcNow;


                        request = PaparaTransferRequestRepository.Update(request);

                        log += ">New provider set #" + paparaProviderId + ". Now provider is(checking) #" + request.ProviderId;
                        transaction.Commit();
                    }
                }
            }
            // check if user has any rejected request within last 15 minutes
            else if (PaparaTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)))
            {
                log += ">Member has rejected request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int paparaProviderId = PaparaTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)).ProviderId.Value;
                    log += ">for provider #" + paparaProviderId;

                    List<NewPaymentProvider> paparaProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 5).ToList();

                    // change if the assigned provider is closed. if so assign another provider
                    if (!paparaProviders.Any(p => p.VoltronProviderId == request.ProviderId))
                    {
                        log += ">But provider is turned off";
                        paparaProviderId = SelectPaparaProvider(memberId, companyId, isProduction);
                        log += ">New provider set #" + paparaProviderId;
                    }

                    request = PaparaTransferRequestRepository.Insert(new PaparaTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = paparaProviderId,
                    });
                    log += ">-99 request created #" + request.Id;

                    transaction.Commit();
                }
            }
            // assign random provider
            else
            {
                log += ">Member has no request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int paparaProviderId = SelectPaparaProvider(memberId, companyId, isProduction);

                    log += ">New provider set #" + paparaProviderId;

                    request = PaparaTransferRequestRepository.Insert(new PaparaTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = paparaProviderId,
                    });

                    log += ">-99 request created #" + request.Id;
                    transaction.Commit();
                }
            }
            Logger.Fatal("CheckPaparaPageLoad ==>Member: " + memberId + " STORY: " + log);
            return request;
        }
        public PaparaTransferRequest CheckPaparaRequestAfterSubmit(int selectedPaparaTransferPaparaAccountId, int memberId, int companyId, bool isProduction)
        {
            PaparaTransferRequest request = new PaparaTransferRequest();
            // check if a provider is assigned to user when s/he visits papara form page
            // check for 15 mins to make sure if scheduled job failed
            string log = string.Empty;

            PaparaTransferPaparaAccount paparaTransferPaparaAccount = PaparaTransferPaparaAccountRepository.Get(selectedPaparaTransferPaparaAccountId);

            if (PaparaTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15)))
            {

                request = PaparaTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15));
                log += ">Member has -99 request #" + request.Id + " for provider #" + request.ProviderId.Value;


                List<NewPaymentProvider> paparaProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 5).ToList();
                // change if the assigned provider is closed
                if (!paparaProviders.Any(p => p.VoltronProviderId == request.ProviderId))
                {
                    log += ">But provider is turned off";
                    using (var uniOfWork = UnitOfWork.Current)
                    {
                        ITransaction transaction = uniOfWork.BeginTransaction(Session);

                        request.PaparaTransferPaparaAccountId = selectedPaparaTransferPaparaAccountId;
                        request.ProviderId = paparaTransferPaparaAccount.ProviderId;
                        request.CreateDate = DateTime.UtcNow;
                        request.UpdateDate = DateTime.UtcNow;


                        request = PaparaTransferRequestRepository.Update(request);

                        log += ">New provider set #" + paparaTransferPaparaAccount.ProviderId + ". Now provider is(checking) #" + request.ProviderId;
                        transaction.Commit();
                    }
                }
            }
            // assign random provider
            else
            {
                log += ">Member has no request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    log += ">New provider set #" + paparaTransferPaparaAccount.ProviderId;

                    request = PaparaTransferRequestRepository.Insert(new PaparaTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = 0,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        PaparaTransferPaparaAccountId = selectedPaparaTransferPaparaAccountId,
                        ProviderId = paparaTransferPaparaAccount.ProviderId,
                    });

                    log += ">-99 request created #" + request.Id;
                    transaction.Commit();
                }
            }
            Logger.Fatal("CheckPaparaAfterSubmit ==> Member: " + memberId + " STORY: " + log);
            return request;
        }
        #endregion
        #region cmt

        public IList<CMTTransferRequest> CMTTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return CMTTransferRequestRepository.GetAll().Where(p => p.MemberId == memberId).ToList();
            }
        }
        public IList<CMTTransferRequest> PendingCMTTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return CMTTransferRequestRepository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Pending && p.MemberId == memberId).ToList(); // success
            }
        }
        public IList<CMTTransferRequest> SuccessfulCMTTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return CMTTransferRequestRepository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Approved && p.MemberId == memberId).ToList(); // success
            }
        }
        public IList<CMTAccount> CMTAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {


                return CMTAccountRepository.GetAll().Where(pa => pa.StatusType == (int)StatusType.Active && pa.MemberId == memberId).OrderByDescending(pa => pa.CreateDate).ToList();
            }
        }
        public CMTAccount InsertCMTAccount(int memberId, string firstname, string lastname, string accountNumber, string currency)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                CMTAccount cmtAccount;
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    cmtAccount = CMTAccountRepository.Insert(new CMTAccount() { MemberId = memberId, Firstname = firstname, Lastname = lastname, StatusType = (int)StatusType.Active, CreateDate = DateTime.UtcNow, AccountNumber = accountNumber, Currency = currency });
                    uniOfWork.Commit(transaction);

                }

                return cmtAccount;
            }
        }
        public CMTAccount UpdateCMTAccount(CMTAccount cmtAccount)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    cmtAccount = CMTAccountRepository.Update(cmtAccount);
                    uniOfWork.Commit(transaction);
                }
                return cmtAccount;
            }
        }
        public void DeleteCMTAccount(int cmtAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    CMTAccount cmtAccount = CMTAccountRepository.Get(cmtAccountId);
                    cmtAccount.StatusType = (int)StatusType.Passive;
                    cmtAccount = CMTAccountRepository.Update(cmtAccount);
                    uniOfWork.Commit(transaction);
                }
            }

        }
        public CMTAccount CMTAccount(int memberId, int CMTAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                return CMTAccountRepository.GetAll().FirstOrDefault(pa => pa.MemberId == memberId && pa.Id == CMTAccountId);
            }
        }
        public CMTAccount CMTAccount(int CMTAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                return CMTAccountRepository.GetAll().FirstOrDefault(pa => pa.Id == CMTAccountId);
            }
        }

        public IList<CMTTransferCMTAccount> CMTTransferCMTAccountList()
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return CMTTransferCMTAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public IList<CMTTransferCMTAccount> CMTTransferCMTAccountList(Member member)
        {
            return CMTTransferCMTAccountListForLevel(member.LevelId.Value);
        }
        public IList<CMTTransferCMTAccount> CMTTransferCMTAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                Member member = MemberService.GetMember(memberId);
                return CMTTransferCMTAccountList(member);
            }
        }
        public IList<CMTTransferCMTAccount> CMTTransferCMTAccountList(int memberId, int providerId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                //Provider provider = ProviderService.GetProviderByVoltronProviderId(providerId); 
                return CMTTransferCMTAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active && ptpa.ProviderId == providerId).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public IList<CMTTransferCMTAccount> CMTTransferCMTAccountListForLevel(Level level)
        {
            return CMTTransferCMTAccountListForLevel(level.Id);
        }
        public IList<CMTTransferCMTAccount> CMTTransferCMTAccountListForLevel(int levelId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return CMTTransferCMTAccountRepository.GetAll().Where(ptpa => ptpa.Levels.Any(l => l.Id == levelId) && ptpa.StatusType == (int)StatusType.Active).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public void InsertCMTTransferRequest(int memberId, int CMTTransferCMTAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);

                CMTTransferRequestRepository.Insert(new CMTTransferRequest()
                {
                    Amount = amount,
                    UpdateAmount = amount,
                    AccountNumber = accountNumber,
                    MemberId = memberId,
                    CMTTransferCMTAccountId = CMTTransferCMTAccountId,
                    PaymentStatusType = (int)PaymentStatusType.Pending,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    SenderFullName = String.Format("{0} {1}", firstname, lastname),
                    WithBonus = withBonus,
                    BonusId = bonusId,
                    Note = note,
                    CompanyId = companyId,
                    ProviderId = 74 // TODO
                });

                transaction.Commit();

                NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made CMT transfer and filled out the form, amount: " + amount * 0.01 + " TL");
            }
        }
        public void UpdateCMTTransferRequest(int cmtTransferRequestId, int CMTTransferCMTAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);

                CMTTransferRequest cmtTransferRequest = CMTTransferRequestRepository.Get(cmtTransferRequestId);
                cmtTransferRequest.Amount = amount;
                cmtTransferRequest.UpdateAmount = amount;
                cmtTransferRequest.AccountNumber = accountNumber;
                cmtTransferRequest.CMTTransferCMTAccountId = CMTTransferCMTAccountId;
                cmtTransferRequest.PaymentStatusType = (int)PaymentStatusType.Pending;
                cmtTransferRequest.CreateDate = DateTime.UtcNow;
                cmtTransferRequest.UpdateDate = DateTime.UtcNow;
                cmtTransferRequest.SenderFullName = String.Format("{0} {1}", firstname, lastname);
                cmtTransferRequest.WithBonus = withBonus;
                cmtTransferRequest.BonusId = bonusId;
                cmtTransferRequest.Note = note;
                cmtTransferRequest.CompanyId = companyId;

                CMTTransferRequestRepository.Update(cmtTransferRequest);
                transaction.Commit();

                NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made CMT transfer and filled out the form, amount: " + amount * 0.01 + " TL");
            }
        }
        public int SelectCMTProvider(int memberId, int companyId, bool isProduction)
        {
            List<NewPaymentProvider> paparaProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 24).ToList();

            //commented out part enables provider management for papara providers
            //List<int> disabledProviders = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).Select(m => m.ProviderId).ToList();

            List<int> tempProviderIds = new List<int>();
            foreach (NewPaymentProvider p in paparaProviders)
            {
                //if (!disabledProviders.Any(dp => dp == p.VoltronProviderId))
                //{
                int weight = p.Weight.HasValue ? p.Weight.Value : 0;
                for (int i = weight; i > 0; i--)
                {
                    tempProviderIds.Add(p.VoltronProviderId.Value);
                }
                //}
            }
            Random rnd = new Random();
            int r = rnd.Next(tempProviderIds.Count);
            return tempProviderIds[r];
        }
        public CMTTransferRequest CheckCMTRequest(int memberId, int companyId, bool isProduction)
        {
            CMTTransferRequest request = new CMTTransferRequest();
            if (CMTTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15)))
            {
                request = CMTTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15));
            }
            else if (CMTTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)))
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int CMTProviderId = CMTTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)).ProviderId.Value;


                    request = CMTTransferRequestRepository.Insert(new CMTTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = CMTProviderId,
                    });

                    transaction.Commit();
                }
            }
            else
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int CMTProviderId = SelectCMTProvider(memberId, companyId, isProduction);


                    request = CMTTransferRequestRepository.Insert(new CMTTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = CMTProviderId,
                    });

                    transaction.Commit();
                }
            }
            return request;
        }
        #endregion
        #region Mefete

        public IList<MefeteTransferRequest> MefeteTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return MefeteTransferRequestRepository.GetAll().Where(p => p.MemberId == memberId).ToList();
            }
        }
        public IList<MefeteTransferRequest> PendingMefeteTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return MefeteTransferRequestRepository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Pending && p.MemberId == memberId).ToList(); // success
            }
        }
        public IList<MefeteTransferRequest> SuccessfulMefeteTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return MefeteTransferRequestRepository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Approved && p.MemberId == memberId).ToList(); // success
            }
        }
        public IList<MefeteAccount> MefeteAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {


                return MefeteAccountRepository.GetAll().Where(pa => pa.StatusType == (int)StatusType.Active && pa.MemberId == memberId).OrderByDescending(pa => pa.CreateDate).ToList();
            }
        }
        public MefeteAccount InsertMefeteAccount(int memberId, string firstname, string lastname, string accountNumber, string currency)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                MefeteAccount MefeteAccount;
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    MefeteAccount = MefeteAccountRepository.Insert(new MefeteAccount() { MemberId = memberId, Firstname = firstname, Lastname = lastname, StatusType = (int)StatusType.Active, CreateDate = DateTime.UtcNow, AccountNumber = accountNumber, Currency = currency });
                    uniOfWork.Commit(transaction);

                }

                return MefeteAccount;
            }
        }
        public MefeteAccount UpdateMefeteAccount(MefeteAccount MefeteAccount)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    MefeteAccount = MefeteAccountRepository.Update(MefeteAccount);
                    uniOfWork.Commit(transaction);
                }
                return MefeteAccount;
            }
        }
        public void DeleteMefeteAccount(int MefeteAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    MefeteAccount MefeteAccount = MefeteAccountRepository.Get(MefeteAccountId);
                    MefeteAccount.StatusType = (int)StatusType.Passive;
                    MefeteAccount = MefeteAccountRepository.Update(MefeteAccount);
                    uniOfWork.Commit(transaction);
                }
            }

        }
        public MefeteAccount MefeteAccount(int memberId, int MefeteAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                return MefeteAccountRepository.GetAll().FirstOrDefault(pa => pa.MemberId == memberId && pa.Id == MefeteAccountId);
            }
        }
        public MefeteAccount MefeteAccount(int MefeteAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                return MefeteAccountRepository.GetAll().FirstOrDefault(pa => pa.Id == MefeteAccountId);
            }
        }

        public IList<MefeteTransferMefeteAccount> MefeteTransferMefeteAccountList()
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return MefeteTransferMefeteAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public IList<MefeteTransferMefeteAccount> MefeteTransferMefeteAccountList(Member member)
        {
            return MefeteTransferMefeteAccountListForLevel(member.LevelId.Value);
        }
        public IList<MefeteTransferMefeteAccount> MefeteTransferMefeteAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                Member member = MemberService.GetMember(memberId);
                return MefeteTransferMefeteAccountList(member);
            }
        }
        public IList<MefeteTransferMefeteAccount> MefeteTransferMefeteAccountList(int memberId, int providerId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                //Provider provider = ProviderService.GetProviderByVoltronProviderId(providerId); 
                return MefeteTransferMefeteAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active && ptpa.ProviderId == providerId).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public IList<MefeteTransferMefeteAccount> MefeteTransferMefeteAccountListForLevel(Level level)
        {
            return MefeteTransferMefeteAccountListForLevel(level.Id);
        }
        public IList<MefeteTransferMefeteAccount> MefeteTransferMefeteAccountListForLevel(int levelId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return MefeteTransferMefeteAccountRepository.GetAll().Where(ptpa => ptpa.Levels.Any(l => l.Id == levelId) && ptpa.StatusType == (int)StatusType.Active).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public void InsertMefeteTransferRequest(int memberId, int MefeteTransferMefeteAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);

                MefeteTransferRequestRepository.Insert(new MefeteTransferRequest()
                {
                    Amount = amount,
                    UpdateAmount = amount,
                    AccountNumber = accountNumber,
                    MemberId = memberId,
                    MefeteTransferMefeteAccountId = MefeteTransferMefeteAccountId,
                    PaymentStatusType = (int)PaymentStatusType.Pending,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    SenderFullName = String.Format("{0} {1}", firstname, lastname),
                    WithBonus = withBonus,
                    BonusId = bonusId,
                    Note = note,
                    CompanyId = companyId,
                    ProviderId = 104 // TODO
                });

                transaction.Commit();

                NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made Mefete transfer and filled out the form, amount: " + amount * 0.01 + " TL");
            }
        }
        public void UpdateMefeteTransferRequest(int MefeteTransferRequestId, int MefeteTransferMefeteAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);

                MefeteTransferRequest MefeteTransferRequest = MefeteTransferRequestRepository.Get(MefeteTransferRequestId);
                MefeteTransferRequest.Amount = amount;
                MefeteTransferRequest.UpdateAmount = amount;
                MefeteTransferRequest.AccountNumber = accountNumber;
                MefeteTransferRequest.MefeteTransferMefeteAccountId = MefeteTransferMefeteAccountId;
                MefeteTransferRequest.PaymentStatusType = (int)PaymentStatusType.Pending;
                MefeteTransferRequest.CreateDate = DateTime.UtcNow;
                MefeteTransferRequest.UpdateDate = DateTime.UtcNow;
                MefeteTransferRequest.SenderFullName = String.Format("{0} {1}", firstname, lastname);
                MefeteTransferRequest.WithBonus = withBonus;
                MefeteTransferRequest.BonusId = bonusId;
                MefeteTransferRequest.Note = note;
                MefeteTransferRequest.CompanyId = companyId;

                MefeteTransferRequestRepository.Update(MefeteTransferRequest);
                transaction.Commit();

                NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made Mefete transfer and filled out the form, amount: " + amount * 0.01 + " TL");
            }
        }
        public int SelectMefeteProvider(int memberId, int companyId, bool isProduction)
        {
            List<NewPaymentProvider> paparaProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 45).ToList();

            //commented out part enables provider management for papara providers
            //List<int> disabledProviders = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).Select(m => m.ProviderId).ToList();

            List<int> tempProviderIds = new List<int>();
            foreach (NewPaymentProvider p in paparaProviders)
            {
                //if (!disabledProviders.Any(dp => dp == p.VoltronProviderId))
                //{
                int weight = p.Weight.HasValue ? p.Weight.Value : 0;
                for (int i = weight; i > 0; i--)
                {
                    tempProviderIds.Add(p.VoltronProviderId.Value);
                }
                //}
            }
            Random rnd = new Random();
            int r = rnd.Next(tempProviderIds.Count);
            return tempProviderIds[r];
        }
        public MefeteTransferRequest CheckMefeteRequest(int memberId, int companyId, bool isProduction)
        {
            MefeteTransferRequest request = new MefeteTransferRequest();
            if (MefeteTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15)))
            {
                request = MefeteTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15));
            }
            else if (MefeteTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)))
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int MefeteProviderId = MefeteTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)).ProviderId.Value;


                    request = MefeteTransferRequestRepository.Insert(new MefeteTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = MefeteProviderId,
                    });

                    transaction.Commit();
                }
            }
            else
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int MefeteProviderId = SelectMefeteProvider(memberId, companyId, isProduction);


                    request = MefeteTransferRequestRepository.Insert(new MefeteTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = MefeteProviderId,
                    });

                    transaction.Commit();
                }
            }
            return request;
        }
        #endregion
        #region Parazula

        public IList<ParazulaTransferRequest> ParazulaTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return ParazulaTransferRequestRepository.GetAll().Where(p => p.MemberId == memberId).ToList();
            }
        }
        public IList<ParazulaTransferRequest> PendingParazulaTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return ParazulaTransferRequestRepository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Pending && p.MemberId == memberId).ToList(); // success
            }
        }
        public IList<ParazulaTransferRequest> SuccessfulParazulaTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return ParazulaTransferRequestRepository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Approved && p.MemberId == memberId).ToList(); // success
            }
        }
        public IList<ParazulaAccount> ParazulaAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {


                return ParazulaAccountRepository.GetAll().Where(pa => pa.StatusType == (int)StatusType.Active && pa.MemberId == memberId).OrderByDescending(pa => pa.CreateDate).ToList();
            }
        }
        public ParazulaAccount InsertParazulaAccount(int memberId, string firstname, string lastname, string accountNumber, string currency)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ParazulaAccount ParazulaAccount;
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    ParazulaAccount = ParazulaAccountRepository.Insert(new ParazulaAccount() { MemberId = memberId, Firstname = firstname, Lastname = lastname, StatusType = (int)StatusType.Active, CreateDate = DateTime.UtcNow, AccountNumber = accountNumber, Currency = currency });
                    uniOfWork.Commit(transaction);

                }

                return ParazulaAccount;
            }
        }
        public ParazulaAccount UpdateParazulaAccount(ParazulaAccount parazulaAccount)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    parazulaAccount = ParazulaAccountRepository.Update(parazulaAccount);
                    uniOfWork.Commit(transaction);
                }
                return parazulaAccount;
            }
        }
        public void DeleteParazulaAccount(int parazulaAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    ParazulaAccount parazulaAccount = ParazulaAccountRepository.Get(parazulaAccountId);
                    parazulaAccount.StatusType = (int)StatusType.Passive;
                    parazulaAccount = ParazulaAccountRepository.Update(parazulaAccount);
                    uniOfWork.Commit(transaction);
                }
            }

        }
        public ParazulaAccount ParazulaAccount(int memberId, int parazulaAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                return ParazulaAccountRepository.GetAll().FirstOrDefault(pa => pa.MemberId == memberId && pa.Id == parazulaAccountId);
            }
        }
        public ParazulaAccount ParazulaAccount(int parazulaAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                return ParazulaAccountRepository.GetAll().FirstOrDefault(pa => pa.Id == parazulaAccountId);
            }
        }

        public IList<ParazulaTransferParazulaAccount> ParazulaTransferParazulaAccountList()
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return ParazulaTransferParazulaAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public IList<ParazulaTransferParazulaAccount> ParazulaTransferParazulaAccountList(Member member)
        {
            return ParazulaTransferParazulaAccountListForLevel(member.LevelId.Value);
        }
        public IList<ParazulaTransferParazulaAccount> ParazulaTransferParazulaAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                Member member = MemberService.GetMember(memberId);
                return ParazulaTransferParazulaAccountList(member);
            }
        }
        public IList<ParazulaTransferParazulaAccount> ParazulaTransferParazulaAccountList(int memberId, int providerId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                //Provider provider = ProviderService.GetProviderByVoltronProviderId(providerId); 
                return ParazulaTransferParazulaAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active && ptpa.ProviderId == providerId).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public IList<ParazulaTransferParazulaAccount> ParazulaTransferParazulaAccountListForLevel(Level level)
        {
            return ParazulaTransferParazulaAccountListForLevel(level.Id);
        }
        public IList<ParazulaTransferParazulaAccount> ParazulaTransferParazulaAccountListForLevel(int levelId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return ParazulaTransferParazulaAccountRepository.GetAll().Where(ptpa => ptpa.Levels.Any(l => l.Id == levelId) && ptpa.StatusType == (int)StatusType.Active).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public void InsertParazulaTransferRequest(int memberId, int parazulaTransferParazulaAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);

                ParazulaTransferRequestRepository.Insert(new ParazulaTransferRequest()
                {
                    Amount = amount,
                    UpdateAmount = amount,
                    AccountNumber = accountNumber,
                    MemberId = memberId,
                    ParazulaTransferParazulaAccountId = parazulaTransferParazulaAccountId,
                    PaymentStatusType = (int)PaymentStatusType.Pending,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    SenderFullName = String.Format("{0} {1}", firstname, lastname),
                    WithBonus = withBonus,
                    BonusId = bonusId,
                    Note = note,
                    CompanyId = companyId,
                    ProviderId = 74 // TODO
                });

                transaction.Commit();

                NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made Parazula transfer and filled out the form, amount: " + amount * 0.01 + " TL");
            }
        }
        public void UpdateParazulaTransferRequest(int parazulaTransferRequestId, int parazulaTransferParazulaAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);

                ParazulaTransferRequest parazulaTransferRequest = ParazulaTransferRequestRepository.Get(parazulaTransferRequestId);
                parazulaTransferRequest.Amount = amount;
                parazulaTransferRequest.UpdateAmount = amount;
                parazulaTransferRequest.AccountNumber = accountNumber;
                parazulaTransferRequest.ParazulaTransferParazulaAccountId = parazulaTransferParazulaAccountId;
                parazulaTransferRequest.PaymentStatusType = (int)PaymentStatusType.Pending;
                parazulaTransferRequest.CreateDate = DateTime.UtcNow;
                parazulaTransferRequest.UpdateDate = DateTime.UtcNow;
                parazulaTransferRequest.SenderFullName = String.Format("{0} {1}", firstname, lastname);
                parazulaTransferRequest.WithBonus = withBonus;
                parazulaTransferRequest.BonusId = bonusId;
                parazulaTransferRequest.Note = note;
                parazulaTransferRequest.CompanyId = companyId;

                ParazulaTransferRequestRepository.Update(parazulaTransferRequest);
                transaction.Commit();

                NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made Parazula transfer and filled out the form, amount: " + amount * 0.01 + " TL");
            }
        }
        public int SelectParazulaProvider(int memberId, int companyId, bool isProduction)
        {
            List<NewPaymentProvider> paparaProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 42).ToList();

            //commented out part enables provider management for papara providers
            //List<int> disabledProviders = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).Select(m => m.ProviderId).ToList();

            List<int> tempProviderIds = new List<int>();
            foreach (NewPaymentProvider p in paparaProviders)
            {
                //if (!disabledProviders.Any(dp => dp == p.VoltronProviderId))
                //{
                int weight = p.Weight.HasValue ? p.Weight.Value : 0;
                for (int i = weight; i > 0; i--)
                {
                    tempProviderIds.Add(p.VoltronProviderId.Value);
                }
                //}
            }
            Random rnd = new Random();
            int r = rnd.Next(tempProviderIds.Count);
            return tempProviderIds[r];
        }
        public ParazulaTransferRequest CheckParazulaRequest(int memberId, int companyId, bool isProduction)
        {
            ParazulaTransferRequest request = new ParazulaTransferRequest();
            if (ParazulaTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15)))
            {
                request = ParazulaTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15));
            }
            else if (ParazulaTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)))
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int parazulaProviderId = ParazulaTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)).ProviderId.Value;


                    request = ParazulaTransferRequestRepository.Insert(new ParazulaTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = parazulaProviderId,
                    });

                    transaction.Commit();
                }
            }
            else
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int parazulaProviderId = SelectParazulaProvider(memberId, companyId, isProduction);


                    request = ParazulaTransferRequestRepository.Insert(new ParazulaTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = parazulaProviderId,
                    });

                    transaction.Commit();
                }
            }
            return request;
        }
        #endregion
        #region PEP

        public IList<PEPTransferRequest> PEPTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PEPTransferRequestRepository.GetAll().Where(p => p.MemberId == memberId).ToList();
            }
        }
        public IList<PEPTransferRequest> PendingPEPTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PEPTransferRequestRepository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Pending && p.MemberId == memberId).ToList(); // success
            }
        }
        public IList<PEPTransferRequest> SuccessfulPEPTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PEPTransferRequestRepository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Approved && p.MemberId == memberId).ToList(); // success
            }
        }
        public IList<PEPAccount> PEPAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {


                return PEPAccountRepository.GetAll().Where(pa => pa.StatusType == (int)StatusType.Active && pa.MemberId == memberId).OrderByDescending(pa => pa.CreateDate).ToList();
            }
        }
        public PEPAccount InsertPEPAccount(int memberId, string firstname, string lastname, string accountNumber, string currency)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                PEPAccount pepAccount;
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    pepAccount = PEPAccountRepository.Insert(new PEPAccount() { MemberId = memberId, Firstname = firstname, Lastname = lastname, StatusType = (int)StatusType.Active, CreateDate = DateTime.UtcNow, AccountNumber = accountNumber, Currency = currency });
                    uniOfWork.Commit(transaction);

                }

                return pepAccount;
            }
        }
        public PEPAccount UpdatePEPAccount(PEPAccount pepAccount)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    pepAccount = PEPAccountRepository.Update(pepAccount);
                    uniOfWork.Commit(transaction);
                }
                return pepAccount;
            }
        }
        public void DeletePEPAccount(int pepAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    PEPAccount pepAccount = PEPAccountRepository.Get(pepAccountId);
                    pepAccount.StatusType = (int)StatusType.Passive;
                    pepAccount = PEPAccountRepository.Update(pepAccount);
                    uniOfWork.Commit(transaction);
                }
            }

        }
        public PEPAccount PEPAccount(int memberId, int pepAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                return PEPAccountRepository.GetAll().FirstOrDefault(pa => pa.MemberId == memberId && pa.Id == pepAccountId);
            }
        }
        public PEPAccount PEPAccount(int pepAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                return PEPAccountRepository.GetAll().FirstOrDefault(pa => pa.Id == pepAccountId);
            }
        }

        public IList<PEPTransferPEPAccount> PEPTransferPEPAccountList()
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PEPTransferPEPAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public IList<PEPTransferPEPAccount> PEPTransferPEPAccountList(Member member)
        {
            return PEPTransferPEPAccountListForLevel(member.LevelId.Value);
        }
        public IList<PEPTransferPEPAccount> PEPTransferPEPAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                Member member = MemberService.GetMember(memberId);
                return PEPTransferPEPAccountList(member);
            }
        }
        public IList<PEPTransferPEPAccount> PEPTransferPEPAccountList(int memberId, int providerId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                //Provider provider = ProviderService.GetProviderByVoltronProviderId(providerId); 
                return PEPTransferPEPAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active && ptpa.ProviderId == providerId).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public IList<PEPTransferPEPAccount> PEPTransferPEPAccountListForLevel(Level level)
        {
            return PEPTransferPEPAccountListForLevel(level.Id);
        }
        public IList<PEPTransferPEPAccount> PEPTransferPEPAccountListForLevel(int levelId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PEPTransferPEPAccountRepository.GetAll().Where(ptpa => ptpa.Levels.Any(l => l.Id == levelId) && ptpa.StatusType == (int)StatusType.Active).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public void InsertPEPTransferRequest(int memberId, int pepTransferPEPAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);

                PEPTransferRequestRepository.Insert(new PEPTransferRequest()
                {
                    Amount = amount,
                    UpdateAmount = amount,
                    AccountNumber = accountNumber,
                    MemberId = memberId,
                    PEPTransferPEPAccountId = pepTransferPEPAccountId,
                    PaymentStatusType = (int)PaymentStatusType.Pending,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    SenderFullName = String.Format("{0} {1}", firstname, lastname),
                    WithBonus = withBonus,
                    BonusId = bonusId,
                    Note = note,
                    CompanyId = companyId,
                    ProviderId = 90 // TODO
                });

                transaction.Commit();

                NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made PEP transfer and filled out the form, amount: " + amount * 0.01 + " TL");
            }
        }
        public void UpdatePEPTransferRequest(int pepTransferRequestId, int pepTransferPEPAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);

                PEPTransferRequest pepTransferRequest = PEPTransferRequestRepository.Get(pepTransferRequestId);
                pepTransferRequest.Amount = amount;
                pepTransferRequest.UpdateAmount = amount;
                pepTransferRequest.AccountNumber = accountNumber;
                pepTransferRequest.PEPTransferPEPAccountId = pepTransferPEPAccountId;
                pepTransferRequest.PaymentStatusType = (int)PaymentStatusType.Pending;
                pepTransferRequest.CreateDate = DateTime.UtcNow;
                pepTransferRequest.UpdateDate = DateTime.UtcNow;
                pepTransferRequest.SenderFullName = String.Format("{0} {1}", firstname, lastname);
                pepTransferRequest.WithBonus = withBonus;
                pepTransferRequest.BonusId = bonusId;
                pepTransferRequest.Note = note;
                pepTransferRequest.CompanyId = companyId;

                PEPTransferRequestRepository.Update(pepTransferRequest);
                transaction.Commit();

                NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made PEP transfer and filled out the form, amount: " + amount * 0.01 + " TL");
            }
        }
        public int SelectPEPProvider(int memberId, int companyId, bool isProduction)
        {
            List<NewPaymentProvider> paparaProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 33).ToList();

            //commented out part enables provider management for papara providers
            //List<int> disabledProviders = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).Select(m => m.ProviderId).ToList();

            List<int> tempProviderIds = new List<int>();
            foreach (NewPaymentProvider p in paparaProviders)
            {
                //if (!disabledProviders.Any(dp => dp == p.VoltronProviderId))
                //{
                int weight = p.Weight.HasValue ? p.Weight.Value : 0;
                for (int i = weight; i > 0; i--)
                {
                    tempProviderIds.Add(p.VoltronProviderId.Value);
                }
                //}
            }
            Random rnd = new Random();
            int r = rnd.Next(tempProviderIds.Count);
            return tempProviderIds[r];
        }
        public PEPTransferRequest CheckPEPRequest(int memberId, int companyId, bool isProduction)
        {
            PEPTransferRequest request = new PEPTransferRequest();
            // check if a provider is assigned to user when s/he visits PEP form page
            // check for 15 mins to make sure if scheduled job failed
            string log = string.Empty;
            if (PEPTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15)))
            {

                request = PEPTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15));
                log += ">Member has -99 request #" + request.Id + " for provider #" + request.ProviderId.Value;


                List<NewPaymentProvider> PEPProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 33).ToList();
                // change if the assigned provider is closed
                if (!PEPProviders.Any(p => p.VoltronProviderId == request.ProviderId))
                {
                    log += ">But provider is turned off";
                    using (var uniOfWork = UnitOfWork.Current)
                    {
                        ITransaction transaction = uniOfWork.BeginTransaction(Session);

                        // assign new provider id & reset time
                        int pepProviderId = SelectPEPProvider(memberId, companyId, isProduction);

                        request.ProviderId = pepProviderId;
                        request.CreateDate = DateTime.UtcNow;
                        request.UpdateDate = DateTime.UtcNow;


                        request = PEPTransferRequestRepository.Update(request);

                        log += ">New provider set #" + pepProviderId + ". Now provider is(checking) #" + request.ProviderId;
                        transaction.Commit();
                    }
                }
            }
            // check if user has any rejected request within last 15 minutes
            else if (PEPTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)))
            {
                log += ">Member has rejected request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int pepProviderId = PEPTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)).ProviderId.Value;
                    log += ">for provider #" + pepProviderId;

                    List<NewPaymentProvider> PEPProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 33).ToList();

                    // change if the assigned provider is closed. if so assign another provider
                    if (!PEPProviders.Any(p => p.VoltronProviderId == request.ProviderId))
                    {
                        log += ">But provider is turned off";
                        pepProviderId = SelectPEPProvider(memberId, companyId, isProduction);
                        log += ">New provider set #" + pepProviderId;
                    }

                    request = PEPTransferRequestRepository.Insert(new PEPTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = pepProviderId,
                    });
                    log += ">-99 request created #" + request.Id;

                    transaction.Commit();
                }
            }
            // assign random provider
            else
            {
                log += ">Member has no request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int pepProviderId = SelectPEPProvider(memberId, companyId, isProduction);

                    log += ">New provider set #" + pepProviderId;

                    request = PEPTransferRequestRepository.Insert(new PEPTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = pepProviderId,
                    });

                    log += ">-99 request created #" + request.Id;
                    transaction.Commit();
                }
            }
            Logger.Fatal("CheckPEPPageLoad ==>Member: " + memberId + " STORY: " + log);
            return request;
        }
        public PEPTransferRequest CheckPEPRequestAfterSubmit(int memberId, int companyId, bool isProduction)
        {
            PEPTransferRequest request = new PEPTransferRequest();
            // check if a provider is assigned to user when s/he visits PEP form page
            // check for 15 mins to make sure if scheduled job failed
            string log = string.Empty;
            if (PEPTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15)))
            {

                request = PEPTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15));
                log += ">Member has -99 request #" + request.Id + " for provider #" + request.ProviderId.Value;


                List<NewPaymentProvider> PEPProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 33).ToList();
                // change if the assigned provider is closed
                if (!PEPProviders.Any(p => p.VoltronProviderId == request.ProviderId))
                {
                    log += ">But provider is turned off";
                    using (var uniOfWork = UnitOfWork.Current)
                    {
                        ITransaction transaction = uniOfWork.BeginTransaction(Session);

                        // assign new provider id & reset time
                        int PEPProviderId = SelectPEPProvider(memberId, companyId, isProduction);

                        request.ProviderId = PEPProviderId;
                        request.CreateDate = DateTime.UtcNow;
                        request.UpdateDate = DateTime.UtcNow;


                        request = PEPTransferRequestRepository.Update(request);

                        log += ">New provider set #" + PEPProviderId + ". Now provider is(checking) #" + request.ProviderId;
                        transaction.Commit();
                    }
                }
            }
            // check if user has any rejected request within last 15 minutes
            else if (PEPTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)))
            {
                log += ">Member has rejected request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int PEPProviderId = PEPTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)).ProviderId.Value;
                    log += ">for provider #" + PEPProviderId;

                    IList<NewPaymentProvider> PEPProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 33).ToList();//PEP providers

                    // change if the assigned provider is closed. if so assign another provider
                    if (!PEPProviders.Any(p => p.VoltronProviderId == PEPProviderId))
                    {
                        log += ">But provider is turned off";
                        PEPProviderId = SelectPEPProvider(memberId, companyId, isProduction);
                        log += ">New provider set #" + PEPProviderId;
                    }

                    request = PEPTransferRequestRepository.Insert(new PEPTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = PEPProviderId,
                    });
                    log += ">-99 request created #" + request.Id;

                    transaction.Commit();
                }
            }
            // assign random provider
            else
            {
                log += ">Member has no request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int PEPProviderId = SelectPEPProvider(memberId, companyId, isProduction);

                    log += ">New provider set #" + PEPProviderId;

                    request = PEPTransferRequestRepository.Insert(new PEPTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = PEPProviderId,
                    });

                    log += ">-99 request created #" + request.Id;
                    transaction.Commit();
                }
            }
            Logger.Fatal("CheckPEPAfterSubmit ==> Member: " + memberId + " STORY: " + log);
            return request;
        }
        #endregion
        #region providers
        public IList<PaymentProvider> AllPaymentProviderList()
        {
            IList<PaymentProvider> providerList = new List<PaymentProvider>() {
                new PaymentProvider() { Id = 31, Name = "AstroPay" },
                new PaymentProvider() { Id = 48, Name = "KingBankTransfer" },
                //new Provider() { Id = 33, Name = "BankTransfer" },
                new PaymentProvider() { Id = 34, Name = "CreditCard" },
                new PaymentProvider() { Id = 38, Name = "Credit Card (ReyPay)" },
                new PaymentProvider() { Id = 39, Name = "QR" },
                new PaymentProvider() { Id = 40, Name = "Crypto2" },
                new PaymentProvider() { Id = 45, Name = "EcoPayz" },
                new PaymentProvider() { Id = 46, Name = "OtoPay" },
                new PaymentProvider() { Id = 47, Name = "BankTransfer3" },
                new PaymentProvider() { Id = 49, Name = "Papara" },
                new PaymentProvider() { Id = 50, Name = "PayminoBankTransfer" },
                new PaymentProvider() { Id = 56, Name = "DFBankTransfer" },
                new PaymentProvider() { Id = 57, Name = "Insta Bank" },
                new PaymentProvider() { Id = 58, Name = "Hizirbank" },
                new PaymentProvider() { Id = 60, Name = "PayKasa" },
                new PaymentProvider() { Id = 61, Name = "Jeton" },
                new PaymentProvider() { Id = 66, Name = "Cashlink" },
                new PaymentProvider() { Id = 32, Name = "In-house Cepbank" },
                new PaymentProvider() { Id = 63, Name = "Jetcepbank" },
                new PaymentProvider() { Id = 65, Name = "Instantcepbank" },
                new PaymentProvider() { Id = 70, Name = "BPCommunityBank" },
                new PaymentProvider() { Id = 71, Name = "KingCommunityBank" },
                new PaymentProvider() { Id = 72, Name = "Crypto" },
                new PaymentProvider() { Id = 74, Name = "CMT" },
                new PaymentProvider() { Id = 78, Name = "Paylink" },
                new PaymentProvider() { Id = 80, Name = "Quickbit" },
                new PaymentProvider() { Id = 81, Name = "PaysUpBankTransfer" },
                new PaymentProvider() { Id = 82, Name = "MGCreditCard" },
                new PaymentProvider() { Id = 104, Name = "Mefete" },

            };

            return providerList;
        }
        public IList<PaymentProvider> PaymentProviderListForCompany(int companyId, bool isProduction)
        {
            IList<PaymentProvider> providerList = new List<PaymentProvider>();

            string paymentProviderInfo = CompanyService.GetValue(companyId, "PaymentProviderInfo", isProduction);

            JObject jObject;
            if (paymentProviderInfo != null)
            {
                jObject = (JObject)JsonConvert.DeserializeObject(paymentProviderInfo);
                JArray providers = (JArray)jObject["providers"];
                foreach (var provider in providers)
                {
                    JObject providerObject = (JObject)provider;
                    if (Convert.ToBoolean(providerObject["isOpen"]))
                    {
                        providerList.Add(new PaymentProvider()
                        {
                            Id = Convert.ToInt32(providerObject["id"]),
                            IsOpen = Convert.ToBoolean(providerObject["isOpen"]),
                            MinAmount = Convert.ToInt64(providerObject["minAmount"]),
                            MaxAmount = Convert.ToInt64(providerObject["maxAmount"]),
                            Currency = providerObject["currency"].ToString(),
                            DisplayOrder = Convert.ToInt32(providerObject["displayOrder"])
                        });
                    }
                }
            }


            return providerList.OrderBy(p => p.DisplayOrder).ToList();
        }
        public IList<NewPaymentProvider> NewPaymentProviderListForMember(int memberId, int companyId)
        {
            List<int> disabledProviders = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).Select(m => m.ProviderId).ToList();
            IList<NewPaymentProvider> paymentProviderList = NewPaymentProviderRepository.GetAll().Where(pp => pp.StatusType == 1 && pp.ParentProviderId == null && !disabledProviders.Contains(pp.VoltronProviderId.Value)).OrderBy(pp => pp.DisplayOrder).ToList();

            //Logger.Fatal(String.Format("Providers for member #{0} : {1}", memberId, string.Join(",", providerList.Select(p => p.Id))));
            return paymentProviderList;
        }
        public IList<NewPaymentProvider> NewPaymentProviderList(int companyId, bool showAll = false)
        {
            IQueryable<NewPaymentProvider> query = NewPaymentProviderRepository.GetAll();
            if (!showAll)
                query = query.Where(p => p.StatusType == 1);

            IList<NewPaymentProvider> paymentProviderList = query.OrderBy(pp => pp.DisplayOrder).ToList();

            //Logger.Fatal(String.Format("Providers for member #{0} : {1}", memberId, string.Join(",", providerList.Select(p => p.Id))));
            return paymentProviderList;
        }
        public void UpdateNewPaymentProvider(NewPaymentProvider newPaymentProvider)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    NewPaymentProviderRepository.Update(newPaymentProvider);
                    unitOfWork.Commit(transaction);
                }
            }

        }
        public IList<PaymentProvider> PaymentProviderListForMember(int memberId, int companyId, bool isProduction)
        {
            List<int> disabledProviders = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).Select(m => m.ProviderId).ToList();
            IList<PaymentProvider> providerList = new List<PaymentProvider>();

            string paymentProviderInfo = CompanyService.GetValue(companyId, "PaymentProviderInfo", isProduction, false);

            JObject jObject;
            if (paymentProviderInfo != null)
            {
                jObject = (JObject)JsonConvert.DeserializeObject(paymentProviderInfo);
                JArray providers = (JArray)jObject["providers"];
                foreach (var provider in providers)
                {
                    JObject providerObject = (JObject)provider;
                    if (Convert.ToBoolean(providerObject["isOpen"]) && !disabledProviders.Any(p => p == Convert.ToInt32(providerObject["id"])))
                    {
                        providerList.Add(new PaymentProvider()
                        {
                            Id = Convert.ToInt32(providerObject["id"]),
                            IsOpen = Convert.ToBoolean(providerObject["isOpen"]),
                            MinAmount = Convert.ToInt64(providerObject["minAmount"]),
                            MaxAmount = Convert.ToInt64(providerObject["maxAmount"]),
                            Currency = providerObject["currency"].ToString(),
                            DisplayOrder = Convert.ToInt32(providerObject["displayOrder"])
                        });
                    }
                }
            }

            //Logger.Fatal(String.Format("Providers for member #{0} : {1}", memberId, string.Join(",", providerList.Select(p => p.Id))));
            return providerList.OrderBy(p => p.DisplayOrder).ToList();
        }
        public PaymentProvider PaymentProviderForCompany(int providerId, int companyId, bool isProduction)
        {
            IList<PaymentProvider> providerList = new List<PaymentProvider>();

            string paymentProviderInfo = CompanyService.GetValue(companyId, "PaymentProviderInfo", isProduction);

            JObject jObject;
            if (paymentProviderInfo != null)
            {
                jObject = (JObject)JsonConvert.DeserializeObject(paymentProviderInfo);
                JArray providers = (JArray)jObject["providers"];
                foreach (var provider in providers)
                {
                    JObject providerObject = (JObject)provider;

                    if (Convert.ToInt32(providerObject["id"]) == providerId)
                    {
                        return new PaymentProvider()
                        {
                            Id = Convert.ToInt32(providerObject["id"]),
                            IsOpen = Convert.ToBoolean(providerObject["isOpen"]),
                            MinAmount = Convert.ToInt64(providerObject["minAmount"]),
                            MaxAmount = Convert.ToInt64(providerObject["maxAmount"]),
                            Currency = providerObject["currency"].ToString(),
                            DisplayOrder = Convert.ToInt32(providerObject["displayOrder"])
                        };
                    }
                }
            }

            return null;

        }







        public NewPaymentProvider GetNewPaymentProvider(int parentProviderId, int memberId, IEnumerable<int> triedAndGotErrorPaymentProviderIdList)
        {
            DateTime utcNow = DateTime.UtcNow.AddHours(3);//turkish timezone
            List<int> disabledProviders = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).Select(m => m.ProviderId).ToList();
            IList<NewPaymentProvider> newPaymentProviderList = NewPaymentProviderRepository.GetAll().Where(npp => !disabledProviders.Contains(npp.Id) && !triedAndGotErrorPaymentProviderIdList.Contains(npp.Id) && npp.StatusType == 1 && npp.ParentProviderId == parentProviderId).ToList();

            IList<NewPaymentProvider> activeProviderList = new List<NewPaymentProvider>();
            foreach (NewPaymentProvider newPaymentProvider in newPaymentProviderList)
            {
                if (!string.IsNullOrEmpty(newPaymentProvider.ClosedCron))
                {
                    var scheduler = CrontabSchedule.Parse(newPaymentProvider.ClosedCron);
                    var closedMinutes = scheduler.GetNextOccurrences(utcNow.AddMinutes(-1), utcNow.AddMinutes(1));
                    if (!closedMinutes.Contains(new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, 0)))
                        activeProviderList.Add(newPaymentProvider);
                }
                else
                    activeProviderList.Add(newPaymentProvider);
            }
            IList<NewPaymentProvider> tempList = new List<NewPaymentProvider>();
            foreach (NewPaymentProvider newPaymentProvider in activeProviderList)
            {
                for (int i = 0; i < newPaymentProvider.Weight; i++)
                {
                    tempList.Add(newPaymentProvider);
                }
            }


            if (tempList.Count > 0)
                return tempList[new Random().Next(0, tempList.Count)];
            else
                return null;
        }


        public NewPaymentProvider GetNewPaymentProviderBySystemName(string systemName)
        {
            return NewPaymentProviderRepository.GetAll().Where(npp => npp.StatusType == 1 && npp.SystemName == systemName).FirstOrDefault();
        }






        #endregion
        #region generic deposit request

        public DepositResult InsertGenericDepositRequest(int memberId, int providerId, long amount, bool withBonus, int? bonusId, int companyId, string note, Object payload)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);
                DepositResult result = new DepositResult { Success = false };
                try
                {
                    var jsonPayload = JsonConvert.SerializeObject(payload);


                    GenericDepositRequestRepository.Insert(new GenericDepositRequest()
                    {
                        Amount = amount,
                        UpdateAmount = amount,
                        RecognisedAmount = amount,
                        MemberId = memberId,
                        ProviderId = providerId,
                        PaymentStatusType = (int)PaymentStatusType.Pending,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        Payload = jsonPayload,
                        WithBonus = withBonus,
                        BonusId = bonusId,
                        Note = note,
                        CompanyId = companyId
                    });

                    transaction.Commit();
                    result.Success = true;

                }
                catch (Exception ex)
                {

                }
                return result;

            }

        }

        public IList<GenericDepositRequest> PendingGenericDepositRequestList(int memberId, int providerId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                return GenericDepositRequestRepository.GetAll().Where(ba => ba.PaymentStatusType == (int)PaymentStatusType.Pending && ba.MemberId == memberId && ba.ProviderId == providerId).ToList(); // pendings
            }
        }

        public IList<GenericDepositRequest> PendingGenericDepositRequestList(int memberId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                return GenericDepositRequestRepository.GetAll().Where(ba => ba.PaymentStatusType == (int)PaymentStatusType.Pending && ba.MemberId == memberId).ToList(); // pendings
            }
        }
        public IList<GenericDepositRequest> SuccessfulGenericDepositRequestList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return GenericDepositRequestRepository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Approved && p.MemberId == memberId).ToList(); // success
            }
        }
        #endregion
        #region BankTransferV2

        public IList<BankTransferV2Request> BankTransferV2List(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return BankTransferRequestV2Repository.GetAll().Where(p => p.MemberId == memberId).ToList();
            }
        }
        public IList<BankTransferV2Request> PendingBankTransferV2List(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return BankTransferRequestV2Repository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Pending && p.MemberId == memberId).ToList(); // success
            }
        }
        public IList<BankTransferV2Request> SuccessfulBankTransferV2List(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return BankTransferRequestV2Repository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Approved && p.MemberId == memberId).ToList(); // success
            }
        }

        public IList<BankTransferRequestV2Model> SuccessfulBankTransferRequestV2ModelList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                IList<BankTransferRequestV2Model> bankTransferRequestV2ModelList = new List<BankTransferRequestV2Model>();

                var bankTransferRequest = BankTransferRequestV2Repository.GetAll().OrderByDescending(p => p.CreateDate).FirstOrDefault(p => p.MemberId == memberId); // success

                if (bankTransferRequest != null && bankTransferRequest.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.BTProviderInfoSet)
                {
                    Member member = MemberRepository.Get(bankTransferRequest.MemberId);

                    Bank bank = BankRepository.Get(bankTransferRequest.RequestBankId);
                    Bank receiverBankId = BankRepository.Get(bankTransferRequest.ReceiverBankId);

                    //BankTransferBankAccount bankTransferBankAccount = BankTransferBankAccountRepository.Get(cb.BankTransferBankAccountId);
                    BankTransferRequestV2Model bankTransferRequestModel = new BankTransferRequestV2Model();
                    bankTransferRequestModel.Id = bankTransferRequest.Id.ToString();
                    bankTransferRequestModel.Username = member.Username;
                    bankTransferRequestModel.MemberId = member.Id;
                    bankTransferRequestModel.SenderFirstname = bankTransferRequest.SenderFirstname;
                    bankTransferRequestModel.SenderLastname = bankTransferRequest.SenderLastname;
                    bankTransferRequestModel.SenderIdentityNumber = bankTransferRequest.SenderIdentityNumber;
                    bankTransferRequestModel.AmountDecimal = bankTransferRequest.UpdateAmount;
                    bankTransferRequestModel.Amount = (bankTransferRequest.UpdateAmount / 100).ToString("N");
                    bankTransferRequestModel.UsersAmount = (bankTransferRequest.Amount / 100).ToString("N");
                    bankTransferRequestModel.ReceiverAccountNumber = String.IsNullOrEmpty(bankTransferRequest.ReceiverAccountNumber) ? "" : bankTransferRequest.ReceiverAccountNumber;
                    bankTransferRequestModel.ReceiverIBAN = bankTransferRequest.ReceiverIBAN;
                    bankTransferRequestModel.ReceiverFullname = bankTransferRequest.ReceiverFullname;
                    bankTransferRequestModel.CreateDateDate = bankTransferRequest.CreateDate;
                    bankTransferRequestModel.CreateDate = bankTransferRequest.CreateDate.ToString("dd.MM.yyyy HH:mm");
                    bankTransferRequestModel.FastEnabled = bankTransferRequest.FastEnabled ? "YES" : "NO";
                    bankTransferRequestModel.Status = ((WithdrawStatusType)bankTransferRequest.PaymentStatusType).ToString();
                    bankTransferRequestModel.LevelName = member.Level.Name;
                    bankTransferRequestModel.CompanyId = bankTransferRequest.CompanyId;
                    bankTransferRequestModel.RequestBankName = bank != null ? bank.Name : string.Empty;
                    bankTransferRequestModel.RequestBankId = bankTransferRequest.RequestBankId;
                    bankTransferRequestModel.ReceiverBranchCode = bankTransferRequest.ReceiverBranchCode;
                    bankTransferRequestModel.ReceiverReference = bankTransferRequest.ReceiverReference;
                    bankTransferRequestModel.ReceiverBankId = bankTransferRequest.ReceiverBankId.ToString();
                    bankTransferRequestModel.ReceiverBankName = receiverBankId != null ? receiverBankId.Name : string.Empty;


                    bankTransferRequestV2ModelList.Add(bankTransferRequestModel);

                }
                return bankTransferRequestV2ModelList;
            }
        }

        public IList<BankTransferV2BankTransferAccount> BankTransferV2BankTransferAccountList()
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return BankTransferV2BankTransferAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public IList<BankTransferV2BankTransferAccount> BankTransferV2BankTransferAccountList(Member member)
        {
            return BankTransferV2BankTransferAccountListForLevel(member.LevelId.Value);
        }
        public IList<BankTransferV2BankTransferAccount> BankTransferV2BankTransferAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                Member member = MemberService.GetMember(memberId);
                return BankTransferV2BankTransferAccountList(member);
            }
        }
        public IList<BankTransferV2BankTransferAccount> BankTransferV2BankTransferAccountList(int memberId, int providerId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                //Provider provider = ProviderService.GetProviderByVoltronProviderId(providerId); 
                return BankTransferV2BankTransferAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active && ptpa.ProviderId == providerId).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public IList<BankTransferV2BankTransferAccount> BankTransferV2BankTransferAccountListForLevel(Level level)
        {
            return BankTransferV2BankTransferAccountListForLevel(level.Id);
        }
        public IList<BankTransferV2BankTransferAccount> BankTransferV2BankTransferAccountListForLevel(int levelId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return BankTransferV2BankTransferAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }

        public IList<BankTransferV2Request> CheckBankTransferV2RequestListIn20Minutes(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                // checking if this member has requested within 20 minutes.

                DateTime currentUTCDateTime = DateTime.UtcNow.AddMinutes(-20);

                int[] paymentStatusType = new int[] { (int)PaymentStatusType.Pending, (int)PaymentStatusType.BTProviderInfoSet };

                return BankTransferRequestV2Repository.GetAll().Where(a => a.MemberId == memberId && a.CreateDate >= currentUTCDateTime && paymentStatusType.Contains(a.PaymentStatusType)).ToList();
            }
        }

        public void InsertBankTransferV2Request(int memberId, long amount, string identityNumber, string firstname, string lastname, int requestBankId, bool fastEnabled, int companyId, bool isProduction)
        {
            int? selectedPaymentProvider = SelectBankTransferV2Provider(memberId, companyId, isProduction, amount / 100, null);
            if (selectedPaymentProvider.HasValue)
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    if (CheckBankTransferV2RequestListIn20Minutes(memberId).Count == 0)
                    {
                        BankTransferRequestV2Repository.Insert(new BankTransferV2Request()
                        {
                            PaymentStatusType = (int)PaymentStatusType.Pending,
                            PaymentProviderId = selectedPaymentProvider.Value,
                            MemberId = memberId,
                            CreateDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow,
                            Amount = amount,
                            UpdateAmount = amount,
                            RecognisedAmount = amount,
                            SenderIdentityNumber = identityNumber,
                            SenderFirstname = firstname,
                            SenderLastname = lastname,
                            RequestBankId = requestBankId,
                            FastEnabled = fastEnabled,
                            CompanyId = companyId
                        });

                        transaction.Commit();
                    }
                }
            }

        }
        public void UpdateBankTransferV2Request(int BankTransferV2RequestId, int BankTransferV2BankTransferAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId)
        {

            //using (var uniOfWork = UnitOfWork.Current)
            //{
            //    ITransaction transaction = uniOfWork.BeginTransaction(Session);

            //    BankTransferV2Request BankTransferV2Request = BankTransferV2Repository.Get(BankTransferV2RequestId);
            //    BankTransferV2Request.Amount = amount;
            //    BankTransferV2Request.UpdateAmount = amount;
            //    BankTransferV2Request.AccountNumber = accountNumber;
            //    BankTransferV2Request.BankTransferV2BankTransferAccountId = BankTransferV2BankTransferAccountId;
            //    BankTransferV2Request.PaymentStatusType = (int)PaymentStatusType.Pending;
            //    BankTransferV2Request.CreateDate = DateTime.UtcNow;
            //    BankTransferV2Request.UpdateDate = DateTime.UtcNow;
            //    BankTransferV2Request.SenderFullName = String.Format("{0} {1}", firstname, lastname);
            //    BankTransferV2Request.WithBonus = withBonus;
            //    BankTransferV2Request.BonusId = bonusId;
            //    BankTransferV2Request.Note = note;
            //    BankTransferV2Request.CompanyId = companyId;

            //    BankTransferRequestV2Repository.Update(BankTransferV2Request);
            //    transaction.Commit();

            //    NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made Parazula transfer and filled out the form, amount: " + amount * 0.01 + " TL");
            //}
        }
        public int? SelectBankTransferV2Provider(int memberId, int companyId, bool isProduction, long? amount = null, int? exceptProviderId = null)
        {
            int? newId = null;
            List<NewPaymentProvider> paparaProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 51).ToList();

            //commented out part enables provider management for papara providers
            List<int> disabledProviders = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).Select(m => m.ProviderId).ToList();

            List<int> tempProviderIds = new List<int>();
            foreach (NewPaymentProvider p in paparaProviders)
            {
                if (paparaProviders.Count > 1 && exceptProviderId.HasValue && exceptProviderId.Value == p.VoltronProviderId)
                    continue;


                if (!disabledProviders.Any(dp => dp == p.VoltronProviderId))
                {
                    int weight = p.Weight.HasValue ? p.Weight.Value : 0;
                    for (int i = weight; i > 0; i--)
                    {
                        if (amount.HasValue && p.MinAmount <= amount.Value && p.MaxAmount > amount.Value)
                        {
                            //tempProviderIds.Add(p.VoltronProviderId.Value);
                            tempProviderIds.Add(p.VoltronProviderId.Value);
                        }
                    }
                }
            }

            if (tempProviderIds.Count > 0)
            {
                Random rnd = new Random();
                int r = rnd.Next(tempProviderIds.Count);
                newId = tempProviderIds[r];
            }
            return newId;
        }

        public Boolean UpdateBankTransferV2RequestPaymentStatusType(int Id, int PaymentStatusTypeId)
        {
            Boolean Result = false;
            try
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    BankTransferV2Request BankTransferV2Request = BankTransferRequestV2Repository.Get(Id);
                    if (BankTransferV2Request != null && BankTransferV2Request.Id > 0)
                    {
                        BankTransferV2Request.PaymentStatusType = PaymentStatusTypeId;
                        BankTransferV2Request.UpdateDate = DateTime.UtcNow;

                        BankTransferRequestV2Repository.Update(BankTransferV2Request);
                        transaction.Commit();
                        Result = true;
                    }

                }
            }
            catch (Exception ex)
            {
                Result = false;
            }

            return Result;
        }

        #endregion
        #region Payfix

        public IList<PayfixTransferRequest> PayfixTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PayfixTransferRequestRepository.GetAll().Where(p => p.MemberId == memberId).ToList();
            }
        }
        public IList<PayfixTransferRequest> PendingPayfixTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PayfixTransferRequestRepository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Pending && p.MemberId == memberId).ToList(); // success
            }
        }
        public IList<PayfixTransferRequest> SuccessfulPayfixTransferList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PayfixTransferRequestRepository.GetAll().Where(p => p.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Approved && p.MemberId == memberId).ToList(); // success
            }
        }
        public IList<PayfixAccount> PayfixAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {


                return PayfixAccountRepository.GetAll().Where(pa => pa.StatusType == (int)StatusType.Active && pa.MemberId == memberId).OrderByDescending(pa => pa.CreateDate).ToList();
            }
        }
        public PayfixAccount InsertPayfixAccount(int memberId, string firstname, string lastname, string accountNumber, string currency)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                PayfixAccount PayfixAccount;
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    PayfixAccount = PayfixAccountRepository.Insert(new PayfixAccount() { MemberId = memberId, Firstname = firstname, Lastname = lastname, StatusType = (int)StatusType.Active, CreateDate = DateTime.UtcNow, AccountNumber = accountNumber, Currency = currency });
                    uniOfWork.Commit(transaction);

                }

                return PayfixAccount;
            }
        }
        public PayfixAccount UpdatePayfixAccount(PayfixAccount payfixAccount)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    payfixAccount = PayfixAccountRepository.Update(payfixAccount);
                    uniOfWork.Commit(transaction);
                }
                return payfixAccount;
            }
        }
        public void DeletePayfixAccount(int payfixAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    PayfixAccount PayfixAccount = PayfixAccountRepository.Get(payfixAccountId);
                    PayfixAccount.StatusType = (int)StatusType.Passive;
                    PayfixAccount = PayfixAccountRepository.Update(PayfixAccount);
                    uniOfWork.Commit(transaction);
                }
            }

        }
        public PayfixAccount PayfixAccount(int memberId, int payfixAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                return PayfixAccountRepository.GetAll().FirstOrDefault(pa => pa.MemberId == memberId && pa.Id == payfixAccountId);
            }
        }
        public PayfixAccount PayfixAccount(int payfixAccountId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                return PayfixAccountRepository.GetAll().FirstOrDefault(pa => pa.Id == payfixAccountId);
            }
        }

        public IList<PayfixTransferPayfixAccount> PayfixTransferPayfixAccountList()
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PayfixTransferPayfixAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public IList<PayfixTransferPayfixAccount> PayfixTransferPayfixAccountList(Member member)
        {
            return PayfixTransferPayfixAccountListForLevel(member.LevelId.Value);
        }
        public IList<PayfixTransferPayfixAccount> PayfixTransferPayfixAccountList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                Member member = MemberService.GetMember(memberId);
                return PayfixTransferPayfixAccountList(member);
            }
        }
        public IList<PayfixTransferPayfixAccount> PayfixTransferPayfixAccountList(int memberId, int providerId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                //Provider provider = ProviderService.GetProviderByVoltronProviderId(providerId); 
                return PayfixTransferPayfixAccountRepository.GetAll().Where(ptpa => ptpa.StatusType == (int)StatusType.Active && ptpa.ProviderId == providerId).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public IList<PayfixTransferPayfixAccount> PayfixTransferPayfixAccountListForLevel(Level level)
        {
            return PayfixTransferPayfixAccountListForLevel(level.Id);
        }
        public IList<PayfixTransferPayfixAccount> PayfixTransferPayfixAccountListForLevel(int levelId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return PayfixTransferPayfixAccountRepository.GetAll().Where(ptpa => ptpa.Levels.Any(l => l.Id == levelId) && ptpa.StatusType == (int)StatusType.Active).OrderBy(ptpa => ptpa.NameSurname).ToList();
            }
        }
        public void InsertPayfixTransferRequest(int memberId, int payfixTransferPayfixAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);

                PayfixTransferRequestRepository.Insert(new PayfixTransferRequest()
                {
                    Amount = amount,
                    UpdateAmount = amount,
                    RecognisedAmount = (amount * 100),
                    AccountNumber = accountNumber,
                    MemberId = memberId,
                    PayfixTransferPayfixAccountId = payfixTransferPayfixAccountId,
                    PaymentStatusType = (int)PaymentStatusType.Pending,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    SenderFullName = String.Format("{0} {1}", firstname, lastname),
                    WithBonus = withBonus,
                    BonusId = bonusId,
                    Note = note,
                    CompanyId = companyId,
                    ProviderId = 90 // TODO
                });

                transaction.Commit();

                NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made Payfix transfer and filled out the form, amount: " + amount * 0.01 + " TL");
            }
        }
        public void UpdatePayfixTransferRequest(int payfixTransferRequestId, int payfixTransferPayfixAccountId, int paymentStatusType, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId, string referenceId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);

                PayfixTransferRequest PayfixTransferRequest = PayfixTransferRequestRepository.Get(payfixTransferRequestId);
                PayfixTransferRequest.Amount = amount;
                PayfixTransferRequest.UpdateAmount = amount;
                PayfixTransferRequest.RecognisedAmount = amount * 100;
                PayfixTransferRequest.AccountNumber = accountNumber;
                PayfixTransferRequest.PayfixTransferPayfixAccountId = payfixTransferPayfixAccountId;
                PayfixTransferRequest.PaymentStatusType = paymentStatusType;
                PayfixTransferRequest.CreateDate = DateTime.UtcNow;
                PayfixTransferRequest.UpdateDate = DateTime.UtcNow;
                PayfixTransferRequest.SenderFullName = String.Format("{0} {1}", firstname, lastname);
                PayfixTransferRequest.WithBonus = withBonus;
                PayfixTransferRequest.BonusId = bonusId;
                PayfixTransferRequest.Note = note;
                PayfixTransferRequest.CompanyId = companyId;
                PayfixTransferRequest.ReferenceId = referenceId;

                PayfixTransferRequestRepository.Update(PayfixTransferRequest);
                transaction.Commit();

                NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made Payfix transfer and filled out the form, amount: " + amount * 0.01 + " TL");
            }
        }
        public int SelectPayfixProvider(int memberId, int companyId, bool isProduction)
        {
            List<NewPaymentProvider> paparaProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 39).ToList();

            //commented out part enables provider management for papara providers
            //List<int> disabledProviders = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).Select(m => m.ProviderId).ToList();

            List<int> tempProviderIds = new List<int>();
            foreach (NewPaymentProvider p in paparaProviders)
            {
                //if (!disabledProviders.Any(dp => dp == p.VoltronProviderId))
                //{
                int weight = p.Weight.HasValue ? p.Weight.Value : 0;
                for (int i = weight; i > 0; i--)
                {
                    tempProviderIds.Add(p.VoltronProviderId.Value);
                }
                //}
            }
            Random rnd = new Random();
            int r = rnd.Next(tempProviderIds.Count);
            return tempProviderIds[r];
        }
        public int SelectPayfixProvider(int memberId, int companyId, bool isProduction, long? amount = null)
        {

            List<NewPaymentProvider> payfixProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 39).ToList();

            //commented out part enables provider management for Payfix providers
            List<int> disabledProviders = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).Select(m => m.ProviderId).ToList();

            List<int> tempProviderIds = new List<int>();
            foreach (NewPaymentProvider p in payfixProviders)
            {
                if (!disabledProviders.Any(dp => dp == p.VoltronProviderId))
                {
                    int weight = p.Weight.HasValue ? p.Weight.Value : 0;
                    for (int i = weight; i > 0; i--)
                    {
                        if (amount.HasValue && p.MinAmount <= amount.Value && p.MaxAmount > amount.Value)
                        {
                            tempProviderIds.Add(p.VoltronProviderId.Value);
                        }
                    }
                }
            }
            Random rnd = new Random();
            int r = rnd.Next(tempProviderIds.Count);
            return tempProviderIds[r];
        }
        public PayfixPaymentProvider GetPayfixPaymentProvider(int memberId, int companyId, long amount, bool isProduction)
        {
            PayfixPaymentProvider payfixPaymentProvider = new PayfixPaymentProvider();
            PayfixTransferRequest request = new PayfixTransferRequest();
            // check if a provider is assigned to user when s/he visits Payfix form page
            // check for 15 mins to make sure if scheduled job failed
            string log = string.Empty;
            int payfixProviderId = 0;
            List<NewPaymentProvider> PayfixProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 39).ToList();
            if (PayfixTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.Amount == amount && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15)))
            {

                request = PayfixTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.Amount == amount && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15));
                payfixProviderId = request.ProviderId.Value;
                log += ">Member has -99 request #" + request.Id + " for provider #" + request.ProviderId.Value;

                // change if the assigned provider is closed
                if (!PayfixProviders.Any(p => p.VoltronProviderId == request.ProviderId))
                {
                    log += ">But provider is turned off";
                    using (var uniOfWork = UnitOfWork.Current)
                    {
                        ITransaction transaction = uniOfWork.BeginTransaction(Session);

                        // assign new provider id & reset time
                        payfixProviderId = SelectPayfixProvider(memberId, companyId, isProduction, amount);

                        request.ProviderId = payfixProviderId;
                        request.CreateDate = DateTime.UtcNow;
                        request.UpdateDate = DateTime.UtcNow;


                        request = PayfixTransferRequestRepository.Update(request);

                        log += ">New provider set #" + payfixProviderId + ". Now provider is(checking) #" + request.ProviderId;
                        transaction.Commit();
                    }
                }
            }
            // check if user has any rejected request within last 15 minutes
            else if (PayfixTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.Amount == amount && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)))
            {
                log += ">Member has rejected request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    payfixProviderId = PayfixTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)).ProviderId.Value;
                    log += ">for provider #" + payfixProviderId;

                    // change if the assigned provider is closed. if so assign another provider
                    if (!PayfixProviders.Any(p => p.VoltronProviderId == request.ProviderId))
                    {
                        log += ">But provider is turned off";
                        payfixProviderId = SelectPayfixProvider(memberId, companyId, isProduction, amount);
                        log += ">New provider set #" + payfixProviderId;
                    }

                    request = PayfixTransferRequestRepository.Insert(new PayfixTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = payfixProviderId,
                        Amount = amount,
                        UpdateAmount = amount,
                        RecognisedAmount = amount * 100
                    });
                    log += ">-99 request created #" + request.Id;

                    transaction.Commit();
                }
            }
            // assign random provider
            else
            {
                log += ">Member has no request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    payfixProviderId = SelectPayfixProvider(memberId, companyId, isProduction, amount);

                    log += ">New provider set #" + payfixProviderId;

                    request = PayfixTransferRequestRepository.Insert(new PayfixTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = payfixProviderId,
                        Amount = amount,
                        UpdateAmount = amount,
                        RecognisedAmount = amount * 100
                    });

                    log += ">-99 request created #" + request.Id;
                    transaction.Commit();
                }
            }
            Logger.Fatal("CheckPayfixPageLoad ==>Member: " + memberId + " STORY: " + log);

            NewPaymentProvider paymentProvider = PayfixProviders.FirstOrDefault(pp => pp.VoltronProviderId == payfixProviderId);

            payfixPaymentProvider.NewPaymentProvider = paymentProvider;
            payfixPaymentProvider.PayfixRequestId = request.Id;
            return payfixPaymentProvider;
        }
        public PayfixTransferRequest CheckPayfixRequest(int memberId, int companyId, bool isProduction)
        {
            PayfixTransferRequest request = new PayfixTransferRequest();
            // check if a provider is assigned to user when s/he visits Payfix form page
            // check for 15 mins to make sure if scheduled job failed
            string log = string.Empty;
            if (PayfixTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15)))
            {

                request = PayfixTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15));
                log += ">Member has -99 request #" + request.Id + " for provider #" + request.ProviderId.Value;


                List<NewPaymentProvider> payfixProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 33).ToList();
                // change if the assigned provider is closed
                if (!payfixProviders.Any(p => p.VoltronProviderId == request.ProviderId))
                {
                    log += ">But provider is turned off";
                    using (var uniOfWork = UnitOfWork.Current)
                    {
                        ITransaction transaction = uniOfWork.BeginTransaction(Session);

                        // assign new provider id & reset time
                        int PayfixProviderId = SelectPayfixProvider(memberId, companyId, isProduction);

                        request.ProviderId = PayfixProviderId;
                        request.CreateDate = DateTime.UtcNow;
                        request.UpdateDate = DateTime.UtcNow;


                        request = PayfixTransferRequestRepository.Update(request);

                        log += ">New provider set #" + PayfixProviderId + ". Now provider is(checking) #" + request.ProviderId;
                        transaction.Commit();
                    }
                }
            }
            // check if user has any rejected request within last 15 minutes
            else if (PayfixTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)))
            {
                log += ">Member has rejected request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int payfixProviderId = PayfixTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)).ProviderId.Value;
                    log += ">for provider #" + payfixProviderId;

                    List<NewPaymentProvider> PayfixProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 33).ToList();

                    // change if the assigned provider is closed. if so assign another provider
                    if (!PayfixProviders.Any(p => p.VoltronProviderId == request.ProviderId))
                    {
                        log += ">But provider is turned off";
                        payfixProviderId = SelectPayfixProvider(memberId, companyId, isProduction);
                        log += ">New provider set #" + payfixProviderId;
                    }

                    request = PayfixTransferRequestRepository.Insert(new PayfixTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = payfixProviderId,
                    });
                    log += ">-99 request created #" + request.Id;

                    transaction.Commit();
                }
            }
            // assign random provider
            else
            {
                log += ">Member has no request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int payfixProviderId = SelectPayfixProvider(memberId, companyId, isProduction);

                    log += ">New provider set #" + payfixProviderId;

                    request = PayfixTransferRequestRepository.Insert(new PayfixTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = payfixProviderId,
                    });

                    log += ">-99 request created #" + request.Id;
                    transaction.Commit();
                }
            }
            Logger.Fatal("CheckPayfixPageLoad ==>Member: " + memberId + " STORY: " + log);
            return request;
        }
        public PayfixTransferRequest CheckPayfixRequestAfterSubmit(int memberId, int companyId, bool isProduction)
        {
            PayfixTransferRequest request = new PayfixTransferRequest();
            // check if a provider is assigned to user when s/he visits Payfix form page
            // check for 15 mins to make sure if scheduled job failed
            string log = string.Empty;
            if (PayfixTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15)))
            {

                request = PayfixTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == -99 && pr.CreateDate > DateTime.UtcNow.AddMinutes(-15));
                log += ">Member has -99 request #" + request.Id + " for provider #" + request.ProviderId.Value;


                List<NewPaymentProvider> payfixProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 33).ToList();
                // change if the assigned provider is closed
                if (!payfixProviders.Any(p => p.VoltronProviderId == request.ProviderId))
                {
                    log += ">But provider is turned off";
                    using (var uniOfWork = UnitOfWork.Current)
                    {
                        ITransaction transaction = uniOfWork.BeginTransaction(Session);

                        // assign new provider id & reset time
                        int payfixProviderId = SelectPayfixProvider(memberId, companyId, isProduction);

                        request.ProviderId = payfixProviderId;
                        request.CreateDate = DateTime.UtcNow;
                        request.UpdateDate = DateTime.UtcNow;


                        request = PayfixTransferRequestRepository.Update(request);

                        log += ">New provider set #" + payfixProviderId + ". Now provider is(checking) #" + request.ProviderId;
                        transaction.Commit();
                    }
                }
            }
            // check if user has any rejected request within last 15 minutes
            else if (PayfixTransferRequestRepository.GetAll().Any(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)))
            {
                log += ">Member has rejected request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int payfixProviderId = PayfixTransferRequestRepository.GetAll().FirstOrDefault(pr => pr.MemberId == memberId && pr.PaymentStatusType == (int)PaymentStatusType.Rejected && pr.UpdateDate > DateTime.UtcNow.AddMinutes(-15)).ProviderId.Value;
                    log += ">for provider #" + payfixProviderId;

                    IList<NewPaymentProvider> payfixProviders = NewPaymentProviderList(companyId).Where(p => p.ParentProviderId == 33).ToList();//Payfix providers

                    // change if the assigned provider is closed. if so assign another provider
                    if (!payfixProviders.Any(p => p.VoltronProviderId == payfixProviderId))
                    {
                        log += ">But provider is turned off";
                        payfixProviderId = SelectPayfixProvider(memberId, companyId, isProduction);
                        log += ">New provider set #" + payfixProviderId;
                    }

                    request = PayfixTransferRequestRepository.Insert(new PayfixTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = payfixProviderId,
                    });
                    log += ">-99 request created #" + request.Id;

                    transaction.Commit();
                }
            }
            // assign random provider
            else
            {
                log += ">Member has no request";
                using (var uniOfWork = UnitOfWork.Current)
                {
                    ITransaction transaction = uniOfWork.BeginTransaction(Session);

                    int payfixProviderId = SelectPayfixProvider(memberId, companyId, isProduction);

                    log += ">New provider set #" + payfixProviderId;

                    request = PayfixTransferRequestRepository.Insert(new PayfixTransferRequest()
                    {
                        MemberId = memberId,
                        PaymentStatusType = -99,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        CompanyId = companyId,
                        ProviderId = payfixProviderId,
                    });

                    log += ">-99 request created #" + request.Id;
                    transaction.Commit();
                }
            }
            Logger.Fatal("CheckPayfixAfterSubmit ==> Member: " + memberId + " STORY: " + log);
            return request;
        }
        #endregion
        public bool HasPendingRequestInBankTransferGroup(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                if (GenericDepositRequestRepository.GetAll().Any(ba => ba.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Pending && ba.MemberId == memberId)) // al generic deposit request providers
                {
                    return true;
                }
                if (BankTransferRequestRepository.GetAll().Any(ba => ba.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Pending && ba.MemberId == memberId))
                {
                    return true;
                }
                /*if (PayminoRequest2Repository.GetAll().Any(pr => pr.MemberId == memberId && pr.StatusType == (int)PaymentStatusType.Pending && pr.UpdateDate != null))
                {
                    return true;
                }*/
            }
            return false;
        }

        #region new bank transfer request 

        public int SelectBTProvider(int memberId, int companyId, bool isProduction)
        {
            string[] btSystemNames = { "dfbanktransfer", "instabank", "hizirbank" };
            List<Provider> btProviders = ProviderService.GetAllProviders(2).Where(p => btSystemNames.Contains(p.SystemName)).ToList();

            List<int> disabledProviders = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == memberId).Select(m => m.ProviderId).ToList();

            //btProviders = btProviders.Where(p => p.ProviderSettings.FirstOrDefault(s => s.Name == "IsOpen").Value == "True").ToList(); //TODO
            List<int> tempProviderIds = new List<int>();
            foreach (Provider p in btProviders)
            {
                if (!disabledProviders.Any(dp => dp == p.VoltronProviderId))
                {
                    string weightString = ProviderService.GetValue(p.Id, companyId, "Weight", isProduction);
                    int weight;
                    bool isNumeric = int.TryParse(weightString, out weight);
                    if (isNumeric)
                    {
                        for (int i = weight; i > 0; i--)
                        {
                            tempProviderIds.Add(p.VoltronProviderId);
                        }
                    }
                }
            }
            if (tempProviderIds.Count > 0)
            {
                Random rnd = new Random();
                int r = rnd.Next(tempProviderIds.Count);
                return tempProviderIds[r];
            }
            else
            {
                return -1; //TODO
            }
        }
        public int? SelectNewBTProvider(NewBankTransferRequest newBankTransferRequest, bool isProduction)
        {

            string[] btSystemNames = { "dfbanktransfer", "instabank", "hizirbank" };
            List<Provider> btProviders = ProviderService.GetAllProviders(2).Where(p => btSystemNames.Contains(p.SystemName)).ToList();

            List<int> disabledProviders = MemberDisabledPaymentMethodRepository.GetAll().Where(m => m.MemberId == newBankTransferRequest.MemberId).Select(m => m.ProviderId).ToList();

            //btProviders = btProviders.Where(p => p.ProviderSettings.FirstOrDefault(s => s.Name == "IsOpen").Value == "True").ToList(); //TODO
            List<int> tempProviderIds = new List<int>();
            foreach (Provider p in btProviders)
            {
                if (!disabledProviders.Any(dp => dp == p.VoltronProviderId) && !newBankTransferRequest.ProviderHistory.Select(ph => ph.ProviderId).Contains(p.VoltronProviderId))
                {
                    string weightString = ProviderService.GetValue(p.Id, newBankTransferRequest.CompanyId, "Weight", isProduction);
                    int weight;
                    bool isNumeric = int.TryParse(weightString, out weight);
                    if (isNumeric)
                    {
                        for (int i = weight; i > 0; i--)
                        {
                            tempProviderIds.Add(p.VoltronProviderId);
                        }
                    }
                }
            }
            if (tempProviderIds.Count > 0)
            {
                Random rnd = new Random();
                int r = rnd.Next(tempProviderIds.Count);
                return tempProviderIds[r];
            }
            else
            {
                return null;
            }
        }
        public DepositResult InsertNewBankTransferRequest(int memberId, int providerId, long amount, string note, string firstName, string lastName, int bankId, bool isEFT, string identityNumber, bool withBonus, int? bonusId, int companyId)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);
                DepositResult result = new DepositResult { Success = false };
                try
                {

                    NewBankTransferRequest newBankTransferRequest = NewBankTransferRequestRepository.Insert(new NewBankTransferRequest()
                    {
                        ProviderId = providerId,
                        Amount = amount,
                        UpdateAmount = amount,
                        MemberId = memberId,
                        PaymentStatusType = (int)PaymentStatusType.BTFormFilled,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        SenderFullName = String.Format("{0} {1}", firstName, lastName),
                        IdentityNumber = identityNumber,
                        WithBonus = withBonus,
                        BonusId = bonusId,
                        Note = note,
                        CompanyId = companyId,
                        SenderBankId = bankId //TODO
                    });

                    BankTransferRequestProviderHistoryRepository.Insert(new BankTransferRequestProviderHistory()
                    {
                        BankTransferRequestId = newBankTransferRequest.Id,
                        ProviderId = providerId,
                        ChangedBy = -1,
                        CreateDate = DateTime.UtcNow,
                        NewBankTransferRequest = newBankTransferRequest,
                    });

                    transaction.Commit();
                    result.Success = true;
                    NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made bank transfer and filled out the form, amount: " + amount * 0.01 + " TL");

                }
                catch (Exception ex)
                {

                }
                return result;

            }
        }
        public DepositResult UpdateNewBankTransferRequest(NewBankTransferRequest newBankTransferRequest)
        {

            using (var uniOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = uniOfWork.BeginTransaction(Session);
                DepositResult result = new DepositResult { Success = false };
                try
                {

                    newBankTransferRequest.UpdateDate = DateTime.UtcNow;

                    transaction.Commit();
                    result.Success = true;
                    NWWebHook.PushBackOffice("www.baymavi7.com", "g:bo", "", "Player made bank transfer and filled out the form, amount: " + newBankTransferRequest.Amount * 0.01 + " TL");

                }
                catch (Exception ex)
                {

                }
                return result;

            }
        }

        public IList<NewBankTransferRequest> NewBankTransferRequestForMember(int memberId, int[] paymentStatusTypes)
        {
            return NewBankTransferRequestRepository.GetAll().Where(bt => bt.MemberId == memberId && paymentStatusTypes.Contains(bt.PaymentStatusType)).ToList();
        }
        #endregion

        private int ChangeIntegrationStatusType(int statusType)
        {
            switch (statusType)
            {
                case 1:
                    return 0;
                case 2:
                    return 1;
                default:
                    return statusType;
            }
        }

        public IList<Tuple<string, decimal, string>> BalanceList(int companyId, bool isProduction)
        {
            //TODO:other providers
            IList<Tuple<string, decimal, string>> result = new List<Tuple<string, decimal, string>>();
            var serviceURL = CompanyService.GetValue(companyId, "Paybin.MainURL", isProduction);
            var publicKey = CompanyService.GetValue(companyId, "Paybin.PublicKey", isProduction);
            var apiKey = CompanyService.GetValue(companyId, "Paybin.XApiKey", isProduction);

            JObject obj = (JObject)JsonConvert.DeserializeObject(NW.Helper.HttpHelper.PostRequest(serviceURL + "merchant/balances", JsonConvert.SerializeObject(new { PublicKey = publicKey }), "application/json", new System.Collections.Specialized.NameValueCollection() { { "X-Api-Key", apiKey } }));

            result.Add(Tuple.Create("Paybin", Convert.ToDecimal(obj["data"]["btcBalance"]), "BTC"));
            return result;
        }

        private long? GetBonusAmount(int companyId, int providerId, int memberId, bool? withBonus, long depositAmount)
        {
            long? bonusAmount = null;

            if (withBonus.HasValue && withBonus.Value)
            {
                int percentage = DEFAULT_DEPOSIT_PERCENTAGE;


                var memberDetailList = MemberService.MemberDetails(memberId, MemberService.DepositBonusPercentage, MemberService.IntegrationVoltronId);
                int depositBonusInt = string.IsNullOrEmpty(memberDetailList[MemberService.DepositBonusPercentage]) ? percentage : int.Parse(memberDetailList[MemberService.DepositBonusPercentage]);



                if (providerId == 85 || providerId == 65 || providerId == 39 || providerId == 63 || providerId == 39)
                    depositBonusInt = 25;
                //if (providerId == 45 || providerId == 70 || providerId == 71 || providerId == 57)
                //    percentage = 25;
                //if (providerId == 85 || providerId == 65 || providerId == 39 || providerId == 63 || providerId == 39 || providerId == 71 || providerId == 70 || providerId == 61 || providerId == 45) //bayram - 29ekim
                //    depositBonusInt = 25;
                //if (providerId == 61 || providerId == 45)
                //    percentage = 22;


                decimal depositBonusPercentage = (decimal)depositBonusInt * 0.01m;


                bonusAmount = (long)(depositAmount * depositBonusPercentage);

            }
            return bonusAmount;
        }

        public CryptoExchangePrice GetPaybinCryptoExchangePrice(bool isProduction, string symbol)
        {
            string apiBaseUrl = isProduction ? "https://api.paybin.io" : "https://sandbox.paybin.io";
            string url = $"{apiBaseUrl}/v1/exchangeprices?symbol={symbol}";
            CryptoExchangePrice result = HttpServiceHelper.GetJsonRequest<CryptoExchangePrice>(url);
            return result;
        }
    }
}