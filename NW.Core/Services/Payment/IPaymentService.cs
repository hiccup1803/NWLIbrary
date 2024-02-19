using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW.Core.Contracts.Payment;
using NW.Core.Entities.Payment;
using NW.Core.Enum;
using NW.Core.Entities;
using NW.Core.Model;
using NW.Core.Model.Finance;
using NW.Core.Model.Payments;

namespace NW.Core.Services.Payment
{
    public interface IPaymentService
    {
        bool IsInRejectedBin(string domain, string filteredCardNo, bool isProduction);
        DepositResult DepositMoneyWithAdjustmentTypeId(string domain, int memberId, bool isProduction,
            decimal bonusAmount, decimal amountAllocatedForBonus, int transactionTypeId, int transactionId);
        DepositResult DepositWithCreditCard(string domain, int memberId, bool isProduction, Int64 amount, string cardHolderName,
            string cardNo, int expiryYear, int expiryMonth, string cvv, string userIp, bool force3D, bool withBonus, int? bonusId);
        DepositResult DepositWithCreditCardReyPay(string domain, int memberId, bool isProduction, Int64 amount, string cardHolderName, string cardNo, int expiryYear,
            int expiryMonth, string cvv, string userIp, bool force3D, bool withBonus, int? bonusId);
        DepositResult DepositWithCreditCardPayIn(string domain, int memberId, bool isProduction, Int64 amount, string cardHolderName, string cardNo, int expiryYear,
            int expiryMonth, string cvv, string userIp, bool force3D, bool withBonus, int? bonusId);
        
        int SelectCCProvider(int memberId, int companyId, bool isProduction);
        DepositResult DepositWithBPProvider(string domain, int memberId, bool isProduction, Int64 amount, string cardHolderName, string cardNo, int expiryYear,
            int expiryMonth, string cvv, string userIp, bool force3D);


        DepositResult StartDepositWithJetMethod(string domain, int memberId, bool isProduction, decimal amount, string currency, bool withBonus, int? bonusId);
        DepositResult DepositWithJetMethod(string domain, string username, int? memberId, long paymentId, string customerNumber, bool isProduction);
        DepositResult DepositWithBPCMethod(string bpcPaymentMethodType, string domain, int memberId, bool isProduction, Int64 amount, string currency, bool withBonus, int? bonusId, string nameOnCard = null, string cardNumber = null, string pin = null, string expirationDate = null, bool? isNewCard = null, string ip = null, string userAgent = null);

        DepositResult DepositWithPapara(string domain, string username, bool isProduction, string firstName, string lastName, string paparaAccountNumber, decimal amount, string currency);
        DepositResult DepositWithTrustpay(string domain, string username, bool isProduction, string firstName, string lastName, string userId, string processId, decimal amount, string currency);
        
        DepositResult WithdrawWithPapara(string domain, string username, bool isProduction, string paparaAccountNumber, decimal amount, string currency);
        DepositResult WithdrawWithCMT(string domain, string username, bool isProduction, string paparaAccountNumber, decimal amount, string currency);
        DepositResult WithdrawWithMefete(string domain, string username, bool isProduction, string paparaAccountNumber, decimal amount, string currency);
        DepositResult WithdrawWithParazula(string domain, string username, bool isProduction, string parazulaAccountNumber, decimal amount, string currency);
        DepositResult WithdrawWithPEP(string domain, string username, bool isProduction, string pepAccountNumber, decimal amount, string currency);
        DepositResult WithdrawWithPayfix(string domain, string username, bool isProduction, string payfixAccountNumber, decimal amount, string currency);
        WithdrawResult WithdrawWithJeton(string domain, string username, bool isProduction, string jetonAccountNumber, decimal amount, string currency);
        WithdrawResult WithdrawWithJetonVoucher(string domain, string username, bool isProduction, Int64 amount, string currency);
        WithdrawResult WithdrawWithCrypto(string domain, int memberId, bool isProduction, long amount, string userIp, string currency, string cryptoWalletAddress);
        
        DepositResult StartDepositWithPaymino(string domain, int memberId, bool isProduction, int bankId, string name, string surname, long amount);
        DepositResult CompletePayminoCallback(string domain, bool isProduction, int requestId);
        DepositResult DeclinePayminoCallback(string domain, bool isProduction, int requestId);
        DepositResult StartDepositWithPaymino2(string domain, int memberId, bool isProduction, string name, string surname, long amount, bool withBonus, int? bonusId, string userIp);
        DepositResult CompletePayminoCallback2(string domain, bool isProduction, int companyId, string jsonRequest);
        IList<PayminoRequest2> PendingPayminoRequest2List(int memberId);
        
