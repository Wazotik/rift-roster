using LeagueSquadApi.Dtos;
using LeagueSquadApi.Services.Interfaces;
using static LeagueSquadApi.Dtos.RiotDtos;

namespace LeagueSquadApi.Services
{
    public class RiotService : IRiotService
    {
        public async Task<ServiceResult<RiotAccountResponse>> GetAccountByPuuidAsync(string puuid, IRiotClient rc, CancellationToken ct)
        {
            var res = await rc.GetAccountByPuuidAsync(puuid, ct);
            if (!res.IsSuccessful) return ServiceResult<RiotAccountResponse>.Fail(HttpStatusToResultStatusMapper.Map(res.StatusCode));
            return ServiceResult<RiotAccountResponse>.Ok(res.Value);
        }

        public async Task<ServiceResult<RiotAccountResponse>> GetAccountByRiotIdAsync(string gameName, string tagLine, IRiotClient rc, CancellationToken ct)
        {
            var res = await rc.GetAccountByRiotIdAsync(gameName, tagLine, ct);
            if (!res.IsSuccessful) return ServiceResult<RiotAccountResponse>.Fail(HttpStatusToResultStatusMapper.Map(res.StatusCode));
            return ServiceResult<RiotAccountResponse>.Ok(res.Value);
        }
    }
}
