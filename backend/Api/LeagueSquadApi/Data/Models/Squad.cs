using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LeagueSquadApi.Data.Models
{
    [Table("squad")]
    public class Squad
    {
        [Key]
        [Column("squad_id")]
        public long Id { get; set; }

        [ForeignKey(nameof(User))]
        [Column("creator_id")]
        public int CreatorId { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("icon_url")]
        public string IconUrl { get; set; } = string.Empty;

        [Column("squad_match_count")]
        public int SquadMatchCount { get; set; } = 0;

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
