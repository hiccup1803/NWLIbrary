using System.Data;
using System.Net;
using Newtonsoft.Json;
using NW.Core.Contracts.Game;
using NW.Core.Entities;
using NW.Core.Repositories;
using NW.Core.Services;
using NW.Core.Work;
using NW.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using RestSharp;
using NW.Security;
using NW.Core.Model;
using Newtonsoft.Json.Linq;
using AutoMapper;
using NW.Core.Model.Game;

namespace NW.Services
{
    public class GameService : BaseService, IGameService
    {
        IGameCategoryRepository GameCategoryRepository { get; set; }
        IGameRepository GameRepository { get; set; }
        ICategoryRepository CategoryRepository { get; set; }
        //IRepository<GameSortable, int> GameSortableRepository { get; set; }
        ICompanyDomainRepository CompanyDomainRepository { get; set; }
        ICompanyService CompanyService { get; set; }
        IMemberRepository MemberRepository { get; set; }
        IRepository<CustomStuff, int> CustomStuffRepository { get; set; }


        public GameService(IMemberRepository _memberRepository, IGameCategoryRepository _gameCategoryRepository,
            IGameRepository _gameRepository,
            ICategoryRepository _categoryRepository, ICompanyService _companyService, ICompanyDomainRepository _companyDomainRepository, IRepository<CustomStuff, int> _customStuffRepository,
            IUnitOfWork _unitOfWork, ISession _session) : base(_unitOfWork, _session)
        {
            GameCategoryRepository = _gameCategoryRepository;
            GameRepository = _gameRepository;
            CategoryRepository = _categoryRepository;
            //GameSortableRepository = _gameSortableRepository;
            CompanyDomainRepository = _companyDomainRepository;
            CompanyService = _companyService;
            MemberRepository = _memberRepository;
            CustomStuffRepository = _customStuffRepository;
        }

        public virtual WinnerListResult GetWinners(string domain, bool isProduction)
        {
            WinnerListResult result = new WinnerListResult { IsSuccess = false, Message = "" };
            using (var uniOfWork = UnitOfWork.Current)
            {


                int? companyId = CompanyDomainRepository.CompanyId(domain);
                //var member = MemberRepository.Member(comapanyId.Value, emailOrUsername, (int)StatusType.Active);

                var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);
                var method = "GetWinners";

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                int numberOfRecordToDisplay = 40;
                var request = new RestRequest(method, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername);
                request.AddParameter("take", numberOfRecordToDisplay);
                request.AddParameter("checksum", SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername, numberOfRecordToDisplay.ToString()));

