using LeagueSquadApi.Dtos;

namespace LeagueSquadApi.Services.Interfaces
{
    public interface ISquadService
    {
        Task<ServiceResult<SquadResponse>> GetAsync(long id, CancellationToken ct);
        Task<ServiceResult<List<SquadResponse>>> GetAllAsync(CancellationToken ct);
        Task<ServiceResult<SquadResponse>> AddAsync(string name, CancellationToken ct);
        Task<ServiceResult<SquadResponse>> UpdateAsync(long id, string name, CancellationToken ct);
        Task<ServiceResult> DeleteAsync(long id, CancellationToken ct);
        Task<ServiceResult<SquadMemberResponse>> AddMemberAsync(long id, SquadMemberRequest req, IPlayerService ps, CancellationToken ct);
        Task<ServiceResult<List<SquadMemberResponse>>> GetAllMembersAsync(long id, CancellationToken ct);
        Task<ServiceResult> DeleteMemberAsync(long id, string puuid, CancellationToken ct);
        Task<ServiceResult<SquadMemberResponse>> GetMemberAsync(long id, string puuid, CancellationToken ct);
        Task<ServiceResult<List<SquadMatchResponse>>> GetSquadMatchesAsync(long id, IRiotService rs, IMatchService ms, ISquadMatchService sms, CancellationToken ct);
    }
}
