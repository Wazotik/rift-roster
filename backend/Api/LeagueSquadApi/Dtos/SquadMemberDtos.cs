namespace LeagueSquadApi.Dtos
{
    public record SquadMemberResponse(long SquadId, string Puuid, string? Role, string? Alias, DateTimeOffset CreatedAt, string GameName, string TagLine, string? Region);
    public record SquadMemberRequest(string Puuid, string? Role, string? Alias);
}
