using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;

namespace LeagueSquadApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult<UserResponse>> GetAsync(string Username, string Password, CancellationToken ct);
        Task<ServiceResult<UserResponse>> CreateAsync(CreateUserRequest req, CancellationToken ct);
        Task<ServiceResult<User>> FindAsync(string username, string password, CancellationToken ct); // not front facing so can use whole User entity
    }
}
