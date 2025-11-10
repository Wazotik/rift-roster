using LeagueSquadApi.Data;
using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Services.Interfaces;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Dtos.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeagueSquadApi.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly AppDbContext db;
        private readonly IRiotClient riotClient;

        public PlayerService(AppDbContext db, IRiotClient riotClient)
        {
            this.db = db;
            this.riotClient = riotClient;
        }

        public async Task<ServiceResult<PlayerResponse>> UpsertWithRiotIdAsync(string gameName, string tagLine, CancellationToken ct)
        {
            Player p;
            var res = await riotClient.GetAccountByRiotIdAsync(gameName, tagLine, ct);
            var riotAccount = res.Value;
            if (riotAccount == null) return ServiceResult<PlayerResponse>.Fail(ResultStatus.NotFound);
            var existingPlayer = await db.Player.FindAsync(riotAccount.Puuid, ct);
            if (existingPlayer == null)
            {
                p = new Player() { Id = riotAccount.Puuid, GameName = riotAccount.GameName, TagLine = riotAccount.TagLine, Region = riotAccount.Region};
                await db.Player.AddAsync(p, ct);
                await db.SaveChangesAsync(ct);
            }
            else
            {
                existingPlayer.GameName = riotAccount.GameName;
                existingPlayer.TagLine = riotAccount.TagLine;
                existingPlayer.Region = riotAccount.Region;
                await db.SaveChangesAsync(ct);
                p = existingPlayer;
            }
            return ServiceResult<PlayerResponse>.Ok(new PlayerResponse(p.Id, p.GameName, p.TagLine, p.Region, p.CreatedAt));
        }

        public async Task<ServiceResult<PlayerResponse>> UpsertWithPuuidAsync(string puuid, CancellationToken ct)
        {
            Player p;

            var res = await riotClient.GetAccountByPuuidAsync(puuid, ct);
            var riotAccount = res.Value;
            if (riotAccount == null) return ServiceResult<PlayerResponse>.Fail(ResultStatus.NotFound);

            var existingPlayer = await db.Player.FindAsync(puuid, ct);
            if (existingPlayer == null)
            {
                p = new Player() { Id = riotAccount.Puuid, GameName = riotAccount.GameName, TagLine = riotAccount.TagLine, Region = riotAccount.Region };
                await db.Player.AddAsync(p, ct);
                await db.SaveChangesAsync(ct);
            }
            else
            {
                existingPlayer.GameName = riotAccount.GameName;
                existingPlayer.TagLine = riotAccount.TagLine;
                existingPlayer.Region = riotAccount.Region;
                p = existingPlayer;
                await db.SaveChangesAsync(ct);
            }
            return ServiceResult<PlayerResponse>.Ok(new PlayerResponse(p.Id, p.GameName, p.TagLine, p.Region, p.CreatedAt));
        }

        public async Task<ServiceResult<PlayerResponse>> GetAsync(string id, CancellationToken ct)
        {
            var p = await db.Player.Where(p => p.Id == id).FirstOrDefaultAsync(ct);
            if (p == null) return ServiceResult<PlayerResponse>.Fail(ResultStatus.NotFound);
            return ServiceResult<PlayerResponse>.Ok(new PlayerResponse(p.Id, p.GameName, p.TagLine, p.Region, p.CreatedAt));
        }

        public async Task<ServiceResult<List<PlayerResponse>>> GetAllAsync(CancellationToken ct)
        {
            var players = await db.Player.Select(p => new PlayerResponse(p.Id, p.GameName, p.TagLine, p.Region, p.CreatedAt)).ToListAsync(ct);
            return ServiceResult<List<PlayerResponse>>.Ok(players);
        }
    }
}
