namespace LeagueSquadApi.Dtos
{
    public record MatchRequest(string Id);

    public record MatchResponse(
        string Id,
        int QueueId,
        DateTimeOffset GameStart,
        DateTimeOffset GameEnd,
        int DurationSeconds,
        string Mode,
        string GameType,
        int MapId,
        DateTimeOffset CreatedAt
    );

    public record MatchAggregatedStatsResponse(
        string MatchId,
        string StatsJson,
        DateTimeOffset CreatedAt
    );
}
