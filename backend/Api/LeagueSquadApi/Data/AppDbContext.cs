using LeagueSquadApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LeagueSquadApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Player> Player => Set<Player>();
        public DbSet<Match> Match => Set<Match>();
        public DbSet<MatchTimeline> MatchTimeline => Set<MatchTimeline>();
        public DbSet<Participant> Participant => Set<Participant>();
        public DbSet<Squad> Squad => Set<Squad>();
        public DbSet<SquadMember> SquadMember => Set<SquadMember>();
        public DbSet<SquadMatch> SquadMatch => Set<SquadMatch>();
    }

}
