
namespace LeagueSquadApi.Dtos
{
    public class RiotDtos
    {
        public record RiotAccountResponse(string Puuid, string GameName, string TagLine, string? Region);
        public record RiotAccountCoreResponse(string Puuid, string GameName, string TagLine);
        public record RiotAccountRegionResponse(string Puuid, string Game, string Region);
        public record RiotMatchResponse(string MatchId, int QueueId, List<string> ParticipantsIds, List<RiotMatchParticipant> Participants, string ParticipantsJson, string TimelineJson, DateTimeOffset GameStart, DateTimeOffset GameEnd, int DurationSeconds, string Mode, string GameType, int MapId);
        public record RiotMatchDto(RiotMatchMetadata Metadata, RiotMatchInfo Info);
        public record RiotMatchMetadata(string MatchId, List<string> Participants);
        public record RiotMatchInfo(int QueueId, List<RiotMatchParticipant> Participants, long GameStartTimestamp, long GameEndTimestamp, int GameDuration, string GameMode, string GameType, int MapId);
        public record RiotMatchParticipant(string Puuid, int TeamId, int ParticipantId, string TeamPosition, int ChampionId, int Kills, int Deaths, int Assists, bool Win);






        public record RiotHttpResult<T>
        {
            public int StatusCode { get; init; }
            public T Value { get; init; }

            public bool IsSuccessful
            {
                get { return StatusCode >= 200 && StatusCode <= 299; }
            }

            private RiotHttpResult() { }

            public static RiotHttpResult<T> Ok(T value, int statusCode = 200)
            {
                return new RiotHttpResult<T> { StatusCode = statusCode, Value = value };
            }
            public static RiotHttpResult<T> Fail(int statusCode, T value)
            {
                return new RiotHttpResult<T> { StatusCode = statusCode, Value = value };
            }
            public static RiotHttpResult<T> Fail(int statusCode)
            {
                return new RiotHttpResult<T> { StatusCode = statusCode };
            }
        }
        public record RiotHttpResult
        {
            public int StatusCode { get; init; }
            private RiotHttpResult() { }
            public bool IsSuccessful
            {
                get { return StatusCode >= 200 && StatusCode <= 299; }
            }

            public static RiotHttpResult Ok(int statusCode = 200)
            {
                return new RiotHttpResult { StatusCode = statusCode};
            }
            public static RiotHttpResult Fail(int statusCode)
            {
                return new RiotHttpResult { StatusCode = statusCode};
            }
        }
    }
}
