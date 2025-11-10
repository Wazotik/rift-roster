using LeagueSquadApi.Data.Models;
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


        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Player>().Property(x => x.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            mb.Entity<Match>().Property(x => x.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            mb.Entity<Squad>().Property(x => x.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            mb.Entity<SquadMember>().Property(x => x.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            mb.Entity<SquadMatch>().Property(x => x.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("now()").ValueGeneratedOnAdd();

            mb.Entity<Participant>()
              .Property(x => x.ParticipantsJson)
              .HasColumnType("jsonb")
              .HasDefaultValueSql("'[]'::jsonb");

            mb.Entity<MatchTimeline>()
              .Property(x => x.TimelineJson)
              .HasColumnType("jsonb")
              .HasDefaultValueSql("'[]'::jsonb");
        }



    }

}
