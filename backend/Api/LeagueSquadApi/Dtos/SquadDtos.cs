namespace LeagueSquadApi.Dtos
{
    public record SquadRequest(string Name);
    public record SquadResponse(long Id, string Name, DateTimeOffset CreatedAt);
}
