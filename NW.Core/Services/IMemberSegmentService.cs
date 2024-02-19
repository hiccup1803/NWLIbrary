using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface IMemberSegmentService
    {
        int FilterMemberCount(MemberTagFilter memberTagFilter);
        MemberSegment MemberSegment(int id);
        IList<MemberSegment> GetAllActiveMemberSegments();
        PagingModel<MemberSegment> GetMemberSegments(int pageIndex, int pageSize);
        MemberSegment InsertMemberSegment(MemberSegment memberSegment);
        void UpdateMemberSegment(MemberSegment memberSegment);
        IList<string> GetUsernameListByMemberSegmentId(int memberSegmentId);

        DateTime? GetLastCronRunDateTime(int memberSegmentId);
        int InsertMemberSegmentCronRunHistory(int memberSegmentId, int? queryResultCount, int? downgradeMemberCount, int? upgradeMemberCount);
        void UpdateMemberSegmentCronRunHistory(int id, int memberSegmentId, int? queryResultCount, int? downgradeMemberCount, int? upgradeMemberCount);
        //IList<MemberSegment> MemberSegmentsForMemberTagFilter(MemberTagFilter memberTagFilter);
        //IList<MemberSegment> MemberSegmentsForMemberTagFilter(int memberTagFilterId);
        int[] MemberIdsForMemberTagFilter(int memberTagFilterId);
        int[] MemberIdsForMemberTagFilter(MemberTagFilter memberTagFilter);
        bool IsFilterHasMember(int memberTagFilterId, int memberId);
        bool IsFilterHasMember(int[] memberSegmentIdList, int memberId);
        string[] MemberSegmentListByMemberId(int memberId);
        int CountMemberByMemberSegmentId(int memberSegmentId);
        DateTime LastRunDateByMemberSegmentId(int memberSegmentId);
        IList<Member> MembersForMemberTagFilter(int memberTagFilterId);
        IList<Member> MembersForMemberTagFilter(MemberTagFilter memberTagFilter);
    }
}
