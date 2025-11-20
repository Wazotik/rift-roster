using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueSquadApi.Data.Models
{
    [Table("match_aggregated_stats")]
    public class MatchAggregatedStats
    {
        [Key]
        [ForeignKey(nameof(Match))]
        [Column("match_id")]
        public string MatchId { get; set; } = null!;

        [Column("stats_json")]
        public string StatsJson { get; set; } = "[]";

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
