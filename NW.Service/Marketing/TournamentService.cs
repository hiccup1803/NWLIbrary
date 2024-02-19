using NHibernate;
using NW.Core.Entities;
using NW.Core.Entities.Marketing;
using NW.Core.Repositories;
using NW.Core.Services.Marketing;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Service.Marketing
{
    public class TournamentService : BaseService, ITournamentService
    {
        private DateTime trNow = DateTime.UtcNow.AddHours(3);
        IRepository<Tournament, int> TournamentRepository { get; set; }
        IRepository<TournamentGame, int> TournamentGameRepository { get; set; }
        IGameRepository GameRepository { get; set; }
        public TournamentService(IRepository<Tournament, int> _tournamentRepository, IRepository<TournamentGame, int> _tournamentGameRepository, IGameRepository _gameRepository, IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            TournamentRepository = _tournamentRepository;
            TournamentGameRepository = _tournamentGameRepository;
            GameRepository = _gameRepository;
        }

        public Tournament Tournament(int id)
        {
            return TournamentRepository.Get(id);
        }
        public PagingModel<Tournament> Tournaments(int companyId, int pageIndex, int pageSize)
        {

            PagingModel<Tournament> pagingModel = new PagingModel<Tournament>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = TournamentRepository.GetAll().Where(t => t.CompanyId == companyId).Count();
                    pagingModel.ItemList = Session.QueryOver<Tournament>()
                            .Where(t => t.CompanyId == companyId)
                            .OrderBy(t => t.StatusType).Desc
                            .ThenBy(p => p.EndDate).Desc
                            .ThenBy(p => p.DisplayOrder).Asc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public Tournament InsertTournament(Tournament tournament)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    tournament.CreateDate = DateTime.Now;
                    tournament = TournamentRepository.Insert(tournament);
                    unitOfWork.Commit(transaction);
                    return tournament;
                }
            }
        }
        public Tournament UpdateTournament(Tournament tournament)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    tournament = TournamentRepository.Update(tournament);
                    unitOfWork.Commit(transaction);
                    return tournament;
                }
            }
        }
        public TournamentGame TournamentGame(int id)
        {
            return TournamentGameRepository.Get(id);
        }
        public PagingModel<TournamentGame> TournamentGames(int pageIndex, int pageSize)
        {
            PagingModel<TournamentGame> pagingModel = new PagingModel<TournamentGame>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = TournamentGameRepository.GetAll().Count();
                    pagingModel.ItemList = Session.QueryOver<TournamentGame>()
                            .OrderBy(mt => mt.DisplayOrder).Asc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public IList<TournamentGame> TournamentGamesForTournament(Tournament tournament)
        {
            return TournamentGamesForTournament(tournament.Id);
        }
        public IList<TournamentGame> TournamentGamesForTournament(int tournamentId)
        {
            return TournamentGameRepository.GetAll().Where(tg => tg.TournamentId == tournamentId).ToList();
        }
        public TournamentGame InsertTournamentGame(TournamentGame tournamentGame)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    tournamentGame = TournamentGameRepository.Insert(tournamentGame);
                    unitOfWork.Commit(transaction);
                    return tournamentGame;
                }
            }
        }
        public void InsertTournamentGames(IList<TournamentGame> tournamentGames)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    foreach (TournamentGame tournamentGame in tournamentGames)
                    {
                        TournamentGameRepository.Insert(tournamentGame);
                    }
                    unitOfWork.Commit(transaction);
                }
            }

        }
        public TournamentGame UpdateTournamentGame(TournamentGame tournamentGame)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    tournamentGame = TournamentGameRepository.Update(tournamentGame);
                    unitOfWork.Commit(transaction);
                    return tournamentGame;
                }
            }
        }
        #region Web
        public IList<Tournament> GetTournaments(int companyId, bool isVip, int tournamentType)
        {
            return TournamentRepository.GetAll().Where(t =>
                                                    t.CompanyId == companyId
                                                    //&& t.IsVip == isVip
                                                    && t.StatusType == (int)NW.Core.Enum.StatusType.Active
                                                    )
                            
                            .OrderByDescending(t => t.StartDate)
                            .OrderBy(t => t.DisplayOrder).ToList();
        }
        public IList<Tournament> GetActiveTournaments(int companyId, bool isVip, int tournamentType)
        {
            return TournamentRepository.GetAll().Where(t =>
                                                    t.CompanyId == companyId
                                                    //&& t.IsVip == isVip
                                                    && t.StatusType == (int)NW.Core.Enum.StatusType.Active
                                                    && t.EndDate > DateTime.UtcNow
                                                    && t.StartDate <= DateTime.UtcNow
                                                    && t.TournamentType == tournamentType
                                                    )
                            .OrderBy(t => t.EndDate)
                            .ToList();
        }
        public IList<Tournament> GetPastTournaments(int companyId, bool isVip, int tournamentType)
        {
            return TournamentRepository.GetAll().Where(t =>
                                                    t.CompanyId == companyId
                                                    //&& t.IsVip == isVip
                                                    && t.StatusType == (int)NW.Core.Enum.StatusType.Active
                                                    && t.EndDate <= DateTime.UtcNow
                                                    && t.TournamentType == tournamentType
                                                    )
                            .OrderByDescending(t => t.EndDate)
                            .ToList();
        }
        public IList<Tournament> GetFutureTournaments(int companyId, bool isVip, int tournamentType)
        {
            return TournamentRepository.GetAll().Where(t =>
                                                    t.CompanyId == companyId
                                                    //&& t.IsVip == isVip
                                                    && t.StatusType == (int)NW.Core.Enum.StatusType.Active
                                                    && t.StartDate > DateTime.UtcNow
                                                    && t.TournamentType == tournamentType
                                                    )
                            .OrderBy(t => t.StartDate)
                            .ToList();
        }

        #endregion
    }
}
