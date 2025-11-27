namespace LeagueSquadApi.Dtos
{
    public record LoginRequest(string Username, string Password);

    public class JwtOptions
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int ExpiresMinutes { get; set; }
    }
}
