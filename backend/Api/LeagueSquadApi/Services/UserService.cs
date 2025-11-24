using LeagueSquadApi.Data;
using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Dtos.Enums;
using LeagueSquadApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LeagueSquadApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext db;
        private readonly IPasswordHasher<User> hasher;

        public UserService(AppDbContext db, IPasswordHasher<User> hasher)
        {
            this.db = db;
            this.hasher = hasher;
        }

        public async Task<ServiceResult<UserResponse>> GetAsync(string username, string password, CancellationToken ct)
        {
            var user = await db.User.Where(u => u.Username == username && u.Password == password).FirstOrDefaultAsync(ct);
            if (user == null) return ServiceResult<UserResponse>.Fail(ResultStatus.NotFound);
            return ServiceResult<UserResponse>.Ok(new UserResponse(user.Username, user.Name, user.Email, user.CreatedAt));
        }

        public async Task<ServiceResult<User>> FindAsync(string username, string password, CancellationToken ct)
        {
            // find user using username  
            var user = await db.User.SingleOrDefaultAsync(u => u.Username == username, ct);
            if (user == null) return ServiceResult<User>.Fail(ResultStatus.Unauthorized);

            // verify incoming password
            var verify = hasher.VerifyHashedPassword(user, user.Password, password);

            // check if hashpassowrd result is the same for incoming and saved password
            if (verify == PasswordVerificationResult.Failed) return ServiceResult<User>.Fail(ResultStatus.Unauthorized);
            return ServiceResult<User>.Ok(user);
        }


        public async Task<ServiceResult<UserResponse>> CreateAsync(CreateUserRequest req, CancellationToken ct)
        {

            var existing = await db.User.AnyAsync(u => u.Username == req.Username, ct);
            if (existing) return ServiceResult<UserResponse>.Fail(ResultStatus.Conflict, "Username already exists!");

            User u = new User() { Username = req.Username, Name = req.Name, Email = req.Email };
            var hashedPassword = hasher.HashPassword(u, req.Password);
            u.Password = hashedPassword;
            await db.User.AddAsync(u, ct);

            try
            {
                await db.SaveChangesAsync(ct);
                return ServiceResult<UserResponse>.Ok(new UserResponse(u.Username, u.Name, u.Email, u.CreatedAt));
            }
            catch (DbUpdateException ex)
            {
                // race-condition fallback
                return ServiceResult<UserResponse>.Fail(ResultStatus.Conflict, "Username already exists!");
            }
            catch (Exception ex)
            {
                return ServiceResult<UserResponse>.Fail(ResultStatus.Unknown, ex.Message);
            }
        }
    }
}
