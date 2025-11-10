
namespace LeagueSquadApi.Dtos
{
    public record MatchRequest(string Id);
    public record MatchResponse(string Id, int QueueId, DateTimeOffset GameStart, DateTimeOffset GameEnd, int DurationSeconds, string Mode, string GameType, int MapId, DateTimeOffset CreatedAt);
}
