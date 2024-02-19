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

namespace NW.Service.Tagging
{
    public class TagService : BaseService, ITagService
    {

        private IRepository<Tag, int> TagRepository { get; set; }
        private IRepository<GameTag, int> GameTagRepository { get; set; }
        private IRepository<MemberTag, int> MemberTagRepository { get; set; }
        private IRepository<Game, int> GameRepository { get; set; }
        private IRepository<Core.Entities.Member, int> MemberRepository { get; set; }
        private IRepository<MemberTagFilter, int> MemberTagFilterRepository { get; set; }

        public TagService(
            IRepository<Tag, int> _tagRepository,
            IRepository<GameTag, int> _gameTagRepository,
            IRepository<MemberTag, int> _memberTagRepository,
            IRepository<Game, int> _gameRepository,
            IRepository<Core.Entities.Member, int> _memberRepository,
            IRepository<MemberTagFilter, int> _memberTagFilterRepository,
            IUnitOfWork _unitOfWork,
            ISession _session)
            : base(_unitOfWork, _session)
        {
            TagRepository = _tagRepository;
            GameTagRepository = _gameTagRepository;
            MemberTagRepository = _memberTagRepository;
            GameRepository = _gameRepository;
            MemberRepository = _memberRepository;
            MemberTagFilterRepository = _memberTagFilterRepository;
        }

