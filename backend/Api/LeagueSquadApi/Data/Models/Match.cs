using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueSquadApi.Data.Models
{
    [Table("match")]
    public class Match
    {
        [Key]
        [Column("match_id")]
        public string Id { get; set; } = null!;

        [Column("queue_id")]
        public int QueueId { get; set; }

        [Column("game_start")]
        public DateTimeOffset GameStart { get; set; }

        [Column("game_end")]
        public DateTimeOffset GameEnd { get; set; }

        [Column("duration_seconds")]
        public int DurationSeconds {  get; set; }

        [Column("mode")]
        public string Mode { get; set; } = string.Empty;

        [Column("game_type")]
        public string GameType { get; set; } = string.Empty;

        [Column("map_id")]
        public  int MapId { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }  
    }
}
