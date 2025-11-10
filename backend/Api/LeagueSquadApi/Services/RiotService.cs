using LeagueSquadApi.Dtos;
using LeagueSquadApi.Services.Interfaces;
using static LeagueSquadApi.Dtos.RiotDtos;

namespace LeagueSquadApi.Services
{
    public class RiotService : IRiotService
    {
        private readonly IRiotClient rc;

        public RiotService(IRiotClient rc)
        {
            this.rc = rc;
        }

        public async Task<ServiceResult<RiotAccountResponse>> GetAccountByPuuidAsync(string puuid, CancellationToken ct)
        {
            var res = await rc.GetAccountByPuuidAsync(puuid, ct);
            if (!res.IsSuccessful) return ServiceResult<RiotAccountResponse>.Fail(HttpStatusToResultStatusMapper.Map(res.StatusCode));
            return ServiceResult<RiotAccountResponse>.Ok(res.Value);
        }

        public async Task<ServiceResult<RiotAccountResponse>> GetAccountByRiotIdAsync(string gameName, string tagLine, CancellationToken ct)
        {
            var res = await rc.GetAccountByRiotIdAsync(gameName, tagLine, ct);
            if (!res.IsSuccessful) return ServiceResult<RiotAccountResponse>.Fail(HttpStatusToResultStatusMapper.Map(res.StatusCode));
            return ServiceResult<RiotAccountResponse>.Ok(res.Value);
        }

        public async Task<ServiceResult<List<string>>> GetMatchIdsAsync(string puuid, int count, CancellationToken ct)
        {
            var res = await rc.GetMatchIdsAsync(puuid, count, ct);
            if (!res.IsSuccessful) return ServiceResult<List<string>>.Fail(HttpStatusToResultStatusMapper.Map(res.StatusCode));
            return ServiceResult<List<string>>.Ok(res.Value);
        }

        public async Task<ServiceResult<RiotMatchResponse>> GetMatchAsync(string id, CancellationToken ct)
        {
            var res = await rc.GetMatchAsync(id, ct);
            if (!res.IsSuccessful) return ServiceResult<RiotMatchResponse>.Fail(HttpStatusToResultStatusMapper.Map(res.StatusCode));
            return ServiceResult<RiotMatchResponse>.Ok(res.Value);
        }
    }
}
