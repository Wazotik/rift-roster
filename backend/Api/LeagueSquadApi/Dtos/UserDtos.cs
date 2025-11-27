namespace LeagueSquadApi.Dtos
{
    public record CreateUserRequest(string Username, string Password, string Name, string Email);
    public record UserResponse(string Username, string Name, string Email, string Role, DateTimeOffset CreatedAt);
}
