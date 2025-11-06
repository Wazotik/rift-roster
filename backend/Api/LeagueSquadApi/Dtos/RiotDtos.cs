namespace LeagueSquadApi.Dtos
{
    public class RiotDtos
    {
        public record RiotAccountResponse(string Puuid, string GameName, string TagLine, string? Region);
        public record RiotAccountCoreResponse(string Puuid, string GameName, string TagLine);
        public record RiotAccountRegionResponse(string Puuid, string Game, string Region);
    }
}
