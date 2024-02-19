using Logging;
using NW.Core.Services.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW.Core.Entities.Payment;
using NW.Core.Work;
using NHibernate;
using NW.Core.Repositories.Payment;
using NW.Core.Repositories;
using System.Configuration;
using NW.Helper;
using NW.Helper.SMS;
using NW.Services;
using Newtonsoft.Json;
using RestSharp;
using NW.Security;
using System.Net;
using NW.Core.Enum;
using NW.Core.Services;

namespace NW.Service.Payment
{
    public class CepBankService : BaseService, ICepBankService
    {
        ICompanyService CompanyService { get; set; }
        IRepository<CepBank, int> CepBankRepository { get; set; }
        IRepository<CepBankRequest, int> CepBankRequestRepository { get; set; }
        IMemberRepository MemberRepository { get; set; }

        Logger Logger { get; set; }

        public CepBankService(IMemberRepository _memberRepository, ICompanyService _companyService, IRepository<CepBank, int> _cepBankRepository, 
            IRepository<CepBankRequest, int> _cepBankRequestRepository, IUnitOfWork _unitOfWork, ISession _session) : base(_unitOfWork, _session)
        {
            CompanyService = _companyService;
            CepBankRepository = _cepBankRepository;
            CepBankRequestRepository = _cepBankRequestRepository;
            MemberRepository = _memberRepository;
            Logger=new Logger("CB");
        }
        public IList<CepBank> CepBankList()
        {
            return CepBankRepository.GetAll().Where(cb => cb.StatusType == (int)StatusType.Active).ToList();
        }

        public string GetCepBankViewTemplateByCepBankId(int cepBankId)
        {
            return CepBankRepository.Get(cepBankId).ViewTemplate;
        }
        public void InsertCepBankRequest(string domain, int paymentProviderId, int cepBankId, int memberId, string senderId, string receipientId, string senderPhone, string receipientPhone, string receipientBirthday, string password, long amount, bool withBonus, int? bonusId, string providerRefId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                using(ITransaction transaction = uniOfWork.BeginTransaction(Session))
                {
                    CepBankRequestRepository.Insert(new CepBankRequest()
                    {
                        PaymentProviderId = paymentProviderId,
                        CepBankId = cepBankId,
                        MemberId = memberId,
                        SenderId = senderId, 
                        ReceipientId = receipientId,
                        SenderPhone = senderPhone,
                        ReceipientPhone = receipientPhone,
                        ReceipientBirthday = receipientBirthday,
                        Password = password,
                        Amount = amount,
                        WithBonus = withBonus,
                        BonusId = bonusId,
                        CreateDate = DateTime.UtcNow,
                        PaymentStatusType = (int)PaymentStatusType.Pending,
                        ProviderRefId = providerRefId
                    });
                    uniOfWork.Commit(transaction);

                    NWWebHook.PushBackOffice(domain, "g:bo", "", "Player made a deposit with cepbank amount of " + amount + " TL");
                }

                //var member = MemberRepository.Get(memberId);
                //Task.Run(() => SendSMSAsync(member.Username, amount, Logger));

            }
        }

        public IList<CepBankRequest> PendingCepBankRequestList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return CepBankRequestRepository.GetAll().Where(cb => cb.PaymentStatusType == 0 && cb.MemberId == memberId).ToList(); // pendings
            }
        }
        public IList<CepBankRequest> SuccessfulCepBankRequestList(int memberId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return CepBankRequestRepository.GetAll().Where(cb => cb.PaymentStatusType == (int)NW.Core.Entities.Payment.PaymentStatusType.Approved && cb.MemberId == memberId).ToList(); // pendings
            }
        }
        
        public IList<CepBankRequest> PendingCepBankRequestList(int memberId, int bankId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return CepBankRequestRepository.GetAll().Where(cb => cb.PaymentStatusType == 0 && cb.MemberId == memberId && cb.CepBankId == bankId).ToList(); // pendings
            }
        }
        //private void SendSMSAsync(string username,long amount, Logger logger)
        //{
        //    // add notification webhook here later, for now adding SMS notification statically.  @cem 20151121

        //    var sms= new SMSRequest();
        //    //sms.To = "447459321484";
        //    sms.To = "447851826464";
        //    sms.Message = "Cepbank request; User: " + username + ", amount: " + amount.ToString("N") + "TL";
        //    try
        //    {
        //        sms.Send();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Fatal("SMS Sending error " + ex.Message,ex);
        //    }
        //}
    }
}
