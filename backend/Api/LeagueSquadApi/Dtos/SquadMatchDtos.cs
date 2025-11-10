namespace LeagueSquadApi.Dtos
{
    public record SquadMatchResponse(long SquadId, string MatchId, string? ReasonForAddition, int QueueId, DateTimeOffset GameStart, DateTimeOffset GameEnd, int DurationSeconds, string Mode, string GameType, int MapId, DateTimeOffset CreatedAt);
}
