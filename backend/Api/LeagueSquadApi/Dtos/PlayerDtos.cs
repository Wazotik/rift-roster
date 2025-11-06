namespace LeagueSquadApi.Dtos
{
    public record PlayerResponse(string Id, string GameName, string TagLine, string? Region,  DateTimeOffset CreatedAt);
}
