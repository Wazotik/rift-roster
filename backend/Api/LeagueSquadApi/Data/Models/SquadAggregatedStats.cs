using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueSquadApi.Data.Models
{
    [Table("squad_aggregated_stats")]
    public class SquadAggregatedStats
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("squad_id")]
        [ForeignKey(nameof(Squad))]
        public long SquadId { get; set; }

        [Column("stat_name")]
        public string StatName { get; set; } = null!;

        // this will just be puuid for now
        [Column("stat_squad_member")]
        public string StatSquadMember { get; set; } = null!;

        [Column("stat_value")]
        public double StatValue { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}