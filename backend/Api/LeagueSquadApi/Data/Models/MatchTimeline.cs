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

        [Column("fetched_at")]
        public DateTimeOffset FetchedAt { get; set; }

        [Column("frame_interval_ms")]
        public int? FrameIntervalMs { get; set; }

        [Column("timeline_json")]
        public string TimelineJson { get; set; } = string.Empty;

    }
}