        public Tag Tag(int id)
        {
            return TagRepository.Get(id);
        }
        public IList<Tag> GetAllTags()
        {
            return TagRepository.GetAll().ToList();
        }
        public PagingModel<Tag> GetTags(int pageIndex, int pageSize)
        {
            PagingModel<Tag> pagingModel = new PagingModel<Tag>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = TagRepository.GetAll().Count();
                    pagingModel.ItemList = Session.QueryOver<Tag>()
                            .OrderBy(t => t.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;

        }
        public Tag InsertTag(Tag tag)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    tag.CreateDate = DateTime.Now;
                    tag = TagRepository.Insert(tag);
                    unitOfWork.Commit(transaction);
                    return tag;
                }
            }
        }
        public void UpdateTag(Tag tag)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    TagRepository.Update(tag);
                    unitOfWork.Commit(transaction);
                }
            }
        }
        public IList<Tag> SearchTags(string q)
        {
            return TagRepository.GetAll().Where(t => t.Name.Contains(q)).ToList();
        }
        public Tag GetTagByName(string tagName)
        {
            return TagRepository.GetAll().Where(t => t.Name.Equals(tagName)).FirstOrDefault();
        }
        public IList<Tag> GetTagsForGame(int gameId, bool onlyActives = true)
        {
            if (onlyActives)
            {
                return GameTagRepository.GetAll().Where(gt => gt.GameId == gameId && gt.Active == true).Select(gt => gt.Tag).ToList();
            }
            else
            {
                return GameTagRepository.GetAll().Where(gt => gt.GameId == gameId).Select(gt => gt.Tag).ToList();
            }
        }
        public IList<Tag> GetTagsForGame(Game game, bool onlyActives = true)
        {
            return GetTagsForGame(game.Id, onlyActives);
        }
        public IList<Game> GetGamesForTag(int tagId, bool onlyActives = true)
        {
            if (onlyActives)
            {
                return GameTagRepository.GetAll().Where(gt => gt.TagId == tagId && gt.Active == true).Select(gt => gt.Game).ToList();
            }
            else
            {
                return GameTagRepository.GetAll().Where(gt => gt.TagId == tagId).Select(gt => gt.Game).ToList();
            }

        }
        public IList<Game> GetGamesForTag(Tag tag, bool onlyActives = true)
        {
            return GetGamesForTag(tag.Id, onlyActives);
        }
        public GameTag GameTag(int id)
        {
            return GameTagRepository.Get(id);
        }
        public GameTag GameTag(int gameId, int tagId)
        {
            return GameTagRepository.GetAll().FirstOrDefault(gt => gt.GameId == gameId && gt.TagId == tagId);
        }
        public IList<GameTag> GetAllGameTags()
        {
            return GameTagRepository.GetAll().ToList();
        }
        public IList<GameTag> GetGameTagsForGame(int gameId, bool onlyActives = true)
        {
            if (onlyActives)
            {
                return GameTagRepository.GetAll().Where(gt => gt.GameId == gameId && gt.Active == true).ToList();
            }
            else
            {
                return GameTagRepository.GetAll().Where(gt => gt.GameId == gameId).ToList();
            }
        }
        public IList<GameTag> GetGameTagsForGame(Game game, bool onlyActives = true)
        {
            return GetGameTagsForGame(game.Id, onlyActives);
        }
        public IList<GameTag> GetGameTagsForTag(int tagId, bool onlyActives = true)
        {
            if (onlyActives)
            {
                return GameTagRepository.GetAll().Where(gt => gt.TagId == tagId && gt.Active == true).ToList();
            }
            else
            {
                return GameTagRepository.GetAll().Where(gt => gt.TagId == tagId).ToList();
            }
        }
        public IList<GameTag> GetGameTagsForTag(Tag tag, bool onlyActives = true)
        {
            return GetGameTagsForTag(tag.Id);
        }
        public void InsertGameTag(GameTag gameTag)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    gameTag.CreateDate = DateTime.Now;
                    gameTag.Game = GameRepository.Get(gameTag.GameId);
                    gameTag.Tag = TagRepository.Get(gameTag.TagId);
                    GameTagRepository.Insert(gameTag);
                    unitOfWork.Commit(transaction);
                }
            }
        }
        public void UpdateGameTag(GameTag gameTag)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    GameTagRepository.Update(gameTag);
                    unitOfWork.Commit(transaction);
                }
            }

        }
        public void ToggleGameTagStatus(int gameTagId)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    GameTag gameTag = GameTagRepository.Get(gameTagId);
                    gameTag.Active = !gameTag.Active;
                    GameTagRepository.Update(gameTag);
                    unitOfWork.Commit(transaction);
                }
            }
        }
        public void ToggleGameTagStatus(GameTag gameTag)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    gameTag.Active = !gameTag.Active;
                    GameTagRepository.Update(gameTag);
                    unitOfWork.Commit(transaction);
                }
            }
        }
        public IList<Tag> GetTagsForMember(int memberId, bool onlyActives = true)
        {
            if (onlyActives)
            {
                return MemberTagRepository.GetAll().Where(gt => gt.MemberId == memberId && gt.Active == true).Select(gt => gt.Tag).ToList();
            }
            else
            {
                return MemberTagRepository.GetAll().Where(gt => gt.MemberId == memberId).Select(gt => gt.Tag).ToList();
            }
        }
        public IList<Tag> GetTagsForMember(Core.Entities.Member member, bool onlyActives = true)
        {
            return GetTagsForMember(member.Id, onlyActives);
        }
        public IList<Core.Entities.Member> GetMembersForTag(int tagId, bool onlyActives = true)
        {
            if (onlyActives)
            {
                return MemberTagRepository.GetAll().Where(gt => gt.TagId == tagId && gt.Active == true).Select(gt => gt.Member).ToList();
            }
            else
            {
                return MemberTagRepository.GetAll().Where(gt => gt.TagId == tagId).Select(gt => gt.Member).ToList();
            }

        }
        public IList<Core.Entities.Member> GetMembersForTag(Tag tag, bool onlyActives = true)
        {
            return GetMembersForTag(tag.Id, onlyActives);
        }
        public MemberTag MemberTag(int id)
        {
            return MemberTagRepository.Get(id);
        }
        public MemberTag MemberTag(int memberId, int tagId)
        {
            return MemberTagRepository.GetAll().FirstOrDefault(gt => gt.MemberId == memberId && gt.TagId == tagId);
        }
        public IList<MemberTag> GetAllMemberTags()
        {
            return MemberTagRepository.GetAll().ToList();
        }
        public IList<MemberTag> GetMemberTagsForMember(int memberId, bool onlyActives = true)
        {
            if (onlyActives)
            {
                return MemberTagRepository.GetAll().Where(gt => gt.MemberId == memberId && gt.Active == true).ToList();
            }
            else
            {
                return MemberTagRepository.GetAll().Where(gt => gt.MemberId == memberId).ToList();
            }
        }
        public IList<MemberTag> GetMemberTagsForMember(Core.Entities.Member member, bool onlyActives = true)
        {
            return GetMemberTagsForMember(member.Id, onlyActives);
        }
        public IList<MemberTag> GetMemberTagsForTag(int tagId, bool onlyActives = true)
        {
            if (onlyActives)
            {
                return MemberTagRepository.GetAll().Where(gt => gt.TagId == tagId && gt.Active == true).ToList();
            }
            else
            {
                return MemberTagRepository.GetAll().Where(gt => gt.TagId == tagId).ToList();
            }
        }
        public IList<MemberTag> GetMemberTagsForTag(Tag tag, bool onlyActives = true)
        {
            return GetMemberTagsForTag(tag.Id);
        }
        public void InsertMemberTag(MemberTag memberTag)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    memberTag.Member = MemberRepository.Get(memberTag.MemberId);
                    memberTag.Tag = TagRepository.Get(memberTag.TagId);
                    memberTag.CreateDate = DateTime.Now;
                    MemberTagRepository.Insert(memberTag);
                    unitOfWork.Commit(transaction);
                }
            }
        }
        public void UpdateMemberTag(MemberTag memberTag)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    MemberTagRepository.Update(memberTag);
                    unitOfWork.Commit(transaction);
                }
            }

        }
        public void ToggleMemberTagStatus(int memberTagId)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    MemberTag memberTag = MemberTagRepository.Get(memberTagId);
                    memberTag.Active = !memberTag.Active;
                    MemberTagRepository.Update(memberTag);
                    unitOfWork.Commit(transaction);
                }
            }
        }
        public void ToggleMemberTagStatus(MemberTag memberTag)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    memberTag.Active = !memberTag.Active;
                    MemberTagRepository.Update(memberTag);
                    unitOfWork.Commit(transaction);
                }
            }
        }


        public MemberTagFilter MemberTagFilter(int id)
        {
            return MemberTagFilterRepository.Get(id);
        }
        public IList<MemberTagFilter> GetAllMemberTagFilters()
        {
            return MemberTagFilterRepository.GetAll().ToList();
        }
        public PagingModel<MemberTagFilter> GetMemberTagFilters(int pageIndex, int pageSize)
        {
            PagingModel<MemberTagFilter> pagingModel = new PagingModel<MemberTagFilter>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = TagRepository.GetAll().Count();
                    pagingModel.ItemList = Session.QueryOver<MemberTagFilter>()
                            .OrderBy(t => t.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public MemberTagFilter InsertMemberTagFilter(MemberTagFilter memberTagFilter)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    memberTagFilter.CreateDate = DateTime.Now;
                    memberTagFilter.UpdateDate = DateTime.Now;
                    memberTagFilter = MemberTagFilterRepository.Insert(memberTagFilter);
                    unitOfWork.Commit(transaction);
                    return memberTagFilter;
                }
            }
        }
        public void UpdateMemberTagFilter(MemberTagFilter memberTagFilter)
        {

            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    memberTagFilter.UpdateDate = DateTime.Now;
                    MemberTagFilterRepository.Update(memberTagFilter);
                    unitOfWork.Commit(transaction);
                }
            }
        }
    }
}
