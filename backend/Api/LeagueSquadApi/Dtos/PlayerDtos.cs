namespace LeagueSquadApi.Dtos
{
    public record PlayerResponse(string Id, string GameName, string TagLine, string? Region, string? Platform,  DateTimeOffset CreatedAt);
}
