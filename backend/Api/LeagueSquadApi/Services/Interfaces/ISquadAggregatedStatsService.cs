using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;

namespace LeagueSquadApi.Services.Interfaces
{
    public interface ISquadAggregatedStatsService
    {
        Task<ServiceResult<List<SquadAggregatedStats>>> GetAsync(long squadId, IMatchAggregatedStatsService mas, IParticipantService ps, CancellationToken ct);
    }
}
