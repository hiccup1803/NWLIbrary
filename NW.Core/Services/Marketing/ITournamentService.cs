using NW.Core.Entities;
using NW.Core.Entities.Marketing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services.Marketing
{
    public interface ITournamentService
    {
        Tournament Tournament(int id);
        PagingModel<Tournament> Tournaments(int companyId, int pageIndex, int pageSize);
        Tournament InsertTournament(Tournament tournament);
        Tournament UpdateTournament(Tournament tournament);
        TournamentGame TournamentGame(int id);
        PagingModel<TournamentGame> TournamentGames(int pageIndex, int pageSize);
        IList<TournamentGame> TournamentGamesForTournament(Tournament tournament);
        IList<TournamentGame> TournamentGamesForTournament(int tournamentId);
        TournamentGame InsertTournamentGame(TournamentGame tournamentGame);
        void InsertTournamentGames(IList<TournamentGame> tournamentGames);
        TournamentGame UpdateTournamentGame(TournamentGame tournamentGame);
        #region Web
        IList<Tournament> GetTournaments(int companyId, bool isVip, int tournamentType);
        IList<Tournament> GetActiveTournaments(int companyId, bool isVip, int tournamentType);
        IList<Tournament> GetPastTournaments(int companyId, bool isVip, int tournamentType);
        IList<Tournament> GetFutureTournaments(int companyId, bool isVip, int tournamentType);
        #endregion
    }
}
