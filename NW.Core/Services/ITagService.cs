using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface ITagService
    {
        Tag Tag(int id);
        IList<Tag> GetAllTags();
        PagingModel<Tag> GetTags(int pageIndex, int pageSize);
        Tag InsertTag(Tag tag);
        void UpdateTag(Tag tag);
        IList<Tag> SearchTags(string q);
        Tag GetTagByName(string tagName);
        IList<Tag> GetTagsForGame(int gameId, bool onlyActives = true);
        IList<Tag> GetTagsForGame(Game game, bool onlyActives = true);
        IList<Game> GetGamesForTag(int tagId, bool onlyActives = true);
        IList<Game> GetGamesForTag(Tag tag, bool onlyActives = true);
        GameTag GameTag(int id);
        GameTag GameTag(int gameId, int tagId);
        IList<GameTag> GetAllGameTags();
        IList<GameTag> GetGameTagsForGame(int gameId, bool onlyActives = true);
        IList<GameTag> GetGameTagsForGame(Game game, bool onlyActives = true);
        IList<GameTag> GetGameTagsForTag(int tagId, bool onlyActives = true);
        IList<GameTag> GetGameTagsForTag(Tag tag, bool onlyActives = true);
        void InsertGameTag(GameTag gameTag);
        void UpdateGameTag(GameTag gameTag);
        void ToggleGameTagStatus(int gameTagId);
        void ToggleGameTagStatus(GameTag gameTag);
        IList<Tag> GetTagsForMember(int memberId, bool onlyActives = true);
        IList<Tag> GetTagsForMember(Core.Entities.Member member, bool onlyActives = true);
        IList<Core.Entities.Member> GetMembersForTag(int tagId, bool onlyActives = true);
        IList<Core.Entities.Member> GetMembersForTag(Tag tag, bool onlyActives = true);
        MemberTag MemberTag(int id);
        MemberTag MemberTag(int memberId, int tagId);
        IList<MemberTag> GetAllMemberTags();
        IList<MemberTag> GetMemberTagsForMember(int memberId, bool onlyActives = true);
        IList<MemberTag> GetMemberTagsForMember(Core.Entities.Member member, bool onlyActives = true);
        IList<MemberTag> GetMemberTagsForTag(int tagId, bool onlyActives = true);
        IList<MemberTag> GetMemberTagsForTag(Tag tag, bool onlyActives = true);
        void InsertMemberTag(MemberTag memberTag);
        void UpdateMemberTag(MemberTag memberTag);
        void ToggleMemberTagStatus(int memberTagId);
        void ToggleMemberTagStatus(MemberTag memberTag);
        //IList<Member> GetMembersForTag(Tag tag, bool onlyActives = true);


        MemberTagFilter MemberTagFilter(int id);
        IList<MemberTagFilter> GetAllMemberTagFilters();
        PagingModel<MemberTagFilter> GetMemberTagFilters(int pageIndex, int pageSize);
        MemberTagFilter InsertMemberTagFilter(MemberTagFilter memberTagFilter);
        void UpdateMemberTagFilter(MemberTagFilter memberTagFilter);
    }
}
