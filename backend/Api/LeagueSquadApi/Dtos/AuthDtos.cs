namespace LeagueSquadApi.Dtos
{
    public record LoginRequest(string Username, string Password);

    public class JwtOptions
    {
        public string Key { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public int ExpiresMinutes { get; set; }
    }
}
