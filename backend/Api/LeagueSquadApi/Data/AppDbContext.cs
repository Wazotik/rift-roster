using LeagueSquadApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LeagueSquadApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> User => Set<User>();
        public DbSet<Player> Player => Set<Player>();
        public DbSet<Match> Match => Set<Match>();
        public DbSet<MatchTimeline> MatchTimeline => Set<MatchTimeline>();
        public DbSet<Participant> Participant => Set<Participant>();
        public DbSet<Squad> Squad => Set<Squad>();
        public DbSet<SquadMember> SquadMember => Set<SquadMember>();
        public DbSet<SquadMatch> SquadMatch => Set<SquadMatch>();
        public DbSet<MatchAggregatedStats> MatchAggregatedStats => Set<MatchAggregatedStats>();
        public DbSet<SquadAggregatedStats> SquadAggregatedStats => Set<SquadAggregatedStats>();


        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Entity<User>().Property(x => x.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            mb.Entity<Player>().Property(x => x.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            mb.Entity<Match>().Property(x => x.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            mb.Entity<Squad>().Property(x => x.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            mb.Entity<SquadMember>().Property(x => x.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            mb.Entity<SquadMatch>().Property(x => x.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            mb.Entity<MatchAggregatedStats>().Property(x => x.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            mb.Entity<SquadAggregatedStats>().Property(x => x.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("now()").ValueGeneratedOnAdd();

            mb.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            mb.Entity<Participant>()
              .Property(x => x.ParticipantsJson)
              .HasColumnType("jsonb")
              .HasDefaultValueSql("'[]'::jsonb");

            mb.Entity<MatchTimeline>()
              .Property(x => x.TimelineJson)
              .HasColumnType("jsonb")
              .HasDefaultValueSql("'[]'::jsonb");

            mb.Entity<MatchAggregatedStats>()
              .Property(x => x.StatsJson)
              .HasColumnType("jsonb")
              .HasDefaultValueSql("'[]'::jsonb");

            mb.Entity<SquadMatch>()
                .HasOne<Squad>()
                .WithMany()
                .HasForeignKey(sm => sm.SquadId)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<SquadAggregatedStats>()
                .HasOne<Squad>()
                .WithMany()
                .HasForeignKey(sas => sas.SquadId)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<SquadMember>()
                .HasOne<Squad>()
                .WithMany()
                .HasForeignKey(sm => sm.SquadId)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<MatchTimeline>()
                .HasOne<Match>()
                .WithMany()
                .HasForeignKey(mt => mt.Id)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<Participant>()
                .HasOne<Match>()
                .WithMany()
                .HasForeignKey(p => p.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<MatchAggregatedStats>()
                .HasOne<Match>()
                .WithMany()
                .HasForeignKey(mas => mas.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