                // execute the request
                var response = client.Execute(request);
                //var content = response.Content; // raw content as string

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<WinnerListResult>(response.Content); ;
                    //result.BonusBalance = response.Data["BonusBalance"];
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "Status code: " + response.StatusCode + ", Err: " + response.ErrorMessage;
                }
            }
            return result;
        }

        public virtual GamePagingListModel GamesByCategoryId(string domain, bool isProduction, int categoryId, string countryCode, bool isMobile, int skip, int take, bool needToGetClosedGames)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                unitOfWork.BeginTransaction(Session, IsolationLevel.ReadUncommitted);


                GamePagingListModel gamePagingListModel = new GamePagingListModel();
                //int[] closedGames = needToGetClosedGames || categoryId == AllGamesCategory.Id ? ClosedGames(domain, isProduction) : new int[] { };
                int[] closedGames = new int[] { };

                IQueryable<Game> gameListQuery;

                if (categoryId != AllGamesCategory.Id)
                {
                    gameListQuery = GameCategoryRepository.GamesByCategoryId(categoryId, isMobile).Where(g => !g.RestrictedCountries.Any(rc => rc.A2Code == countryCode) && !closedGames.Contains(g.VoltronGameId));

                    gamePagingListModel.GameList = gameListQuery.Skip(skip).Take(take).ToList().Select(g => Mapper.Map<Game, GameModel>(g)).ToList();
                    gamePagingListModel.GameCount = gameListQuery.Count();
                    return gamePagingListModel;
                }
                else
                {

                    gamePagingListModel.GameList = GameRepository.GetAll().Where(gs => gs.Active && gs.IsMobile == isMobile && !gs.RestrictedCountries.Any(rc => rc.A2Code == countryCode)).OrderByDescending(g => g.CreateDate).Skip(skip).Take(take).Select(g => Mapper.Map<Game, GameModel>(g)).ToList();
                    gamePagingListModel.GameCount = GameRepository.GetAll().Where(gs => gs.Active && gs.IsMobile == isMobile && !gs.RestrictedCountries.Any(rc => rc.A2Code == countryCode)).Count();
                    return gamePagingListModel;

                }

            }
        }
        public virtual GameModel Game(string seoURL)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                unitOfWork.BeginTransaction(Session, IsolationLevel.ReadUncommitted);
                return Mapper.Map<Game, GameModel>(GameRepository.Game(seoURL));
            }
        }

        public virtual GameModel Game(int gameId)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                unitOfWork.BeginTransaction(Session, IsolationLevel.ReadUncommitted);
                return Mapper.Map<Game, GameModel>(GameRepository.Game(gameId));
            }
        }
        public virtual GameModel GameByVoltronGameId(int voltronGameId)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                unitOfWork.BeginTransaction(Session, IsolationLevel.ReadUncommitted);
                return Mapper.Map<Game, GameModel>(GameRepository.GameByVoltronGameId(voltronGameId));
            }
        }

        public virtual IList<GameModel> GetAllActiveGameList(string domain)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                unitOfWork.BeginTransaction(Session, IsolationLevel.ReadUncommitted);
                var obj = GameRepository.GetAll().Where(w => w.Active).ToList();
                var gameModelList = Mapper.Map<IList<Game>, IList<GameModel>>(obj);
                return gameModelList;
            }
        }

        public virtual IList<GameModel> Games(IList<int> gameIds)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                unitOfWork.BeginTransaction(Session, IsolationLevel.ReadUncommitted);
                var obj = GameRepository.GetAll().Where(w => gameIds.Contains(w.Id)).Distinct().ToList();
                var gameModelList = Mapper.Map<IList<Game>, IList<GameModel>>(obj);
                return gameIds.Select(id => gameModelList.FirstOrDefault(gm => gm.Id == id)).ToList();
            }
        }

        public IList<GameModel> GamesByResourceName(string domain, IList<string> resourceNames, string countryCode, bool isMobile, int skip, int take)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                unitOfWork.BeginTransaction(Session, IsolationLevel.ReadUncommitted);

                int? companyId = CompanyDomainRepository.CompanyId(domain);

                return GameRepository.GetAll().Where(g => g.Active == true && g.IsMobile == isMobile && !g.RestrictedCountries.Any(rc => rc.A2Code == countryCode) && resourceNames.Contains(g.ResourceName) && g.GameCategories.Any(gc => gc.Category.CompanyId == companyId.Value)).Skip(skip).Take(take).ToList().Select(g => Mapper.Map<Game, GameModel>(g)).ToList();
            }
        }

        public virtual int GameCount(int categoryId, string countryCode, bool isMobile)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                unitOfWork.BeginTransaction(Session, IsolationLevel.ReadUncommitted);

                IQueryable<GameCategory> query = GameCategoryRepository.GetAll().Where(gc => gc.Game.IsMobile == isMobile && !gc.Game.RestrictedCountries.Any(rc => rc.A2Code == countryCode));

                if (categoryId != AllGamesCategory.Id)
                {
                    Category category = CategoryRepository.Get(categoryId);
                    query = query.Where(gc => gc.CategoryId == category.Id);
                }


                return query.Count();
            }
        }

        public IList<GameModel> FavouritedGames(int memberId, string countryCode, bool isMobile, int skip, int take)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                unitOfWork.BeginTransaction(Session, IsolationLevel.ReadUncommitted);

                Member member = MemberRepository.Get(memberId);

                return member.FavoriteGames.Where(g => g.Active == true && g.IsMobile == isMobile && !g.RestrictedCountries.Any(rc => rc.A2Code == countryCode)).Skip(skip).Take(take).ToList().Select(g => Mapper.Map<Game, GameModel>(g)).ToList();
            }
        }

        public int FavouritedGameCount(int memberId, string countryCode, bool isMobile)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                unitOfWork.BeginTransaction(Session, IsolationLevel.ReadUncommitted);

                Member member = MemberRepository.Get(memberId);
                return member.FavoriteGames.Count(g => g.IsMobile == isMobile && !g.RestrictedCountries.Any(rc => rc.A2Code == countryCode));
            }
        }

        public int SearchGameCount(string searchTerm, string countryCode, bool isMobile)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                unitOfWork.BeginTransaction(Session, IsolationLevel.ReadUncommitted);

                return GameRepository.GetAll().Count(g => g.IsMobile == isMobile && g.Name.Contains(searchTerm) && !g.RestrictedCountries.Any(rc => rc.A2Code == countryCode));
            }
        }

        public virtual int[] ClosedGames(string domain, bool isProduction)
        {
            int[] ids = new int[] { };
            using (var uniOfWork = UnitOfWork.Current)
            {


                int? companyId = CompanyDomainRepository.CompanyId(domain);
                //var member = MemberRepository.Member(comapanyId.Value, emailOrUsername, (int)StatusType.Active);

                var partnerUrl = CompanyService.GetValue(companyId.Value, "Voltron.ServiceURL", isProduction);
                var vCompanyId = CompanyService.GetValue(companyId.Value, "Voltron.CompanyId", isProduction);
                var apiUsername = CompanyService.GetValue(companyId.Value, "Voltron.APIUsername", isProduction);
                var apiPassword = CompanyService.GetValue(companyId.Value, "Voltron.APIPassword", isProduction);
                var method = "ClosedGameIds";

                var client = new RestClient(partnerUrl);
                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                var request = new RestRequest(method, Method.GET);
                request.AddParameter("companyId", vCompanyId); // adds to POST or URL querystring based on Method
                request.AddParameter("apiUsername", apiUsername);
                request.AddParameter("checksum", SecurityHelper.CalculateMD5HashWithPrivateKey(apiPassword, vCompanyId, apiUsername));

                // execute the request
                var response = client.Execute(request);
                //var content = response.Content; // raw content as string

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ids = ((JArray)JsonConvert.DeserializeObject<dynamic>(response.Content).ClosedGameIds).Values<int>().ToArray();
                    //result.BonusBalance = response.Data["BonusBalance"];
                }
            }
            return ids;
        }



        public GameModel GetMobileVersion(GameModel game)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                unitOfWork.BeginTransaction(Session, IsolationLevel.ReadUncommitted);
                return Mapper.Map<Game, GameModel>(GameRepository.MobileGameByDesktopAlias(game.Alias, game.Vendor));
            }
        }
        public GameModel GetDesktopVersion(GameModel game)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                unitOfWork.BeginTransaction(Session, IsolationLevel.ReadUncommitted);
                return Mapper.Map<Game, GameModel>(GameRepository.DesktopGameByMobileAlias(game.Alias, game.Vendor));
            }
        }

    }
}
