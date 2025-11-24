using LeagueSquadApi.Dtos;

namespace LeagueSquadApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<string>> LoginAsync(LoginRequest req, IUserService us, CancellationToken ct);
        Task<ServiceResult<UserResponse>> RegisterAsync(CreateUserRequest req, IUserService us, CancellationToken ct);
    }
}
