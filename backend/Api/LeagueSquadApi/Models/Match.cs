using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueSquadApi.Models
{
    [Table("match")]
    public class Match
    {
        [Key]
        [Column("match_id")]
        public string Id { get; set; } = null!;

        [Column("start_time")]
        public DateTimeOffset StartTime { get; set; }

        [Column("patch")]
        public string? Patch {  get; set; }

        [Column("queue")]
        public int? Queue {  get; set; }

        [Column("has_timeline")]
        public  Boolean? HasTimeline { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
