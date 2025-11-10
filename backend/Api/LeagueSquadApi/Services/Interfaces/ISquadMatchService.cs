using LeagueSquadApi.Dtos;

namespace LeagueSquadApi.Services.Interfaces
{
    public interface ISquadMatchService
    {
        Task<ServiceResult<SquadMatchResponse>> AddAsync(long squadId, string matchId, string? ReasonForAddition, MatchResponse mr, CancellationToken ct);
    }
}