        DepositResult StartDepositWithPaybin(string domain, int memberId, string symbol, bool isProduction, long amount, bool withBonus, int? bonusId);
        DepositResult CompletePaybinCallback(string domain, bool isProduction, int paybinRequestId, string uniqueId, decimal originalAmount, string symbol, int orderId, string signature, string callbackData, Dictionary<string, decimal> priceList);
        DepositResult VerifyPaybinWithdraw(string domain, bool isProduction, int referenceId, string uniqueId, decimal originalAmount, string symbol, int orderId, string address, string signature, string callbackData);
        CryptoExchangePrice GetPaybinCryptoExchangePrice(bool isProduction, string symbol);


        DepositResult SendSmsReyPay(string domain, int memberId, bool isProduction, string reference, string smsCode);
        DepositResult CheckSmsStatusReyPay(string domain, int memberId, bool isProduction, string reference);
        DepositResult CheckStatusReyPay(string domain, int memberId, bool isProduction, string reference);
        DepositResult SendSmsPayIn(string domain, int memberId, bool isProduction, string reference, string smsCode);
        DepositResult CheckSmsStatusPayIn(string domain, int memberId, bool isProduction, string reference);
        DepositResult CheckStatusPayIn(string domain, int memberId, bool isProduction, string reference);
        DepositResult CheckStatusJeton(string domain, int memberId, long paymentId);
        
        CreditCardRequest IsSmsStatusFlowReyPay(int memberId);
        CreditCardRequest IsStatusFlowReyPay(int memberId);

        IList<BankTransferRequest> BankTransferList(int memberId);
        IList<BankTransferRequest> SuccessfulBankTransferList(int memberId);
        IList<BankTransferRequest> PendingBankTransferList(int memberId);

        
        DepositResult CompleteBPCallback(string domain, bool isProduction, string jsonRequest);
        DepositResult CompleteJetonCallback(string domain, bool isProduction, string jsonRequest);
        DepositResult CompleteKingCommunityCallback(string domain, bool isProduction, string jsonRequest);
        DepositResult CompleteKingQRCallback(string domain, bool isProduction, int companyId, string jsonRequest);
        
        
        DepositResult CompleteCepbankCallback(string domain, int companyId, bool isProduction, string jsonRequest, int paymentProviderId);
        DepositResult CompleteTrustPayCallback(string domain, bool isProduction, int companyId, string jsonRequest);
        List<CreditCard> ExistingCreditCards(string domain, int memberId, bool isProduction);

        WithdrawRequestBankTransfer GetWithdrawRequestBankTransfer(int id);

        bool HasExistingCreditCards(string domain, int memberId, bool isProduction);
        CreditCard GetCardDetails(string domain, int memberId, int creditCardId, bool isProduction);
        ResultModel CompleteCreditCardDepositAfter3D(string domain, int providerId, int statusCode, long amount, long transactionId, string providerTransactionReference, bool isProduction);
        IList<BankModel> BankList();
        IList<BankAccount> BankAccountList(int memberId);
        BankAccount InsertBankAccount(int memberId, string identityNumber, string firstname, string lastname, string bank, string IBAN, string branchCode, string accountNumber, string currency);
        BankAccount UpdateBankAccount(BankAccount bankAccount);
        void DeleteBankAccount(int bankAccountId);
        BankAccount BankAccount(int memberId, int bankAccountId);
        BankAccount BankAccount(int bankAccountId);
        DepositResult WithdrawRequest(string domain, string language, string username, Dictionary<int, BankAccount> bankAccounts, decimal amount, bool isProduction);
        DepositResult WithdrawCancel(string domain, string username, long transactionRefId, bool isProduction);
        IList<Withdraw> PendingWithdrawListAndApprovedInLastDays(string domain, string username, bool isProduction);

        
        IList<BankModel> BankTransferBankAccountList();
        IList<BankModel> BankTransferBankAccountList(Member member);
        IList<BankModel> BankTransferBankAccountList(int memberId);
        IList<BankModel> BankTransferBankAccountListForLevel(Level level);
        IList<BankModel> BankTransferBankAccountListForLevel(int levelId);
        IList<BankModel> BankTransferBankAccountListForMember(int memberId);
        IList<BankModel> BankTransferBankAccountListForMember(Member member);
        IList<BankModel> BankTransferBankAccountOrderForMember(Member member, IList<BankModel> banks);
        void InsertBankTransferRequest(int memberId, int bankTransferBankAccountId, long amount, DateTime transferDate, int transferWayType, string identityNumber, string bank, string iban, string branchCode, string accountNumber, string note, string senderFullName, bool withBonus, int? bonusId, int companyId);
        Boolean UpdateBankTransferV2RequestPaymentStatusType(int Id, int PaymentStatusTypeId);

