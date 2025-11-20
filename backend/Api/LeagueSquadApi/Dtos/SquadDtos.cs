namespace LeagueSquadApi.Dtos
{
    public record SquadRequest(string Name, string IconUrl);
    public record SquadResponse(long Id, string Name, string IconUrl, DateTimeOffset CreatedAt);
}
