using LeagueSquadApi.Services.Interfaces;
using System.Net;
using static LeagueSquadApi.Dtos.RiotDtos;

namespace LeagueSquadApi.Services
{
    public class RiotClient : IRiotClient
    {
        private readonly HttpClient http;

        public RiotClient(HttpClient http)
        {
            this.http = http;
        }

        public async Task<RiotHttpResult<RiotAccountResponse>> GetAccountByRiotIdAsync(string gameName, string tagLine, CancellationToken ct)
        {
            var urlCore = $"{http.BaseAddress}account/v1/accounts/by-riot-id/{Uri.EscapeDataString(gameName)}/{Uri.EscapeDataString(tagLine)}";
            var resCore = await http.GetAsync(urlCore, ct);
            if (!resCore.IsSuccessStatusCode) return RiotHttpResult<RiotAccountResponse>.Fail((int)resCore.StatusCode);
            var bodyCore = await resCore.Content.ReadFromJsonAsync<RiotAccountCoreResponse>(ct);
            if (bodyCore == null) return RiotHttpResult<RiotAccountResponse>.Fail((int)HttpStatusCode.NotFound);


            var urlRegion = $"{http.BaseAddress}account/v1/region/by-game/lol/by-puuid/{Uri.EscapeDataString(bodyCore.Puuid)}";
            var resRegion = await http.GetAsync(urlRegion, ct);
            if (!resRegion.IsSuccessStatusCode) return RiotHttpResult<RiotAccountResponse>.Fail((int)resCore.StatusCode);
            var bodyRegion = await resRegion.Content.ReadFromJsonAsync<RiotAccountRegionResponse>(ct);
            if (bodyRegion == null) return RiotHttpResult<RiotAccountResponse>.Fail((int)HttpStatusCode.NotFound);

            return RiotHttpResult<RiotAccountResponse>.Ok(new RiotAccountResponse(bodyCore.Puuid, bodyCore.GameName, bodyCore.TagLine, bodyRegion.Region));
        }
        public async Task<RiotHttpResult<RiotAccountResponse>> GetAccountByPuuidAsync(string puuid, CancellationToken ct)
        {
            var urlCore = $"{http.BaseAddress}account/v1/accounts/by-puuid/{Uri.EscapeDataString(puuid)}";
            var resCore = await http.GetAsync(urlCore, ct);
            if (!resCore.IsSuccessStatusCode) return RiotHttpResult<RiotAccountResponse>.Fail((int)resCore.StatusCode);
            var bodyCore = await resCore.Content.ReadFromJsonAsync<RiotAccountCoreResponse>(ct);
            if (bodyCore == null) return RiotHttpResult<RiotAccountResponse>.Fail((int)HttpStatusCode.NotFound);

            var urlRegion = $"{http.BaseAddress}account/v1/region/by-game/lol/by-puuid/{Uri.EscapeDataString(bodyCore.Puuid)}";
            Console.WriteLine(urlRegion);
            var resRegion = await http.GetAsync(urlRegion, ct);
            if (!resRegion.IsSuccessStatusCode) return RiotHttpResult<RiotAccountResponse>.Fail((int)resCore.StatusCode);
            var bodyRegion = await resRegion.Content.ReadFromJsonAsync<RiotAccountRegionResponse>(ct);
            if (bodyRegion == null) return RiotHttpResult<RiotAccountResponse>.Fail((int)HttpStatusCode.NotFound);

            return RiotHttpResult<RiotAccountResponse>.Ok(new RiotAccountResponse(bodyCore.Puuid, bodyCore.GameName, bodyCore.TagLine, bodyRegion.Region));
        }

    }
}
