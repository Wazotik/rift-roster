using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;

namespace LeagueSquadApi.Services.Interfaces
{
    public interface IMatchAggregatedStatsService
    {
        Task<ServiceResult<MatchAggregatedStatsResponse>> GetAsync(
            string matchId,
            IParticipantService ps,
            CancellationToken ct
        );
    }
}
