using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueSquadApi.Data.Models
{
    [Table("match_timeline")]
    public class MatchTimeline
    {
        [Key]
        [ForeignKey(nameof(Match))]
        [Column("match_id")]
        public string Id { get; set; } = null!;

        [Column("timeline_json")]
        public string TimelineJson { get; set; } = "[]";

    }
}
