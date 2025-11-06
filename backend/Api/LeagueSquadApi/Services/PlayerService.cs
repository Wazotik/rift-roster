using LeagueSquadApi.Data;
using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Services.Interfaces;
using LeagueSquadApi.Dtos;

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

        public async Task<PlayerResponse?> UpsertPlayerWithRiotIdAsync(string gameName, string tagLine, CancellationToken ct)
        {
            Player p;
            var riotAccount = await riotClient.GetAccountByRiotIdAsync(gameName, tagLine, ct);
            if (riotAccount == null) return null;
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

                p = existingPlayer;
                await db.SaveChangesAsync(ct);
            }
            return new PlayerResponse(p.Id, p.GameName, p.TagLine, p.Region, p.CreatedAt);
        }


        public async Task<PlayerResponse?> UpsertPlayerWithPuuidAsync(string puuid, CancellationToken ct)
        {
            Player p;
            var riotAccount = await riotClient.GetAccountByPuuidAsync(puuid, ct);
            if (riotAccount == null) return null;
            var existingPlayer = await db.Player.FindAsync(riotAccount.Puuid, ct);
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
            return new PlayerResponse(p.Id, p.GameName, p.TagLine, p.Region, p.CreatedAt);
        }
    }
}
