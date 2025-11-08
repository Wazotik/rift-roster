using System.Net;

namespace LeagueSquadApi.Dtos
{
    public class RiotDtos
    {
        public record RiotAccountResponse(string Puuid, string GameName, string TagLine, string? Region);
        public record RiotAccountCoreResponse(string Puuid, string GameName, string TagLine);
        public record RiotAccountRegionResponse(string Puuid, string Game, string Region);
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
