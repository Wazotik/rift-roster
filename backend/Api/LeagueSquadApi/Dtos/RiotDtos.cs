namespace LeagueSquadApi.Dtos
{
    public class RiotDtos
    {
        public record RiotAccountResponse(string Puuid, string GameName, string TagLine, string? Region, string? Platform);

    }
}