        IList<int> DisabledPaymentProviderList(int memberId);
        void UpdateDisabledProviderList(int memberId, IList<int> disabledProviderIdList);
        Currency GetCurrency(int id);
        IList<Currency> GetAllCurrencies();
        CurrencyRate GetCurrencyRate(int id);
        CurrencyRate GetCurrencyRate(int fromCurrencyId, int toCurrencyId);
        IList<CurrencyRate> GetAllCurrencyRates();
        CurrencyRate InsertCurrecyRate(CurrencyRate currencyRate);
        CurrencyRate UpdateCurrecyRate(CurrencyRate currencyRate);


        DepositResult DepositWithBPCommunityBank(string domain, int memberId, bool isProduction, int withdrawId, int amount, string bankCode, string currency, bool withBonus, int? bonusId);
        IList<Tuple<string, string>> GetBPCommunityBankList(string domain, bool isProduction);
        IList<Tuple<int, int, string>> GetBPCommunityActiveAmountList(string domain, bool isProduction, string bank, long? amount);


        DepositResult DepositWithKingCommunityBank(string domain, int memberId, bool isProduction, string bank, string refCode, int amount, string currency, bool withBonus, int? bonusId);
        IList<Tuple<string, string>> GetKingCommunityBankList(string domain, bool isProduction);
        IList<Tuple<string, int>> GetKingCommunityActiveAmountList(string domain, bool isProduction, string bank, long? amount);
        IList<DepositSummaryModel> GetLastDaysDepositList(int memberId);

        #region papara
        IList<PaparaTransferRequest> PaparaTransferList(int memberId);
        IList<PaparaTransferRequest> SuccessfulPaparaTransferList(int memberId);
        IList<PaparaTransferRequest> PendingPaparaTransferList(int memberId);
        IList<PaparaAccount> PaparaAccountList(int memberId);
        PaparaAccount InsertPaparaAccount(int memberId, string firstname, string lastname, string accountNumber, string currency);
        PaparaAccount UpdatePaparaAccount(PaparaAccount paparaAccount);
        void DeletePaparaAccount(int paparaAccountId);
        PaparaAccount PaparaAccount(int memberId, int paparaAccountId);
        PaparaAccount PaparaAccount(int paparaAccountId);

        IList<PaparaTransferPaparaAccount> PaparaTransferPaparaAccountList();
        IList<PaparaTransferPaparaAccount> PaparaTransferPaparaAccountList(Member member);
        IList<PaparaTransferPaparaAccount> PaparaTransferPaparaAccountList(int memberId);
        IList<PaparaTransferPaparaAccount> PaparaTransferPaparaAccountList(int memberId, int providerId);
        IList<PaparaTransferPaparaAccount> PaparaTransferPaparaAccountListForLevel(Level level);
        IList<PaparaTransferPaparaAccount> PaparaTransferPaparaAccountListForLevel(int levelId);
        void InsertPaparaTransferRequest(int memberId, int paparaTransferPaparaAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId);
        void UpdatePaparaTransferRequest(int paparaTransferPaparaRequestId, int paparaTransferPaparaAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId);
        int SelectPaparaProvider(int memberId, int companyId, bool isProduction, long? amount = null);
        NewPaymentProvider GetPaparaPaymentProvider(int memberId, int companyId, long amount, bool isProduction);
        PaparaTransferRequest CheckPaparaRequest(int memberId, int companyId, bool isProduction);
        PaparaTransferRequest CheckPaparaRequestAfterSubmit(int selectedPaparaTransferPaparaAccountId, int memberId, int companyId, bool isProduction);
        #endregion

        #region cmt

