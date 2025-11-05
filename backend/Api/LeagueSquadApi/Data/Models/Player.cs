using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueSquadApi.Data.Models
{
    [Table("player")]
    public class Player
    {
        [Key]
        [Column("puuid")]
        public string Id { get; set; } = null!;

        [Column("game_name")]
        public string GameName { get; set; } = string.Empty;

        [Column("tag_line")]
        public string TagLine { get; set; } = string.Empty;

        [Column("region")]
        public string? Region { get; set; }

        [Column("platform")]
        public string? Platform { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
