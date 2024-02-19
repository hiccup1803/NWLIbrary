using DbLocalization;
using NHibernate;
using NW.Core.Entities;
using NW.Core.Enum;
using NW.Core.Model;
using NW.Core.Repositories;
using NW.Core.Services;
using NW.Core.Work;
using NW.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Services
{ 
    public class ProviderService : BaseService, IProviderService
    {
        private object connectionId;

        IRepository<Provider, int> ProviderRepository { get; set; }
        private IProviderSettingRepository ProviderSettingRepository { get; set; }

        public ProviderService(IProviderSettingRepository _providerSettingRepository, 
        IRepository<Provider, int> _providerRepository, IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            ProviderSettingRepository = _providerSettingRepository;
            ProviderRepository = _providerRepository;
        }
        

        public virtual string GetValue(int providerId, int companyId, string key, bool isProduction)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                return ProviderSettingRepository.GetValue(providerId, companyId, key, isProduction);
            }
        }

        public void SetValue(int providerId, int companyId, string key, string value, bool isProduction)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    ProviderSettingRepository.SetValue(providerId, companyId, key, value, isProduction);
                    unitOfWork.Commit(transaction);
                }
            }
        }


        public Provider Provider(int id)
        {
            return ProviderRepository.Get(id);
        }
        public Provider GetProviderByVoltronProviderId(int voltronProviderId)
        {
            return ProviderRepository.GetAll().FirstOrDefault(p => p.VoltronProviderId == voltronProviderId);
        }
        public IList<Provider> GetAllProviders()
        {
            return ProviderRepository.GetAll().ToList();
        }
        public IList<Provider> GetAllProviders(int providerTypeId)
        {
            return ProviderRepository.GetAll().Where(p => p.ProviderTypeId == providerTypeId).ToList();
        }
        public void InsertProvider(Provider provider)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    ProviderRepository.Insert(provider);
                    unitOfWork.Commit(transaction);
                }
            }
        }
        public void UpdateProvider(Provider provider)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = unitOfWork.BeginTransaction(Session);
                ProviderRepository.Update(provider);
                unitOfWork.Commit(transaction);
            }
        }
        public int EnableProviderForLevel(int providerId, int levelId)
        {
            int result = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBt"].ConnectionString))
            {
                string sqlQuery = "DELETE FROM MemberDisabledPaymentMethod WHERE ProviderId = @providerId AND EXISTS (SELECT * from Member WHERE Member.LevelId = @levelId and Member.Id = MemberDisabledPaymentMethod.MemberId);";

                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.Parameters.Add(new SqlParameter("@providerId", providerId));
                sqlCommand.Parameters.Add(new SqlParameter("@levelId", levelId));
                sqlCommand.CommandText = sqlQuery;

                conn.Open();

                result = sqlCommand.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();
            }
            return result;

        }
        public int DisableProviderForLevel(int providerId, int levelId)
        {
            int result = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBt"].ConnectionString))
            {
                string sqlQuery = "INSERT INTO MemberDisabledPaymentMethod (ProviderId, MemberId, CreateDate) SELECT @providerId, Member.Id, getutcdate() FROM Member WHERE Member.LevelId = @levelId and NOT EXISTS (SELECT * from MemberDisabledPaymentMethod WHERE MemberDisabledPaymentMethod.MemberId = Member.Id AND MemberDisabledPaymentMethod.ProviderId = @providerId);";

                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.Parameters.Add(new SqlParameter("@providerId", providerId));
                sqlCommand.Parameters.Add(new SqlParameter("@levelId", levelId));
                sqlCommand.CommandText = sqlQuery;

                conn.Open();

                result = sqlCommand.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();
            }
            return result;
        }
    }
}