        IList<CMTTransferRequest> CMTTransferList(int memberId);
        IList<CMTTransferRequest> SuccessfulCMTTransferList(int memberId);
        IList<CMTTransferRequest> PendingCMTTransferList(int memberId);
        IList<CMTAccount> CMTAccountList(int memberId);
        CMTAccount InsertCMTAccount(int memberId, string firstname, string lastname, string accountNumber, string currency);
        CMTAccount UpdateCMTAccount(CMTAccount CMTAccount);
        void DeleteCMTAccount(int CMTAccountId);
        CMTAccount CMTAccount(int memberId, int CMTAccountId);
        CMTAccount CMTAccount(int CMTAccountId);

        IList<CMTTransferCMTAccount> CMTTransferCMTAccountList();
        IList<CMTTransferCMTAccount> CMTTransferCMTAccountList(Member member);
        IList<CMTTransferCMTAccount> CMTTransferCMTAccountList(int memberId);
        IList<CMTTransferCMTAccount> CMTTransferCMTAccountList(int memberId, int providerId);
        IList<CMTTransferCMTAccount> CMTTransferCMTAccountListForLevel(Level level);
        IList<CMTTransferCMTAccount> CMTTransferCMTAccountListForLevel(int levelId);
        void InsertCMTTransferRequest(int memberId, int CMTTransferCMTAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId);
        void UpdateCMTTransferRequest(int CMTTransferCMTRequestId, int CMTTransferCMTAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId);
        int SelectCMTProvider(int memberId, int companyId, bool isProduction);
        CMTTransferRequest CheckCMTRequest(int memberId, int companyId, bool isProduction);
        #endregion
        #region Mefete

        IList<MefeteTransferRequest> MefeteTransferList(int memberId);
        IList<MefeteTransferRequest> SuccessfulMefeteTransferList(int memberId);
        IList<MefeteTransferRequest> PendingMefeteTransferList(int memberId);
        IList<MefeteAccount> MefeteAccountList(int memberId);
        MefeteAccount InsertMefeteAccount(int memberId, string firstname, string lastname, string accountNumber, string currency);
        MefeteAccount UpdateMefeteAccount(MefeteAccount MefeteAccount);
        void DeleteMefeteAccount(int MefeteAccountId);
        MefeteAccount MefeteAccount(int memberId, int MefeteAccountId);
        MefeteAccount MefeteAccount(int MefeteAccountId);

        IList<MefeteTransferMefeteAccount> MefeteTransferMefeteAccountList();
        IList<MefeteTransferMefeteAccount> MefeteTransferMefeteAccountList(Member member);
        IList<MefeteTransferMefeteAccount> MefeteTransferMefeteAccountList(int memberId);
        IList<MefeteTransferMefeteAccount> MefeteTransferMefeteAccountList(int memberId, int providerId);
        IList<MefeteTransferMefeteAccount> MefeteTransferMefeteAccountListForLevel(Level level);
        IList<MefeteTransferMefeteAccount> MefeteTransferMefeteAccountListForLevel(int levelId);
        void InsertMefeteTransferRequest(int memberId, int MefeteTransferMefeteAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId);
        void UpdateMefeteTransferRequest(int MefeteTransferMefeteRequestId, int MefeteTransferMefeteAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId);
        int SelectMefeteProvider(int memberId, int companyId, bool isProduction);
        MefeteTransferRequest CheckMefeteRequest(int memberId, int companyId, bool isProduction);
#endregion
        #region Parazula

        IList<ParazulaTransferRequest> ParazulaTransferList(int memberId);
        IList<ParazulaTransferRequest> SuccessfulParazulaTransferList(int memberId);
        IList<ParazulaTransferRequest> PendingParazulaTransferList(int memberId);
        IList<ParazulaAccount> ParazulaAccountList(int memberId);
        ParazulaAccount InsertParazulaAccount(int memberId, string firstname, string lastname, string accountNumber, string currency);
        ParazulaAccount UpdateParazulaAccount(ParazulaAccount parazulaAccount);
        void DeleteParazulaAccount(int ParazulaAccountId);
        ParazulaAccount ParazulaAccount(int memberId, int parazulaAccountId);
        ParazulaAccount ParazulaAccount(int parazulaAccountId);

