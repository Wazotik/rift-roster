namespace LeagueSquadApi.Dtos
{
    public record ParticipantResponse(
        string MatchId,
        int ParticipantId,
        string Puuid,
        int TeamId,
        string TeamPosition,
        int ChampionId,
        int Kills,
        int Deaths,
        int Assists,
        bool Win,
        string ParticipantsJson
    );
}
