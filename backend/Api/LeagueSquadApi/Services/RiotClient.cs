using System.Net;
using System.Text.Json;
using LeagueSquadApi.Data.Models;
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

        public async Task<RiotHttpResult<RiotAccountResponse>> GetAccountByRiotIdAsync(
            string gameName,
            string tagLine,
            CancellationToken ct
        )
        {
            var urlCore =
                $"{http.BaseAddress}riot/account/v1/accounts/by-riot-id/{Uri.EscapeDataString(gameName)}/{Uri.EscapeDataString(tagLine)}";
            var resCore = await http.GetAsync(urlCore, ct);
            if (!resCore.IsSuccessStatusCode)
                return RiotHttpResult<RiotAccountResponse>.Fail((int)resCore.StatusCode);
            var bodyCore = await resCore.Content.ReadFromJsonAsync<RiotAccountCoreResponse>(ct);
            if (bodyCore == null)
                return RiotHttpResult<RiotAccountResponse>.Fail((int)HttpStatusCode.NotFound);

            var urlRegion =
                $"{http.BaseAddress}riot/account/v1/region/by-game/lol/by-puuid/{Uri.EscapeDataString(bodyCore.Puuid)}";
            var resRegion = await http.GetAsync(urlRegion, ct);
            if (!resRegion.IsSuccessStatusCode)
                return RiotHttpResult<RiotAccountResponse>.Fail((int)resCore.StatusCode);
            var bodyRegion = await resRegion.Content.ReadFromJsonAsync<RiotAccountRegionResponse>(
                ct
            );
            if (bodyRegion == null)
                return RiotHttpResult<RiotAccountResponse>.Fail((int)HttpStatusCode.NotFound);

            return RiotHttpResult<RiotAccountResponse>.Ok(
                new RiotAccountResponse(
                    bodyCore.Puuid,
                    bodyCore.GameName,
                    bodyCore.TagLine,
                    bodyRegion.Region
                )
            );
        }

        public async Task<RiotHttpResult<RiotAccountResponse>> GetAccountByPuuidAsync(
            string puuid,
            CancellationToken ct
        )
        {
            var urlCore =
                $"{http.BaseAddress}riot/account/v1/accounts/by-puuid/{Uri.EscapeDataString(puuid)}";
            var resCore = await http.GetAsync(urlCore, ct);
            if (!resCore.IsSuccessStatusCode)
                return RiotHttpResult<RiotAccountResponse>.Fail((int)resCore.StatusCode);
            var bodyCore = await resCore.Content.ReadFromJsonAsync<RiotAccountCoreResponse>(ct);
            if (bodyCore == null)
                return RiotHttpResult<RiotAccountResponse>.Fail((int)HttpStatusCode.NotFound);

            var urlRegion =
                $"{http.BaseAddress}riot/account/v1/region/by-game/lol/by-puuid/{Uri.EscapeDataString(bodyCore.Puuid)}";
            var resRegion = await http.GetAsync(urlRegion, ct);
            if (!resRegion.IsSuccessStatusCode)
                return RiotHttpResult<RiotAccountResponse>.Fail((int)resCore.StatusCode);
            var bodyRegion = await resRegion.Content.ReadFromJsonAsync<RiotAccountRegionResponse>(
                ct
            );
            if (bodyRegion == null)
                return RiotHttpResult<RiotAccountResponse>.Fail((int)HttpStatusCode.NotFound);

            return RiotHttpResult<RiotAccountResponse>.Ok(
                new RiotAccountResponse(
                    bodyCore.Puuid,
                    bodyCore.GameName,
                    bodyCore.TagLine,
                    bodyRegion.Region
                )
            );
        }

        public async Task<RiotHttpResult<List<string>>> GetMatchIdsAsync(
            string puuid,
            int count,
            CancellationToken ct
        )
        {
            var url =
                $"{http.BaseAddress}lol/match/v5/matches/by-puuid/{Uri.EscapeDataString(puuid)}/ids?start=0&count={count}";
            var res = await http.GetAsync(url, ct);
            if (!res.IsSuccessStatusCode)
                return RiotHttpResult<List<string>>.Fail((int)res.StatusCode);
            var body = await res.Content.ReadFromJsonAsync<List<string>>(ct);
            if (body == null)
            {
                return RiotHttpResult<List<string>>.Fail((int)HttpStatusCode.NotFound);
            }

            return RiotHttpResult<List<string>>.Ok(body);
        }

        public async Task<RiotHttpResult<RiotMatchResponse>> GetMatchAsync(
            string id,
            CancellationToken ct
        )
        {
            var url = $"{http.BaseAddress}lol/match/v5/matches/{Uri.EscapeDataString(id)}";
            var res = await http.GetAsync(url, ct);
            if (!res.IsSuccessStatusCode)
                return RiotHttpResult<RiotMatchResponse>.Fail((int)res.StatusCode);

            var bodyRaw = await res.Content.ReadAsStringAsync(ct);
            var body = await res.Content.ReadFromJsonAsync<RiotMatchDto>(ct);
            if (body == null)
                return RiotHttpResult<RiotMatchResponse>.Fail((int)HttpStatusCode.NotFound);

            using var doc = JsonDocument.Parse(bodyRaw);
            var participantsJson = doc
                .RootElement.GetProperty("info")
                .GetProperty("participants")
                .GetRawText();

            // get match timeline
            var urlTimeline =
                $"{http.BaseAddress}lol/match/v5/matches/{Uri.EscapeDataString(id)}/timeline";
            var resTimeline = await http.GetAsync(urlTimeline, ct);
            if (!resTimeline.IsSuccessStatusCode)
                return RiotHttpResult<RiotMatchResponse>.Fail((int)res.StatusCode);

            var timelineJson = await resTimeline.Content.ReadAsStringAsync(ct);

            var match = new RiotMatchResponse(
                MatchId: body!.Metadata.MatchId,
                QueueId: body.Info.QueueId,
                ParticipantsIds: body.Metadata.Participants,
                Participants: body.Info.Participants,
                ParticipantsJson: participantsJson,
                TimelineJson: timelineJson,
                GameStart: DateTimeOffset.FromUnixTimeMilliseconds(body.Info.GameStartTimestamp),
                GameEnd: DateTimeOffset.FromUnixTimeMilliseconds(body.Info.GameEndTimestamp),
                DurationSeconds: body.Info.GameDuration,
                Mode: body.Info.GameMode,
                GameType: body.Info.GameType,
                MapId: body.Info.MapId
            );

            return RiotHttpResult<RiotMatchResponse>.Ok(match);
        }

        //public async Task<RiotHttpResult<RiotMatchResponse>> GetMatchTimelineAsync(string id, CancellationToken ct)
    }
}
