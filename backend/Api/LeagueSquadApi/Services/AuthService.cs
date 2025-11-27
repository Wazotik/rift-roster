using LeagueSquadApi.Data;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Dtos.Enums;
using LeagueSquadApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LeagueSquadApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext db;
        private readonly JwtOptions jwt;

        public AuthService(AppDbContext db, IOptions<JwtOptions> jwtOptions)
        {
            this.db = db;
            this.jwt = jwtOptions.Value;
        }

        public async Task<ServiceResult<string>> LoginAsync(LoginRequest req, IUserService us, CancellationToken ct)
        {
            var resFindUser = await us.FindAsync(req.Username, req.Password, ct);
            if (!resFindUser.IsSuccessful) return ServiceResult<string>.Fail(resFindUser.Status, resFindUser.Message);

            var user = resFindUser.Value;
            if (user == null) return ServiceResult<string>.Fail(ResultStatus.Unknown);

            // if found, build list of claims (name identifier = userId, name, and role) 
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, (user.Id).ToString()),
                new(ClaimTypes.Name, user.Email),
                new(ClaimTypes.Role, user.Role ?? "User")
            };

            if (string.IsNullOrWhiteSpace(jwt.Key)) throw new InvalidOperationException("JWT Key is missing");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));    // get secret key, convert to bytes, wrap in crypto key object
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);     // use key to sign the 

            // create JWT object
            var jwtToken = new JwtSecurityToken(
                issuer: jwt.Issuer,
                audience: jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwt.ExpiresMinutes),
                signingCredentials: creds
            );

            var jwtTokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return ServiceResult<string>.Ok(jwtTokenString);
        }

        public async Task<ServiceResult<UserResponse>> RegisterAsync(CreateUserRequest req, IUserService us, CancellationToken ct)
        {
            var resCreateUser = await us.CreateAsync(req, ct);
            if (!resCreateUser.IsSuccessful) return ServiceResult<UserResponse>.Fail(resCreateUser.Status, resCreateUser.Message);
            var u = resCreateUser.Value;
            return ServiceResult<UserResponse>.Ok(new UserResponse(u.Username, u.Name, u.Email, u.Role, u.CreatedAt));
        }

    }
}
