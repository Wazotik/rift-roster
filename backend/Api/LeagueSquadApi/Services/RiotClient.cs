using LeagueSquadApi.Services.Interfaces;
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


        public async Task<RiotAccountResponse?> GetAccountByRiotIdAsync(string gameName, string tagLine, CancellationToken ct)
        {
            var urlCore = $"{http.BaseAddress}account/v1/accounts/by-riot-id/{Uri.EscapeDataString(gameName)}/{Uri.EscapeDataString(tagLine)}";
            var resCore = await http.GetAsync(urlCore, ct);
            resCore.EnsureSuccessStatusCode();
            var bodyCore = await resCore.Content.ReadFromJsonAsync<RiotAccountCoreResponse>(ct);
            if (bodyCore == null) throw new InvalidOperationException("Successful response but no usable payload");

            var urlRegion = $"{http.BaseAddress}account/v1/region/by-game/lol/{Uri.EscapeDataString(bodyCore.Puuid)}";
            var resRegion = await http.GetAsync(urlRegion, ct);
            resRegion.EnsureSuccessStatusCode();
            var bodyRegion = await resRegion.Content.ReadFromJsonAsync<RiotAccountRegionResponse>(ct);
            if (bodyRegion == null) throw new InvalidOperationException("Successful response but no usable payload");

            return new RiotAccountResponse(bodyCore.Puuid, bodyCore.GameName, bodyCore.TagLine, bodyRegion.Region);
        }
        public async Task<RiotAccountResponse?> GetAccountByPuuidAsync(string puuid, CancellationToken ct)
        {
            var urlCore = $"{http.BaseAddress}account/v1/accounts/by-riot-id/{Uri.EscapeDataString(puuid)}";
            var resCore = await http.GetAsync(urlCore, ct);
            resCore.EnsureSuccessStatusCode();
            var bodyCore = await resCore.Content.ReadFromJsonAsync<RiotAccountCoreResponse>(ct);
            if (bodyCore == null) throw new InvalidOperationException("Successful response but no usable payload");

            var urlRegion = $"{http.BaseAddress}account/v1/region/by-game/lol/{Uri.EscapeDataString(bodyCore.Puuid)}";
            var resRegion = await http.GetAsync(urlRegion, ct);
            resRegion.EnsureSuccessStatusCode();
            var bodyRegion = await resRegion.Content.ReadFromJsonAsync<RiotAccountRegionResponse>(ct);
            if (bodyRegion == null) throw new InvalidOperationException("Successful response but no usable payload");

            return new RiotAccountResponse(bodyCore.Puuid, bodyCore.GameName, bodyCore.TagLine, bodyRegion.Region);
        }

    }
}
