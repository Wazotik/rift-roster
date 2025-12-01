namespace LeagueSquadApi.Dtos
{
    public record SquadRequest(string Name, string IconUrl);
    public record SquadResponse(long Id, string Name, string IconUrl, DateTimeOffset CreatedAt);
    public record SquadAggregatedStatsResponse(long Id, long squadId, string Name, string StatName, string StatSquadMember, double StatValue, DateTimeOffset CreatedAt);
}
