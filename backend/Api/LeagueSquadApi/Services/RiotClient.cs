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
            var url = $"{http.BaseAddress}account/v1/accounts/by-riot-id/{Uri.EscapeDataString(gameName)}/{Uri.EscapeDataString(tagLine)}";

            var res = await http.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();

            var body = await res.Content.ReadFromJsonAsync<RiotAccountResponse>(ct);

            return body;
        }
        public async Task<RiotAccountResponse?> GetAccountByPuuidAsync(string puuid, CancellationToken ct)
        {
            var url = $"{http.BaseAddress}account/v1/accounts/by-puuid/{Uri.EscapeDataString(puuid)}";

            var res = await http.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();

            var body = await res.Content.ReadFromJsonAsync<RiotAccountResponse>(ct);

            return body;
        }

    }
}