        IList<ParazulaTransferParazulaAccount> ParazulaTransferParazulaAccountList();
        IList<ParazulaTransferParazulaAccount> ParazulaTransferParazulaAccountList(Member member);
        IList<ParazulaTransferParazulaAccount> ParazulaTransferParazulaAccountList(int memberId);
        IList<ParazulaTransferParazulaAccount> ParazulaTransferParazulaAccountList(int memberId, int providerId);
        IList<ParazulaTransferParazulaAccount> ParazulaTransferParazulaAccountListForLevel(Level level);
        IList<ParazulaTransferParazulaAccount> ParazulaTransferParazulaAccountListForLevel(int levelId);
        void InsertParazulaTransferRequest(int memberId, int ParazulaTransferParazulaAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId);
        void UpdateParazulaTransferRequest(int parazulaTransferParazulaRequestId, int parazulaTransferParazulaAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId);
        int SelectParazulaProvider(int memberId, int companyId, bool isProduction);
        ParazulaTransferRequest CheckParazulaRequest(int memberId, int companyId, bool isProduction);
        #endregion
        #region PEP

        IList<PEPTransferRequest> PEPTransferList(int memberId);
        IList<PEPTransferRequest> SuccessfulPEPTransferList(int memberId);
        IList<PEPTransferRequest> PendingPEPTransferList(int memberId);
        IList<PEPAccount> PEPAccountList(int memberId);
        PEPAccount InsertPEPAccount(int memberId, string firstname, string lastname, string accountNumber, string currency);
        PEPAccount UpdatePEPAccount(PEPAccount PEPAccount);
        void DeletePEPAccount(int PEPAccountId);
        PEPAccount PEPAccount(int memberId, int PEPAccountId);
        PEPAccount PEPAccount(int PEPAccountId);

        IList<PEPTransferPEPAccount> PEPTransferPEPAccountList();
        IList<PEPTransferPEPAccount> PEPTransferPEPAccountList(Member member);
        IList<PEPTransferPEPAccount> PEPTransferPEPAccountList(int memberId);
        IList<PEPTransferPEPAccount> PEPTransferPEPAccountList(int memberId, int providerId);
        IList<PEPTransferPEPAccount> PEPTransferPEPAccountListForLevel(Level level);
        IList<PEPTransferPEPAccount> PEPTransferPEPAccountListForLevel(int levelId);
        void InsertPEPTransferRequest(int memberId, int PEPTransferPEPAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId);
        void UpdatePEPTransferRequest(int PEPTransferPEPRequestId, int PEPTransferPEPAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId);
        int SelectPEPProvider(int memberId, int companyId, bool isProduction);
        PEPTransferRequest CheckPEPRequest(int memberId, int companyId, bool isProduction);
        PEPTransferRequest CheckPEPRequestAfterSubmit(int memberId, int companyId, bool isProduction);
        #endregion
        #region providers
        IList<PaymentProvider> AllPaymentProviderList();
        IList<PaymentProvider> PaymentProviderListForCompany(int companyId, bool isProduction);
        IList<PaymentProvider> PaymentProviderListForMember(int memberId, int companyId, bool isProduction);
        PaymentProvider PaymentProviderForCompany(int providerId, int companyId, bool isProduction);

        NewPaymentProvider GetNewPaymentProvider(int parentProviderId, int memberId, IEnumerable<int> triedAndGotErrorPaymentProviderIdList);
        NewPaymentProvider GetNewPaymentProviderBySystemName(string systemName);
        #endregion
        #region generic deposit request
        DepositResult InsertGenericDepositRequest(int memberId, int providerId, long amount, bool withBonus, int? bonusId, int companyId, string note, Object payload);

        IList<GenericDepositRequest> PendingGenericDepositRequestList(int memberId, int providerId);
        IList<GenericDepositRequest> PendingGenericDepositRequestList(int memberId);
        IList<GenericDepositRequest> SuccessfulGenericDepositRequestList(int memberId);
        #endregion
        bool HasPendingRequestInBankTransferGroup(int memberId);
        #region new bank transfer request 

        int SelectBTProvider(int memberId, int companyId, bool isProduction);
        int? SelectNewBTProvider(NewBankTransferRequest newBankTransferRequest, bool isProduction);
        DepositResult InsertNewBankTransferRequest(int memberId, int providerId, long amount, string note, string firstName, string lastName, int bankId, bool isEFT, string identityNumber, bool withBonus, int? bonusId, int companyId);
        DepositResult UpdateNewBankTransferRequest(NewBankTransferRequest newBankTransferRequest);
        IList<NewBankTransferRequest> NewBankTransferRequestForMember(int memberId, int[] paymentStatusTypes);
        #endregion

