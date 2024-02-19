using DbLocalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using NW.Core.Entities;
using NW.Core.Repositories;
using NW.Core.Services;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Service
{
    public class MemberSegmentService : BaseService, IMemberSegmentService
    {
        private IMemberService MemberService { get; set; }
        private ITagService TagService { get; set; }
        private IRepository<MemberSegment, int> MemberSegmentRepository { get; set; }
        private IRepository<MemberSegmentMember, int> MemberSegmentMemberRepository { get; set; }
        private IRepository<MemberSegmentCronRunHistory, int> MemberSegmentCronRunHistoryRepository { get; set; }

        public MemberSegmentService(
            IMemberService _memberService,
            ITagService _tagService,
            IRepository<MemberSegment, int> _memberSegmentRepository,
            IRepository<MemberSegmentMember, int> _memberSegmentMemberRepository,
            IRepository<MemberSegmentCronRunHistory, int> _memberSegmentCronRunHistoryRepository,
            IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            MemberService = _memberService;
            TagService = _tagService;
            MemberSegmentRepository = _memberSegmentRepository;
            MemberSegmentMemberRepository = _memberSegmentMemberRepository;
            MemberSegmentCronRunHistoryRepository = _memberSegmentCronRunHistoryRepository;
        }

        public MemberSegment MemberSegment(int id)
        {
            return MemberSegmentRepository.Get(id);
        }
        public IList<MemberSegment> GetAllActiveMemberSegments()
        {
            return MemberSegmentRepository.GetAll().Where(ms => ms.StatusType == 1).ToList();
        }
        public PagingModel<MemberSegment> GetMemberSegments(int pageIndex, int pageSize)
        {

            PagingModel<MemberSegment> pagingModel = new PagingModel<MemberSegment>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MemberSegmentRepository.GetAll().Count();
                    pagingModel.ItemList = Session.QueryOver<MemberSegment>().Where(ms => ms.StatusType == 1 || ms.StatusType == 0)
                            .OrderBy(t => t.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public MemberSegment InsertMemberSegment(MemberSegment memberSegment)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    memberSegment.CreateDate = DateTime.Now;
                    memberSegment.UpdateDate = DateTime.Now;
                    memberSegment = MemberSegmentRepository.Insert(memberSegment);
                    unitOfWork.Commit(transaction);
                    return memberSegment;
                }
            }
        }
        public void UpdateMemberSegment(MemberSegment memberSegment)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    memberSegment.UpdateDate = DateTime.Now;
                    memberSegment = MemberSegmentRepository.Update(memberSegment);
                    unitOfWork.Commit(transaction);
                }
            }
        }
        //public IList<MemberSegment> MemberSegmentsForMemberTagFilter(MemberTagFilter memberTagFilter)
        //{
        //    return MemberSegmentsForMemberTagFilter(memberTagFilter.Id);
        //}
        //public IList<MemberSegment> MemberSegmentsForMemberTagFilter(int memberTagFilterId)
        //{
        //    return MemberSegmentRepository.GetAll().Where(ms => ms.MemberTagFilterId == memberTagFilterId).ToList();
        //}
        public int FilterMemberCount(MemberTagFilter memberTagFilter)
        {
            using (SqlConnection conn = SqlResourceDataAccess.CreateConnection(false, null))
            {
                string countQuery = memberTagFilter.FilterQuery.Replace("*", "COUNT(*)");

                SqlCommand countCommand = conn.CreateCommand();
                countCommand.CommandText = countQuery;

                conn.Open();
                int count = (int)countCommand.ExecuteScalar();
                conn.Close();
                conn.Dispose();

                return count;
            }
        }
        public IList<string> GetUsernameListByMemberSegmentId(int memberSegmentId)
        {
            return MemberSegmentMemberRepository.GetAll().Where(msm => msm.MemberSegmentId == memberSegmentId && msm.StatusType == 1).ToList().Select(msm => MemberService.GetMember(msm.MemberId).Username).ToList();
        }
        public int[] MemberIdsForMemberTagFilter(int memberTagFilterId)
        {
            MemberTagFilter memberTagFilter = TagService.MemberTagFilter(memberTagFilterId);
            return MemberIdsForMemberTagFilter(memberTagFilter);

        }
        public int[] MemberIdsForMemberTagFilter(MemberTagFilter memberTagFilter)
        {
            List<int> memberIds = new List<int>();

            using (SqlConnection conn = SqlResourceDataAccess.CreateConnection(false, null))
            {
                string sqlQuery = memberTagFilter.FilterQuery.Replace("*", "DISTINCT MemberId");

                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = sqlQuery;

                conn.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    memberIds.Add(Convert.ToInt32(sqlDataReader["MemberId"]));
                }
                sqlDataReader.Close();
                sqlDataReader.Dispose();

                conn.Close();
                conn.Dispose();

                return memberIds.ToArray();
            }
        }
        public bool IsFilterHasMember(int[] memberSegmentIdList, int memberId)
        {
            return MemberSegmentMemberRepository.GetAll().Any(msm => memberSegmentIdList.Contains(msm.MemberSegmentId) && msm.StatusType == 1 && msm.MemberId == memberId);
        }


        public bool IsFilterHasMember(int memberTagFilterId, int memberId)
        {
            MemberTagFilter memberTagFilter = TagService.MemberTagFilter(memberTagFilterId);
            return MemberIdsForMemberTagFilter(memberTagFilter).Any(m => m == memberId);

        }

        public IList<NW.Core.Entities.Member> MembersForMemberTagFilter(int memberTagFilterId)
        {
            MemberTagFilter memberTagFilter = TagService.MemberTagFilter(memberTagFilterId);
            return MembersForMemberTagFilter(memberTagFilter);

        }
        public IList<NW.Core.Entities.Member> MembersForMemberTagFilter(MemberTagFilter memberTagFilter)
        {
            List<NW.Core.Entities.Member> members = new List<NW.Core.Entities.Member>();

            using (SqlConnection conn = SqlResourceDataAccess.CreateConnection(false, null))
            {
                string sqlQuery = memberTagFilter.FilterQuery.Replace("*", "DISTINCT MemberId");

                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = sqlQuery;

                conn.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    members.Add(MemberService.GetMember(Convert.ToInt32(sqlDataReader["MemberId"])));
                }
                sqlDataReader.Close();
                sqlDataReader.Dispose();

                conn.Close();
                conn.Dispose();

                return members.ToArray();
            }
        }

        public string[] MemberSegmentListByMemberId(int memberId)
        {
            return MemberSegmentMemberRepository.GetAll().Where(msm => msm.StatusType == 1 && msm.MemberId == memberId && msm.MemberSegment.StatusType == 1).Select(msm => msm.MemberSegment.Name).ToArray();
        }

        public int CountMemberByMemberSegmentId(int memberSegmentId)
        {
            return MemberSegmentMemberRepository.GetAll().Count(ms => ms.StatusType == 1 && ms.MemberSegmentId == memberSegmentId);
        }

        public DateTime LastRunDateByMemberSegmentId(int memberSegmentId)
        {
            var memberSegmentMember = MemberSegmentMemberRepository.GetAll().Where(msm => msm.MemberSegmentId == memberSegmentId).OrderByDescending(o => o.CreateDate).Take(1).FirstOrDefault();
            return memberSegmentMember != null ? memberSegmentMember.CreateDate : DateTime.MinValue;
        }
        public DateTime? GetLastCronRunDateTime(int memberSegmentId)
        {
            var memberSegmentCronRunHistory = MemberSegmentCronRunHistoryRepository.GetAll().OrderByDescending(msrh => msrh.CreateDate).FirstOrDefault(msrh => msrh.MemberSegmentId == memberSegmentId);
            return memberSegmentCronRunHistory != null ? memberSegmentCronRunHistory.CreateDate : new Nullable<DateTime>();
        }
        public int InsertMemberSegmentCronRunHistory(int memberSegmentId, int? queryResultCount, int? downgradeMemberCount, int? upgradeMemberCount)
        {
            MemberSegmentCronRunHistory memberSegmentCronRunHistory;
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    memberSegmentCronRunHistory = new MemberSegmentCronRunHistory() { MemberSegmentId = memberSegmentId, DowngradeMemberCount = downgradeMemberCount, UpgradeMemberCount = upgradeMemberCount, QueryResultCount = queryResultCount, CreateDate = DateTime.UtcNow };

                    memberSegmentCronRunHistory = MemberSegmentCronRunHistoryRepository.Insert(memberSegmentCronRunHistory);
                    transaction.Commit();
                }
            }
            return memberSegmentCronRunHistory.Id;
        }
        public void UpdateMemberSegmentCronRunHistory(int id, int memberSegmentId, int? queryResultCount, int? downgradeMemberCount, int? upgradeMemberCount)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    MemberSegmentCronRunHistory memberSegmentCronRunHistory = MemberSegmentCronRunHistoryRepository.Get(id);

                    memberSegmentCronRunHistory.MemberSegmentId = memberSegmentId;
                    memberSegmentCronRunHistory.QueryResultCount = queryResultCount;
                    memberSegmentCronRunHistory.DowngradeMemberCount = downgradeMemberCount;
                    memberSegmentCronRunHistory.UpgradeMemberCount = upgradeMemberCount;
                    MemberSegmentCronRunHistoryRepository.Update(memberSegmentCronRunHistory);
                    transaction.Commit();
                }
            }
        }
    }
}
