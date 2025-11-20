using LeagueSquadApi.Dtos;

namespace LeagueSquadApi.Services.Interfaces
{
    public interface IParticipantService
    {
        Task<ServiceResult<List<ParticipantResponse>>> GetAllAsync(
            string matchId,
            CancellationToken ct
        );
    }
}