        #region Payfix

        IList<PayfixTransferRequest> PayfixTransferList(int memberId);
        IList<PayfixTransferRequest> SuccessfulPayfixTransferList(int memberId);
        IList<PayfixTransferRequest> PendingPayfixTransferList(int memberId);
        IList<PayfixAccount> PayfixAccountList(int memberId);
        PayfixAccount InsertPayfixAccount(int memberId, string firstname, string lastname, string accountNumber, string currency);
        PayfixAccount UpdatePayfixAccount(PayfixAccount PayfixAccount);
        void DeletePayfixAccount(int PayfixAccountId);
        PayfixAccount PayfixAccount(int memberId, int PayfixAccountId);
        PayfixAccount PayfixAccount(int PayfixAccountId);

        IList<PayfixTransferPayfixAccount> PayfixTransferPayfixAccountList();
        IList<PayfixTransferPayfixAccount> PayfixTransferPayfixAccountList(Member member);
        IList<PayfixTransferPayfixAccount> PayfixTransferPayfixAccountList(int memberId);
        IList<PayfixTransferPayfixAccount> PayfixTransferPayfixAccountList(int memberId, int providerId);
        IList<PayfixTransferPayfixAccount> PayfixTransferPayfixAccountListForLevel(Level level);
        IList<PayfixTransferPayfixAccount> PayfixTransferPayfixAccountListForLevel(int levelId);
        void InsertPayfixTransferRequest(int memberId, int PayfixTransferPayfixAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId);
        void UpdatePayfixTransferRequest(int payfixTransferRequestId, int payfixTransferPayfixAccountId, int paymentStatusType, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId, string referenceId);
        int SelectPayfixProvider(int memberId, int companyId, bool isProduction);
        PayfixTransferRequest CheckPayfixRequest(int memberId, int companyId, bool isProduction);
        PayfixTransferRequest CheckPayfixRequestAfterSubmit(int memberId, int companyId, bool isProduction);
        PayfixPaymentProvider GetPayfixPaymentProvider(int memberId, int companyId, long amount, bool isProduction);
        #endregion
        #region BankTransferV2

        IList<BankTransferV2Request> BankTransferV2List(int memberId);
        IList<BankTransferV2Request> SuccessfulBankTransferV2List(int memberId);
        IList<BankTransferRequestV2Model> SuccessfulBankTransferRequestV2ModelList(int memberId);
        IList<BankTransferV2Request> PendingBankTransferV2List(int memberId);

        IList<BankTransferV2BankTransferAccount> BankTransferV2BankTransferAccountList();
        IList<BankTransferV2BankTransferAccount> BankTransferV2BankTransferAccountList(Member member);
        IList<BankTransferV2BankTransferAccount> BankTransferV2BankTransferAccountList(int memberId);
        IList<BankTransferV2BankTransferAccount> BankTransferV2BankTransferAccountList(int memberId, int providerId);
        IList<BankTransferV2BankTransferAccount> BankTransferV2BankTransferAccountListForLevel(Level level);
        IList<BankTransferV2BankTransferAccount> BankTransferV2BankTransferAccountListForLevel(int levelId);
        IList<BankTransferV2Request> CheckBankTransferV2RequestListIn20Minutes(int memberId);
        void InsertBankTransferV2Request(int memberId, long amount, string identityNumber, string firstname, string lastname, int requestBankId, bool fastEnabled, int companyId, bool isProduction);
        void UpdateBankTransferV2Request(int bankTransferV2BankTransferRequestId, int bankTransferV2BankTransferAccountId, long amount, string accountNumber, string firstname, string lastname, string note, bool withBonus, int? bonusId, int companyId);
        int? SelectBankTransferV2Provider(int memberId, int companyId, bool isProduction, long? amount = null, int? exceptProviderId = null);
        #endregion


        IList<NewPaymentProvider> NewPaymentProviderListForMember(int memberId, int companyId);
        IList<NewPaymentProvider> NewPaymentProviderList(int companyId, bool showAll = false);
        void UpdateNewPaymentProvider(NewPaymentProvider newPaymentProvider);

        IList<Tuple<string, decimal, string>> BalanceList(int companyId, bool isProduction);




    }
}
